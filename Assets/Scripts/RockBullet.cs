using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RockBullet : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 targetPos;
    Quaternion targetRot;
    float timer = 0;
    float maxTimer = 20;

    void Start()
    {

    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        } else
        {
            timer += Time.deltaTime;
            if(timer >= maxTimer)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision.gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        OnCollision(collision.gameObject);
    }

    void OnCollision(GameObject collision)
    {
        //attacker can shot the player model, his weapon or something else, which is not the parent of parents; the player, so we get it using FindTopParent method
        GameObject collisionGO;
        collisionGO = UsefulMethods.FindTopParent(collision);
        //only attacker can do things below
        if(GetComponent<PhotonView>() && collisionGO.GetComponent<PhotonView>())
        {
            if (photonView.IsMine)
            {
                if (collisionGO.tag == "Player" && collisionGO != UsefulReferences.player)
                {
                    //we subtract damage from shot player's health
                    collisionGO.GetPhotonView().RPC("TakeDamage", PlayerInfo.FindPPByGO(collisionGO), Weapon.rock.damage);
                }
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
