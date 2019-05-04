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

    /// <summary>
    /// If rock has hit anything, let's make it safe for everyone
    /// </summary>
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
        OnCollision(collision.gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        OnCollision(collision.gameObject);
    }

    void OnCollision(GameObject hitGO)
    {
        //attacker can shot the player model, his weapon or something else, so we need transform.root
        GameObject hitGORoot;
        hitGORoot = hitGO.transform.root.gameObject;
        //only attacker can do things below
        if (GetComponent<PhotonView>() && hitGORoot.GetComponent<PhotonView>() && isDangerous)
        {
            if (photonView.IsMine)
            {
                switch(hitGORoot.tag)
                {
                    case "Player":
                        if (hitGORoot != UsefulReferences.player && hitGO.GetComponent<WeaponMB>() == null)
                        {
                            float weaponDamage = Weapon.weaponDamages()[Weapon.WeaponType.Rock];

                            //if the rock hit a sensitive place, f.e. head, the damage will be increased
                            if (Weapon.hitboxDamageMultiplier.ContainsKey(hitGO.name))
                            {
                                weaponDamage *= Weapon.hitboxDamageMultiplier[hitGO.name];
                            }
                            //we subtract damage from shot player's health
                            hitGORoot.GetPhotonView().RPC("TakeDamage", PlayerInfo.FindPPByGO(hitGORoot), weaponDamage, UsefulReferences.player.transform.forward * Rock.speed * ((GameObject)Resources.Load("RockBullet")).GetComponent<Rigidbody>().mass * 1000f, hitGO.name);
                        }
                        break;
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
