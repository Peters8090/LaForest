using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    public void GameDisconnect(bool onlyDisconnect = false)
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        loading = false;
        if (!onlyDisconnect)
            UsefulReferences.localGameControlObject.GetComponent<MainMenu>().SetGame(true);
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
            //photonView.RPC("PlayerConnected", RpcTarget.AllBuffered, newPlayer);
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

    [PunRPC]
    public void PlayerConnected(Player pp)
    {
        //while playing, some player joins the game
        PlayerInfo player = new PlayerInfo();
        player.nick = pp.NickName;
        player.pp = pp;
        PlayerInfo.players.Add(player);
        //we are the player, who joined
        if (pp == PhotonNetwork.LocalPlayer)
        {
            PlayerInfo.myPlayerInfo = player;

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
        else
            photonView.RPC("SetAnotherPlayer", pp, PlayerInfo.myPlayerInfo.SerializePlayer
                (), PhotonNetwork.LocalPlayer);
    }


    [PunRPC]
    void PlayerConnected3(Player pp)
    {
        if (pp != null)
        {
            PlayerInfo player = new PlayerInfo();
            player.nick = pp.NickName;
            player.pp = pp;
            PlayerInfo.players.Add(player);
            if (pp == PhotonNetwork.LocalPlayer)
            {
                PlayerInfo.myPlayerInfo = player;

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
        }
    }

    [PunRPC]
    void PlayerConnected2((string, Player) serializedPlayer)
    {
        Player pp = serializedPlayer.Item2;
        Weapon weapon = Weapon.Deserialize(serializedPlayer.Item1);

        PlayerInfo player = new PlayerInfo();
        player.nick = pp.NickName;
        player.pp = pp;
        PlayerInfo.players.Add(player);
        if (pp == PhotonNetwork.LocalPlayer)
        {
            PlayerInfo.myPlayerInfo = player;

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
    }

    [PunRPC]
    void PlayerDisconnected(Player pp)
    {
        PlayerInfo player = PlayerInfo.FindPlayerInfoByPP(pp);
        if (player != null)
        {
            PlayerInfo.players.Remove(player);
        }
    }

    public void SpawnPlayer()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            GameObject player;
            if (Tests.tests)
                player = GameObject.Find("Player");
            else
            {
                player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
            }

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
            GetComponent<PhotonView>().RPC("SetSpawnedPlayer", RpcTarget.AllBuffered, PlayerInfo.myPlayerInfo.nick, player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer, false, null);
        }
    }

    [PunRPC]
    void SetAnotherPlayer(string serializedPlayer, Player pp)
    {
        string[] deserializedPlayer = serializedPlayer.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.LogError(serializedPlayer);
        Weapon weapon = Weapon.Deserialize(deserializedPlayer[0]);
        int anotherPlayerPvID = int.Parse(deserializedPlayer[1]);

        PlayerInfo player = new PlayerInfo();
        player.nick = pp.NickName;
        player.pp = pp;
        PlayerInfo.players.Add(player);
        SetSpawnedPlayer(pp.NickName, anotherPlayerPvID, pp, false, weapon);
    }

    [PunRPC]
    void SetSpawnedPlayer(string nick, int pvID, Player newPlayersPP, bool justJoined, Weapon accWeapon)
    {
        GameObject newPlayer = PhotonView.Find(pvID).gameObject;
        newPlayer.name = nick;
        newPlayer.transform.position = ((GameObject)Resources.Load("Player")).gameObject.transform.position;
        newPlayer.transform.Find("TextMeshPro Nick").gameObject.GetComponent<TextMeshPro>().text = nick;
        
        if (newPlayersPP == PhotonNetwork.LocalPlayer)
        {
            UsefulReferences.Initialize(newPlayer);
            UsefulReferences.localGameControlObject.GetComponent<MainMenu>().SetGame(false);
        }
        else
        {
            newPlayer.GetComponent<AudioSource>().volume = 0.5f;
        }

        if (justJoined)
        {
            newPlayer.GetComponent<PlayerWeapons>().SetWeapon(accWeapon.weaponObjPvID, accWeapon.Serialize(), newPlayersPP);
        }
    }
}
