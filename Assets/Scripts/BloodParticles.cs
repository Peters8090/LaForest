using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BloodParticles : MonoBehaviour
{
    [HideInInspector]
    public float timer = 0;
    [HideInInspector]
    public float maxTimer = 3f;
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= maxTimer && GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void SetUpMyself(string hitBoneName, PhotonMessageInfo pmi)
    {
        if (hitBoneName == "ybot" || hitBoneName == PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject.name)
            hitBoneName = "mixamorig:Spine";
        transform.parent = UsefulMethods.FindChild(PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject.transform, hitBoneName);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
}
