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
    bool isDangerous = true;

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
        Debug.Log(collision.gameObject);
        OnCollision(collision.gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject);
        OnCollision(collision.gameObject);
    }

    void OnCollision(GameObject collision)
    {
        //we cannot recognise CharacterController collisions
        //if(collision.tag != "Player")
        {
            //attacker can shot the player model, his weapon or something else, which is not the parent of parents; the player, so we get it using FindTopParent method
            GameObject collisionGO;
            collisionGO = UsefulMethods.FindTopParent(collision);
            //only attacker can do things below
            if (GetComponent<PhotonView>() && collisionGO.GetComponent<PhotonView>() && isDangerous)
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

        //make our bullet not attack anyone anymore
        isDangerous = false;
        //destroy gameObject after it falls down (after 1 second)
        timer = maxTimer - 1;
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
