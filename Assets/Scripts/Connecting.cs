using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Connecting : MonoBehaviourPunCallbacks
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && PhotonNetwork.NetworkClientState.ToString() == "Joined")
        {
            PlayerInfo.DebugPlayersList();
        }
    }

    public void Play()
    {
        if (GameObject.Find("NickInputField").GetComponent<InputField>().text.Length > 0)
        {
            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 30;
            PhotonNetwork.GameVersion = "LaForest: Development";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = GameObject.Find("NickInputField").GetComponent<InputField>().text;
            Application.LoadLevel("Main");
        }
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
        PlayerInfo player = PlayerInfo.FindPlayer(pp);
        if (player != null)
        {
            PlayerInfo.players.Remove(player);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 200, 20), PhotonNetwork.NetworkClientState.ToString());
    }
}
