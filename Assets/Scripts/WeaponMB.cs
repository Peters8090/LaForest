using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponMB : MonoBehaviourPunCallbacks
{
    public Weapon weapon;

    public void SetMyWeapon()
    {
        if (!photonView.IsMine)
        {
            switch (weapon.weaponType)
            {
                case Weapon.WeaponType.Axe:
                    GetComponent<Axe>().enabled = false;
                    break;
                case Weapon.WeaponType.Flashlight:
                    GetComponent<Flashlight>().enabled = false;
                    break;
                case Weapon.WeaponType.Rock:
                    GetComponent<Rock>().enabled = false;
                    break;
                case Weapon.WeaponType.Sword:
                    GetComponent<Sword>().enabled = false;
                    break;
            }
        }

        if (photonView.Owner != null && PlayerInfo.FindPlayerInfoByPP(photonView.Owner) != null)
        {
            transform.parent = PlayerInfo.FindPlayerInfoByPP(photonView.Owner).gameObject.GetComponent<UsefulReferencesPlayer>().eq.transform;
            gameObject.name = weapon.name;
            transform.localPosition = ((GameObject)Resources.Load(weapon.name)).transform.localPosition;
            transform.localRotation = ((GameObject)Resources.Load(weapon.name)).transform.localRotation;
            transform.localScale = ((GameObject)Resources.Load(weapon.name)).transform.localScale;
        }
    }
}
