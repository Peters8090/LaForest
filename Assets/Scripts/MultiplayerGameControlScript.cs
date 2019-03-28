using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

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
        photonView.RPC("PlayerConnected", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlayerConnected", RpcTarget.AllBuffered, newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlayerDisconnected", RpcTarget.AllBuffered, newPlayer);
        }
    }

    [PunRPC]
    void PlayerConnected(Player pp)
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
            GetComponent<PhotonView>().RPC("SetSpawnedPlayer", RpcTarget.AllBuffered, PlayerInfo.myPlayerInfo.nick, player.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void SetSpawnedPlayer(string nick, int pvID, PhotonMessageInfo pmi)
    {
        if(pmi.Sender != null && PhotonNetwork.GetPhotonView(pvID) != null && PlayerInfo.FindPlayerInfoByPP(pmi.Sender) != null)
        {
            GameObject newPlayer = PhotonView.Find(pvID).gameObject;
            newPlayer.name = nick;
            newPlayer.transform.position = ((GameObject)Resources.Load("Player")).gameObject.transform.position;
            newPlayer.transform.Find("TextMeshPro Nick").gameObject.GetComponent<TextMeshPro>().text = nick;
            PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject = newPlayer;
            if (pmi.Sender == PhotonNetwork.LocalPlayer)
            {
                UsefulReferences.Initialize(newPlayer);
                UsefulReferences.localGameControlObject.GetComponent<MainMenu>().SetGame(false);
            } else
            {
                newPlayer.GetComponent<AudioSource>().volume = 0.5f;
            }
        }
    }
}
