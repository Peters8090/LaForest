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

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && PhotonNetwork.NetworkClientState.ToString() == "Joined")
        {
            PlayerInfo.DebugPlayersList();
        }

        if(playPanelText)
            playPanelText.text = PhotonNetwork.NetworkClientState.ToString();
        if (playPanelImage)
            playPanelImage.transform.Rotate(Vector3.back * 10);
    }

    public void Play(Text playPanelText)
    {
        if (GameSettings.nick.Length > 0)
        {
            this.playPanelText = playPanelText;
            playPanelImage = playPanelText.transform.parent.Find("SpiderImage").gameObject;

            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 30;
            PhotonNetwork.GameVersion = "LaForest: Development";
            PhotonNetwork.ConnectUsingSettings();

            //checks whether will we have any nick duplicates
            bool CheckNickDuplicates()
            {
                bool correct = true;
                foreach (var item in PhotonNetwork.PlayerList)
                {
                    if(item.NickName == GameSettings.nick)
                    {
                        correct = false;
                    }
                }
                return correct;
            }

            foreach (var item in PhotonNetwork.PlayerList)
            {
                Debug.Log(item.NickName);
            }

            //todo: end it
            if (!CheckNickDuplicates())
            {
                GameDisconnect();
                playPanelText.text = "Error! Your nick is already in use";
                return;
            } else
            {
                Debug.Log("a");
            }

            if(GameSettings.nick != null)
                PhotonNetwork.LocalPlayer.NickName = GameSettings.nick;
        } else
        {
            playPanelText.text = "Error! Go to game settings and enter your nick";
        }
    }

    public void GameDisconnect()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
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
    /*
    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 200, 20), PhotonNetwork.NetworkClientState.ToString());
    }*/
}
