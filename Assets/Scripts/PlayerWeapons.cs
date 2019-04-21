using System.Collections;
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

    public WeaponMB accWeaponMB;

    //if weapon plays an animation, it can't be used while moving because it disturbs the moving animation
    public bool nonAnimWeapon = false;

    //array of weapons, which don't play any animation
    Weapon.WeaponType[] nonAnimWeapons = new Weapon.WeaponType[1] { Weapon.WeaponType.Rock };
    
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

        //if current weapon doesn't play any animation, nonAnimWeapon is equal to true, otherwise false
        nonAnimWeapon = nonAnimWeapons.Contains(weapons[weaponIndex]);

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
            //we set actual WeaponMB script reference to null, if there is one, we set accWeaponMB to its WeaponMB
            accWeaponMB = null;
            
            //to prevent NullReferenceException
            if (eq.transform.childCount > 0)
            {
                PhotonNetwork.Destroy(eq.transform.GetChild(0).gameObject.GetPhotonView());
            }

            //if player is unarmed, we can spawn new weapon
            if (!unarmed)
            {
                GameObject go = PhotonNetwork.Instantiate(weapons[weaponIndex].ToString(), Vector3.zero, Quaternion.identity);
                accWeaponMB = go.GetComponent<WeaponMB>();
                accWeaponMB.weapon = new Weapon(weapons[weaponIndex], Weapon.weaponDamages()[weapons[weaponIndex]], go.GetPhotonView().ViewID);
                photonView.RPC("SetWeapon", RpcTarget.All, accWeaponMB.weapon.Serialize());
            }
        }

        UsefulReferences.activeWeaponImg.texture = (Texture2D)Resources.Load(weapons[weaponIndex].ToString() + "Img");
    }

    [PunRPC]
    public void SetWeapon(string serializedWeapon)
    {
        if(serializedWeapon == "unarmed") //this player is unarmed (we set this in PlayerInfo, in SerializePlayer method)
        {
            return;
        }
        //get the viewID from the serializedWeapon (to get the spawned weapon)
        int viewID = int.Parse(serializedWeapon.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)[2]);
        PhotonNetwork.GetPhotonView(viewID).GetComponent<WeaponMB>().weapon = Weapon.Deserialize(serializedWeapon);
        PhotonNetwork.GetPhotonView(viewID).GetComponent<WeaponMB>().SetMyWeapon();
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
