using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerGameControlScript : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(550, 170), 0, Random.Range(1480, 1500)), Quaternion.identity, 0);
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
        GameObject newPlayer = PhotonView.Find(pvID).gameObject;
        newPlayer.name = nick;
        newPlayer.transform.position = new Vector3(917, 175, 325);
        //here set the text mesh text to player's nick
        if (pmi.Sender == PhotonNetwork.LocalPlayer)
        {
            UsefulReferences.Initialize(newPlayer);
            UsefulReferences.mainMenuCamera.GetComponent<Menu>().SetGame(false);
        }
    }
}
