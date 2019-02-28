using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Connecting : MonoBehaviourPunCallbacks
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

        if(loading)
        {
            playPanelText.text = PhotonNetwork.NetworkClientState.ToString();
            playPanelImage.transform.Rotate(Vector3.back * 10);
        } else
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
            
            if(GameSettings.nick != null)
                PhotonNetwork.LocalPlayer.NickName = GameSettings.nick;
        } else
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
        if(!onlyDisconnect)
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

                GetComponent<MultiplayerGameControlScript>().SpawnPlayer();
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
}
