using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Axe : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Range of the axe
    /// </summary>
    float range = 1f;

    /// <summary>
    /// When this var is true, we are still scanning for objects which possibly can hit this axe (so we haven't found any object with any tag from the switch loop); if it is false we found the object (axe has hit it); this var is to prevent further sending info about hits after the axe has hit the object
    /// </summary>
    bool scanning = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && UsefulReferences.playerWeapons.canAttack && !UsefulReferences.playerWeapons.isAttacking)
        {
            UsefulReferences.playerAnimator.Play("Attack");
            Invoke("StartScanning", 0.83f);
            //invoke the method in 0.83 seconds, which is time after which the axe becomes dangerous (swing is before 0.83 seconds)
        }

        //If player is attacking axe and (it hasn't hit anything, what could be damaged by axe (variable scanning is true)), then we use raycast to detect the objects hitting the axe
        if(scanning)
        {
            if (UsefulReferences.playerWeapons.isAttacking)
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out RaycastHit raycastHit, range))
                {
                    GameObject hitGO = raycastHit.collider.gameObject;
                    GameObject hitGORoot = raycastHit.collider.transform.root.gameObject;

                    switch (hitGORoot.tag)
                    {
                        case "Player":
                            {
                                if (hitGORoot != UsefulReferences.player)
                                {
                                    float weaponDamage = Weapon.weaponDamages()[Weapon.WeaponType.Axe];

                                    //if the rock hit a sensitive place, f.e. head, the damage will be increased
                                    if (Weapon.hitboxDamageMultiplier.ContainsKey(hitGO.name))
                                    {
                                        weaponDamage *= Weapon.hitboxDamageMultiplier[hitGO.name];
                                    }
                                    //we subtract damage from shot player's health
                                    hitGORoot.GetPhotonView().RPC("TakeDamage", PlayerInfo.FindPPByGO(hitGORoot), weaponDamage, UsefulReferences.player.transform.forward);
                                    hitGORoot.GetPhotonView().RPC("RestorePlayerModelPosAndRot", RpcTarget.All);
                                }
                                scanning = false;
                                break;
                            }
                    }
                }
            }
        }
    }

    void StartScanning()
    {
        if (UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            scanning = true;
        else
            return;
    }
}
