﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class PlayerWeapons : MonoBehaviourPunCallbacks
{
    public int weaponIndex;
    public List<Weapon.WeaponType> weapons;
    GameObject eq;
    public bool unarmed = false;
    public bool canAttack = true;
    public bool canChangeWeapons = true;

    /// <summary>
    /// Can detect only non-anim weapons
    /// </summary>
    public bool isAttacking = false;

    public WeaponMB curWeaponMB;

    /// <summary>
    /// This variable is true when player holds a weapon which doesn't play any animation, otherwise it is equal to false; we can use it to detect whether current weapon can disturb the animation we want to play or is playing
    /// </summary>
    public bool nonAnimWeapon = false;
    
    Texture2D axeImgTexture;
    Texture2D flashlightImgTexture;
    Texture2D swordImgTexture;

    void Start()
    {
        weapons = new List<Weapon.WeaponType>() { Weapon.WeaponType.Axe, Weapon.WeaponType.Flashlight, Weapon.WeaponType.Rock, Weapon.WeaponType.Sword };
        eq = UsefulReferences.eq;
        UsefulReferences.activeWeaponImg.gameObject.SetActive(true);
    }

    void Update()
    {
        if (canChangeWeapons)
            GetNumberKeys();

        isAttacking = UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        //if current weapon doesn't play any animation, nonAnimWeapon is equal to true, otherwise false
        nonAnimWeapon = Weapon.nonAnimWeapons.Contains(weapons[weaponIndex]);

        if(unarmed)
        {
            //we check if we have destroyed the actual weapon
            if (eq.transform.childCount > 0)
                ChangeWeapon();
        } else
        {
            //to prevent NullReferenceException (transform.GetChild() causes it when there is no child and we try to get it)
            if(eq.transform.childCount > 0)
            {
                //we check whether the change of weapon is required (returns true if we haven't changed)
                if (eq.transform.GetChild(0).name != weapons[weaponIndex].ToString())
                {
                    ChangeWeapon();
                }
            } else
            {
                //if there is no weapon, we have to spawn one in ChangeWeapon() method
                ChangeWeapon();
            }
        }

        //this method can be used both for spawning new weapon and destroying actual weapon (making player unarmed)
        void ChangeWeapon()
        {
            //we set actual WeaponMB script reference to null, if there is one, we set curWeaponMB to its WeaponMB
            curWeaponMB = null;
            
            //to prevent NullReferenceException
            if (eq.transform.childCount > 0)
            {
                PhotonNetwork.Destroy(eq.transform.GetChild(0).gameObject.GetPhotonView());
            }

            //if player is unarmed, we can spawn new weapon
            if (!unarmed)
            {
                GameObject go = PhotonNetwork.Instantiate(weapons[weaponIndex].ToString(), Vector3.zero, Quaternion.identity);
                curWeaponMB = go.GetComponent<WeaponMB>();
                curWeaponMB.weapon = new Weapon(weapons[weaponIndex], go.GetPhotonView().ViewID);
                photonView.RPC("SetUpWeapon", RpcTarget.All, curWeaponMB.weapon.Serialize());
            }
        }

        UsefulReferences.activeWeaponImg.texture = (Texture2D)Resources.Load(weapons[weaponIndex].ToString() + "Img");
    }

    [PunRPC]
    public void SetUpWeapon(string serializedWeapon)
    {
        if(serializedWeapon == "unarmed") //this player is unarmed (we set this in PlayerInfo, in SerializePlayer method)
        {
            return;
        }
        //get the viewID from the serializedWeapon (to get the spawned weapon)
        int viewID = Weapon.Deserialize(serializedWeapon).weaponObjPvID;
        PhotonNetwork.GetPhotonView(viewID).GetComponent<WeaponMB>().weapon = Weapon.Deserialize(serializedWeapon);
        PhotonNetwork.GetPhotonView(viewID).GetComponent<WeaponMB>().SetUpMyWeapon();
    }
    
    void GetNumberKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (weapons.Count >= 1)
            {
                weaponIndex = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (weapons.Count >= 2)
            {
                weaponIndex = 1;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (weapons.Count >= 3)
            {
                weaponIndex = 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (weapons.Count >= 4)
            {
                weaponIndex = 3;
            }
        }

        if (Input.GetButtonDown("ChangeWeaponLeft"))
        {
            if (weaponIndex > 0)
            {
                weaponIndex--;
            }
            else
            {
                weaponIndex = weapons.Count - 1;
            }
        }

        if (Input.GetButtonDown("ChangeWeaponRight"))
        {
            if (weaponIndex < weapons.Count - 1)
            {
                weaponIndex++;
            }
            else
            {
                weaponIndex = 0;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (weaponIndex < weapons.Count - 1)
            {
                weaponIndex++;
            }
            else
            {
                weaponIndex = 0;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (weaponIndex > 0)
            {
                weaponIndex--;
            }
            else
            {
                weaponIndex = weapons.Count - 1;
            }
        }
    }
}
