using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerDeath : MonoBehaviourPunCallbacks
{
    float timer = 0;
    float timerMax = 1f;
    public bool died = false;
    Slider slider;

    void Start()
    {
        slider = UsefulReferences.deathUI.transform.Find("Slider").GetComponent<Slider>();
    }

    void Update()
    {
        if (died)
        {
            if (timer < timerMax)
            {
                timer += Time.deltaTime;
                slider.value = timer / timerMax;
            }
            else
            {
                UsefulReferences.playerHealth.Regenerate();
                DisableRagdoll();
                timer = 0;
                died = false;
            }
        }
    }

    /*
                GameObject ragdollModel = (GameObject)Resources.Load("ybot ragdoll");
                GameObject ybotRagdoll = Instantiate(ragdollModel, Vector3.zero, Quaternion.identity);
                ybotRagdoll.name = "ybot ragdoll";
                ybotRagdoll.transform.parent = UsefulReferences.player.transform;
                ybotRagdoll.transform.localPosition = ragdollModel.transform.localPosition;
                ybotRagdoll.transform.localRotation = ragdollModel.transform.localRotation;
                foreach (Behaviour script in ybotRagdoll.GetComponentsInChildren<Camera>())
                {
                    script.enabled = true;
                }*/

    void EnableRagdoll()
    {
        GameObject ybotRagdoll;
        if (UsefulReferences.player.transform.Find("ybot ragdoll") == null)
        {
            UsefulReferences.mainCamera.SetActive(false);
            UsefulReferences.ybot.SetActive(false);
            ybotRagdoll = PhotonNetwork.Instantiate("ybot ragdoll", Vector3.zero, Quaternion.identity);
            photonView.RPC("EnableRagdollRPC", RpcTarget.AllBuffered, ybotRagdoll.GetPhotonView().ViewID);
        }
    }

    void DisableRagdoll()
    {
        UsefulReferences.mainCamera.SetActive(true);
        UsefulReferences.ybot.SetActive(true);
        if (UsefulReferences.player.transform.Find("ybot ragdoll") != null)
        {
            PhotonNetwork.Destroy(UsefulReferences.player.transform.Find("ybot ragdoll").gameObject);
            photonView.RPC("DisableRagdollRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void EnableRagdollRPC(int pvID, PhotonMessageInfo pmi)
    {
        //because PhotonView.Find(pvID) != null caused NullReferenceException
        if(PhotonNetwork.GetPhotonView(pvID) != null && pmi.Sender != null && PlayerInfo.FindPlayerInfoByPP(pmi.Sender) != null)
        {
            GameObject ybotRagdoll = PhotonView.Find(pvID).gameObject;
            GameObject ragdollModel = (GameObject)Resources.Load("ybot ragdoll");

            PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject.transform.Find("ybot").gameObject.SetActive(false);
            ybotRagdoll.name = "ybot ragdoll";
            ybotRagdoll.transform.parent = PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject.transform;
            ybotRagdoll.transform.localPosition = ragdollModel.transform.localPosition;
            ybotRagdoll.transform.localRotation = ragdollModel.transform.localRotation;

            if (pmi.Sender == PhotonNetwork.LocalPlayer)
            {
                foreach (Behaviour script in ybotRagdoll.GetComponentsInChildren<Camera>())
                {
                    script.enabled = true;
                }
            }
        }
    }

    [PunRPC]
    void DisableRagdollRPC(PhotonMessageInfo pmi)
    {
        if(pmi.Sender != null && PlayerInfo.FindPlayerInfoByPP(pmi.Sender) != null)
            PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject.transform.Find("ybot").gameObject.SetActive(true);
    }

    public void Die()
    {
        died = true;
        
        EnableRagdoll();
    }
}
