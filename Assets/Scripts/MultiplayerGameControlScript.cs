using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class MultiplayerGameControlScript : MonoBehaviourPunCallbacks
{
    Text playPanelText;
    GameObject playPanelImage;
    public static bool loading = false;

    void Start()
    {
        playPanelText = UsefulReferences.ui.transform.Find("Main Menu").Find("Panels").Find("Play").Find("NetworkClientStateText").gameObject.GetComponent<Text>();
        playPanelImage = UsefulReferences.ui.transform.Find("Main Menu").Find("Panels").Find("Play").Find("SpiderImage").gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && PhotonNetwork.NetworkClientState.ToString() == "Joined")
        {
            PlayerInfo.DebugPlayersList();
        }

        if (loading)
        {
            playPanelText.text = PhotonNetwork.NetworkClientState.ToString();
            playPanelImage.transform.Rotate(Vector3.back * 10);
        }
        else
        {
            playPanelImage.transform.localEulerAngles = Vector3.zero;
        }
    }

    public void Play()
    {
        if (GameSettings.nick.Length > 0)
        {
            loading = true;

            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 30;
            PhotonNetwork.GameVersion = "LaForest: Development";
            PhotonNetwork.ConnectUsingSettings();

            if (GameSettings.nick != null)
                PhotonNetwork.LocalPlayer.NickName = GameSettings.nick;
        }
        else
        {
            playPanelText.text = "Error! Go to game settings and enter your nick";
        }
    }

    /// <summary>
    /// Disconnects all services and if onlyDisconnect is equal false, then we also open main menu (resets game)
    /// </summary>
    /// <param name="onlyDisconnect"></param>
    public void GameDisconnect(bool onlyDisconnect = false)
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PlayerInfo.players = new List<PlayerInfo>();
        loading = false;
        if (!onlyDisconnect)
            UsefulReferences.localGameControlObject.GetComponent<MainMenu>().SetUpGame(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnCreatedRoom()
    {
        photonView.RPC("PlayerConnected", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlayerConnected", RpcTarget.All, newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlayerDisconnected", RpcTarget.All, newPlayer);
        }
    }

    /// <summary>
    /// Receive whole information about another players and let them appear in out local game (executes only on the player, who joined and is called by the master client)
    /// </summary>
    /// <param name="serializedPlayers"></param>
    [PunRPC]
    void ReceiveAnotherPlayers(string[] serializedPlayers)
    {
        List<string> curWeaponSerialized = new List<string>(); //the weapon, which other player (the i. element's of this list) is currently holding
        List<int> playerPvID = new List<int>(); //other player's (the i. element's of this list) photon view id
        List<Player> playerPP = new List<Player>(); //other player's (the i. element's of this list) photon player, which is actually the owner of the photon view with playerPvID[i]

        //let's deserialize all data
        for (int i = 0; i < serializedPlayers.Length; i++)
        {
            Dictionary<string, string> deserializedPlayer;
            deserializedPlayer = PlayerInfo.DeserializePlayer(serializedPlayers[i]); //create a dictionary to easily put the information out of it later
            curWeaponSerialized.Add(deserializedPlayer["weapon"]);
            playerPvID.Add(int.Parse(deserializedPlayer["playerPvID"]));
            playerPP.Add(PhotonView.Find(playerPvID[i]).Owner);
        }

        //some of the below methods need full info about all another players', so we couldn't pack all methods in one "for" loop

        for(int i = 0; i < serializedPlayers.Length; i++)
        {
            //add another player to the players list in PlayerInfo
            PlayerConnected(playerPP[i]);
        }

        for (int i = 0; i < serializedPlayers.Length; i++)
        {
            SetSpawnedPlayer(playerPvID[i], playerPP[i]);
        }

        for (int i = 0; i < serializedPlayers.Length; i++)
        {
            //set last held weapon by another player
            PhotonView.Find(playerPvID[i]).gameObject.GetComponent<PlayerWeapons>().SetUpWeapon(curWeaponSerialized[i]);
        }
    }

    /// <summary>
    /// Use to set the spawned player (set his nickname above his head, lower his footsteps' volume etc.)
    /// </summary>
    /// <param name="pvID"></param>
    /// <param name="newPP"></param>
    [PunRPC]
    void SetSpawnedPlayer(int pvID, Player newPP)
    {
        GameObject newPlayer = PhotonView.Find(pvID).gameObject; //find the spawned player gameObject
        newPlayer.name = newPP.NickName; //set other player's name to his nickname
        newPlayer.transform.position = ((GameObject)Resources.Load("Player")).gameObject.transform.position;
        newPlayer.transform.Find("TextMeshPro Nick").gameObject.GetComponent<TextMeshPro>().text = newPlayer.name; //let the 3d text above head show his nickname
        PlayerInfo.FindPlayerInfoByPP(newPP).gameObject = newPlayer; //set PlayerInfo gameObject reference to newPlayer's instance
        if (newPP == PhotonNetwork.LocalPlayer) //if we are new on this server
        {
            UsefulReferences.Initialize(newPlayer); //initialize the game
            UsefulReferences.localGameControlObject.GetComponent<MainMenu>().SetUpGame(false); //and disable everything, what could disturb the player while playing
        }
        else
        {
            newPlayer.GetComponent<AudioSource>().volume = 0.5f; //lower (the footsteps', shooting) sounds volume to let the player recognise when he hears his sounds and other players' sounds
        }
    }

    /// <summary>
    /// Spawn the player (executes only on the player, who joined)
    /// </summary>
    public void SpawnPlayer()
    {
        //check if we are on the main scene (3/28/2019 useless due to fact that there is actually just one scene)
        if (SceneManager.GetActiveScene().name == "Main")
        {
            GameObject player; //we instantiate or use the player gameObject waiting for us the scene (only tests in editor)
            if (Tests.tests)
                player = GameObject.Find("Player");
            else
            {
                player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
            }

            //enable our scripts (for example PlayerMovement etc., they are disabled by default)
            Behaviour[] scripts = player.GetComponents<Behaviour>();
            Behaviour[] scriptsChildren = player.GetComponentsInChildren<Behaviour>();
            foreach (Behaviour script in scripts)
            {
                script.enabled = true;
            }
            foreach (Behaviour script in scriptsChildren)
            {
                script.enabled = true;
            }
            GetComponent<PhotonView>().RPC("SetSpawnedPlayer", RpcTarget.All, player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer); 
        }
    }

    /// <summary>
    /// Calls when a player joins or we are the player, who joined and we want to let another players appear in our local game
    /// </summary>
    /// <param name="newPP"></param>
    [PunRPC]
    void PlayerConnected(Player newPP)
    {
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.nick = newPP.NickName;
        playerInfo.pp = newPP;
        PlayerInfo.players.Add(playerInfo);

        if (newPP == PhotonNetwork.LocalPlayer)
        {
            PlayerInfo.myPlayerInfo = playerInfo;

            //checks whether will we have any nick duplicates
            bool CheckNickDuplicates()
            {
                foreach (var item in PhotonNetwork.PlayerListOthers)
                {
                    if (item.NickName == PhotonNetwork.NickName)
                    {
                        return false;
                    }
                }
                return true;
            }

            if (!CheckNickDuplicates())
            {
                GameDisconnect(true);
                playPanelText.text = "Error! Your nick is already in use";
                return;
            }

            SpawnPlayer();
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            //the master client collects all player information needed to play skipped rpc's, serializes them and sends info to the player, who has just joined
            List<string> serializedPlayers = new List<string>();
            foreach (var player in PlayerInfo.players)
            {
                if(player.pp != newPP) //do not include the new player's info in message to him
                    serializedPlayers.Add(player.SerializePlayer()); //add next player from the server
            }

            //finally send information of all players to new player
            photonView.RPC("ReceiveAnotherPlayers", newPP, (object) serializedPlayers.ToArray());
        }
    }

    
    /// <summary>
    /// Calls when a player leaves the game (executes only on the players, who currently are on the server)
    /// </summary>
    /// <param name="pp"></param>
    [PunRPC]
    void PlayerDisconnected(Player pp)
    {
        PlayerInfo player = PlayerInfo.FindPlayerInfoByPP(pp);
        PlayerInfo.players.Remove(player);
    }
}
