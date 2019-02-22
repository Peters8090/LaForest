using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RockBullet : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 targetPos;
    Quaternion targetRot;

    void Start()
    {

    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //only attacker can do things below
        if (photonView.IsMine)
        {
            //attacker shots a player
            if (collision.gameObject.tag == "Player")
            {
                GameObject shotPlayer;
                //attacker can shot the player model, his weapon or something else, which is not the parent of parents; the player, so we get it using FindTopParent method
                shotPlayer = UsefulMethods.FindTopParent(collision.gameObject);
                //we subtract damage from shot player's health
                shotPlayer.GetPhotonView().RPC("TakeDamage", PlayerInfo.FindPPByGO(shotPlayer), Weapon.rock.damage);
            }
            else
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            targetPos = (Vector3)stream.ReceiveNext();
            targetRot = (Quaternion)stream.ReceiveNext();
        }
    }


    #endregion
}
