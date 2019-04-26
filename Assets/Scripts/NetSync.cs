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
            if (Vector3.Distance(transform.position, targetPos) > 3)
                transform.position = targetPos;
            else
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
            stream.SendNext(UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Movement"));
            stream.SendNext(UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
            stream.SendNext(UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
            stream.SendNext(UsefulReferences.playerDeath.died);
        }
        else
        {
            Animator myAnimator;
            GameObject ybot;
            GameObject player = gameObject;

            myAnimator = transform.Find("ybot").GetComponent<Animator>();
            ybot = transform.Find("ybot").gameObject;
            targetPos = (Vector3)stream.ReceiveNext();
            targetRot = (Quaternion)stream.ReceiveNext();
            myAnimator.SetFloat("VelX", (float)stream.ReceiveNext());
            myAnimator.SetFloat("VelY", (float)stream.ReceiveNext());
            if ((bool)stream.ReceiveNext() && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Movement")) myAnimator.Play("Movement");
            if ((bool)stream.ReceiveNext() && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) myAnimator.Play("Jump");
            if ((bool)stream.ReceiveNext() && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) myAnimator.Play("Attack");
            GetComponent<PlayerRagdoll>().died = (bool)stream.ReceiveNext();
        }
    }


    #endregion
}
