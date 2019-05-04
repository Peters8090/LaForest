﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sword : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Range of the sword
    /// </summary>
    float range = 1f;

    /// <summary>
    /// When this var is true, we are still scanning for objects which possibly can hit this sword (so we haven't found any object with any tag from the switch loop); if it is false we found the object (sword has hit it); this var is to prevent further sending info about hits after the sword has hit the object
    /// </summary>
    bool scanning = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && UsefulReferences.playerWeapons.canAttack && !UsefulReferences.playerWeapons.isAttacking)
        {
            UsefulReferences.playerAnimator.Play("Attack");
            Invoke("StartScanning", 0.83f);
            //invoke the method in 0.83 seconds, which is the time after which sword becomes dangerous (swing is before 0.83 seconds)
        }
    }

    void StartScanning()
    {
        if (UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            scanning = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root == UsefulReferences.player) //if the weapon accidentally touched its owner's body, return
            return;

        if (scanning)
        {
            if (UsefulReferences.playerWeapons.isAttacking)
            {
                GameObject hitGO = other.gameObject;
                GameObject hitGORoot = other.transform.root.gameObject;

                switch (hitGORoot.tag)
                {
                    case "Player":
                        {
                            if (hitGORoot != UsefulReferences.player && hitGO.GetComponent<WeaponMB>() == null)
                            {
                                float weaponDamage = Weapon.weaponDamages()[Weapon.WeaponType.Sword];

                                //if the rock hit a sensitive place, f.e. head, the damage will be increased
                                if (Weapon.hitboxDamageMultiplier.ContainsKey(hitGO.name))
                                {
                                    weaponDamage *= Weapon.hitboxDamageMultiplier[hitGO.name];
                                }
                                //we subtract damage from shot player's health
                                hitGORoot.GetPhotonView().RPC("TakeDamage", PlayerInfo.FindPPByGO(hitGORoot), weaponDamage, UsefulReferences.player.transform.forward, other.name);
                            }
                            scanning = false;
                            break;
                        }
                }
            }
        }
    }
}
