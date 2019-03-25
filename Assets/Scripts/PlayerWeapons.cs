using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerWeapons : MonoBehaviourPunCallbacks
{
    public int weaponIndex;
    public List<Weapon.WeaponType> weapons;
    public bool disarmed = false;
    public bool canAttack = true;
    public bool canChangeWeapons = true;

    //returns the local player's WeaponMB script, which is attached to actual weapon
    public static WeaponMB accWeaponMB;

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
        UsefulReferences.activeWeaponImg.gameObject.SetActive(true);
    }

    void Update()
    {
        if(canChangeWeapons)
            GetNumberKeys();

        //if current weapon doesn't play any animation, nonAnimWeapon is equal to true, otherwise false
        nonAnimWeapon = nonAnimWeapons.Contains(weapons[weaponIndex]);

        if(disarmed)
        {
            //eq always has to have one child, unless the PlayerWeapons script isn't running
            if (UsefulReferences.eq.transform.childCount == 1)
                Disarmed();
        } else
        {
            //we check whether the change of weapon is required (returns true if we haven't changed)
            if (UsefulReferences.eq.transform.GetChild(0).name != weapons[weaponIndex].ToString())
            {
                if (UsefulReferences.eq.transform.GetChild(0).gameObject.GetComponent<PhotonView>())
                    PhotonNetwork.Destroy(UsefulReferences.eq.transform.GetChild(0).gameObject);

                GameObject go = PhotonNetwork.Instantiate(weapons[weaponIndex].ToString(), Vector3.zero, Quaternion.identity);

                photonView.RPC("SetWeapon", RpcTarget.All, go.GetPhotonView().ViewID, new Weapon(weapons[weaponIndex], Weapon.weaponDamages[weapons[weaponIndex]], go.GetComponent<PhotonView>().ViewID).Serialize(), PhotonNetwork.LocalPlayer);
            }
        }
        
        void Disarmed()
        {
            if(UsefulReferences.eq.transform.GetChild(0).gameObject.GetComponent<PhotonView>())
                PhotonNetwork.Destroy(UsefulReferences.eq.transform.GetChild(0).gameObject);
            else
                PhotonNetwork.Destroy(UsefulReferences.eq.transform.GetChild(0).gameObject);

            //we can do it locally because the PlayerWeapons script isn't running on other players in our local game
            GameObject go = Instantiate((GameObject)Resources.Load("EmptyGameObject"));
            go.transform.parent = UsefulReferences.eq.transform;
        }

        //if there is EmptyGameObject, we aren't holding any weapon
        if (UsefulReferences.eq.transform.GetChild(0).name == "EmptyGameObject")
            accWeaponMB = null;
        else
            accWeaponMB = UsefulReferences.eq.transform.GetChild(0).GetComponent<WeaponMB>();

        //TODO: make accWeaponMB non-nullable
        UsefulReferences.activeWeaponImg.texture = (Texture2D) Resources.Load( weapons[weaponIndex].ToString() + "Img");
    }
    
    [PunRPC]
    public void SetWeapon(int weaponViewID, string serializedWeapon, Player weaponOwner)
    {
        if (PhotonNetwork.GetPhotonView(weaponViewID).Owner == weaponOwner)
        {
            PhotonNetwork.GetPhotonView(weaponViewID).GetComponent<WeaponMB>().weapon = Weapon.Deserialize(serializedWeapon);
            PhotonNetwork.GetPhotonView(weaponViewID).GetComponent<WeaponMB>().SetMyWeapon();
        }
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
