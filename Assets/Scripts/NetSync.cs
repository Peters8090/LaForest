using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetSync : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 targetPos;
    Quaternion targetRot;

    void Start()
    {

    }

    void Update()
    {
        //the Player, who created the gameObject is its owner
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }
    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(UsefulReferences.playerAnimator.GetFloat("VelX"));
            stream.SendNext(UsefulReferences.playerAnimator.GetFloat("VelY"));
            stream.SendNext(UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
            stream.SendNext(UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
            stream.SendNext(UsefulReferences.playerWeapons.weapons[UsefulReferences.playerWeapons.weaponIndex].name);
            //stream.SendNext(UsefulReferences.playerDeath.died);
        }
        else
        {
            Animator myAnimator;
            GameObject ybot;
            GameObject player = gameObject;
            GameObject ragdollModel = (GameObject)Resources.Load("ybot ragdoll");

            myAnimator = transform.Find("ybot").GetComponent<Animator>();
            ybot = transform.Find("ybot").gameObject;

            targetPos = (Vector3)stream.ReceiveNext();
            targetRot = (Quaternion)stream.ReceiveNext();
            myAnimator.SetFloat("VelX", (float)stream.ReceiveNext());
            myAnimator.SetFloat("VelY", (float)stream.ReceiveNext());
            if ((bool)stream.ReceiveNext() && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) myAnimator.Play("Jump");
            if ((bool)stream.ReceiveNext() && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) myAnimator.Play("Attack");
            GetComponent<WeaponsSync>().activeWeapon = new Weapon((string)stream.ReceiveNext());
            /*if ((bool)stream.ReceiveNext()) { ybot.gameObject.SetActive(false); if (player.transform.Find("ybot ragdoll") == null)
                {
                    GameObject ybotRagdoll = PhotonNetwork.Instantiate("ybot ragdoll", Vector3.zero, Quaternion.identity);
                    ybotRagdoll.name = "ybot ragdoll";
                    ybotRagdoll.transform.parent = player.transform;
                    ybotRagdoll.transform.localPosition = ragdollModel.transform.localPosition;
                    ybotRagdoll.transform.localRotation = ragdollModel.transform.localRotation;
                }
            } else { ybot.gameObject.SetActive(true); if (player.transform.Find("ybot ragdoll") != null)
                {
                    Destroy(player.transform.Find("ybot ragdoll").gameObject);
                }
            }*/
        }
    }


    #endregion
}
