using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Axe : MonoBehaviourPunCallbacks
{
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
            //invoke the method in 0.83 seconds, which is the time after which axe becomes dangerous (swing is before 0.83 seconds)
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

        if(scanning)
        {
            if(UsefulReferences.playerWeapons.isAttacking)
            {
                GameObject hitGO = other.gameObject;
                GameObject hitGORoot = other.transform.root.gameObject;

                switch (hitGORoot.tag)
                {
                    case "Player":
                        {
                            if (hitGORoot != UsefulReferences.player && hitGO.GetComponent<WeaponMB>() == null)
                            {
                                float axeDamage = Weapon.weaponDamages()[Weapon.WeaponType.Axe];

                                //if the rock hit a sensitive place, f.e. head, the damage will be increased
                                if (Weapon.hitboxDamageMultiplier.ContainsKey(hitGO.name))
                                {
                                    axeDamage *= Weapon.hitboxDamageMultiplier[hitGO.name];
                                }
                                //we subtract damage from shot player's health
                                hitGORoot.GetPhotonView().RPC("TakeDamage", PlayerInfo.FindPPByGO(hitGORoot), axeDamage, UsefulReferences.player.transform.forward, other.name);
                            }
                            scanning = false;
                            break;
                        }
                }
            }
        }
    }
}
