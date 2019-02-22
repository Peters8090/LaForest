using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerWeapons : MonoBehaviour
{
    public int weaponIndex;
    public List<Weapon> weapons;
    GameObject eq;
    public bool disarmed = false;
    public bool canAttack = true;
    public bool canChangeWeapons = true;

    //if weapon plays an animation, it can't be used while moving because it disturbs the moving animation
    public bool nonAnimWeapon = false;

    //array of weapons, which don't play any animation
    string[] nonAnimWeaponNames = new string[1] { "Rock" };
    
    Texture2D axeImgTexture;
    Texture2D flashlightImgTexture;
    Texture2D swordImgTexture;

    void Start()
    {
        weapons = new List<Weapon>() { Weapon.axe, Weapon.flashlight, Weapon.sword, Weapon.rock };
        eq = UsefulReferences.eq;
        UsefulReferences.activeWeaponImg.gameObject.SetActive(true);
    }
    
    void Update()
    {
        if(canChangeWeapons)
            GetNumberKeys();

        //if current weapon doesn't play any animation, nonAnimWeapon is equal to true, otherwise false
        nonAnimWeapon = nonAnimWeaponNames.Contains(weapons[weaponIndex].name);

        if(disarmed)
        {
            //eq always has to have one child
            if (eq.transform.childCount == 1)
                Disarmed();
        } else
        {
            //we check whether the change of weapon is required (returns true if we haven't changed)
            if (eq.transform.GetChild(0).name != weapons[weaponIndex].name)
            {
                Destroy(eq.transform.GetChild(0).gameObject);
                GameObject go = Instantiate((GameObject)Resources.Load(weapons[weaponIndex].name));
                go.transform.parent = eq.transform;
                go.name = weapons[weaponIndex].name;
                go.transform.localPosition = ((GameObject)Resources.Load(weapons[weaponIndex].name)).transform.position;
                go.transform.localRotation = ((GameObject)Resources.Load(weapons[weaponIndex].name)).transform.rotation;
                go.transform.localScale = ((GameObject)Resources.Load(weapons[weaponIndex].name)).transform.localScale;
            }
        }

        void Disarmed()
        {
            Destroy(eq.transform.GetChild(0).gameObject);
            GameObject go = Instantiate((GameObject)Resources.Load("EmptyGameObject"));
            go.transform.parent = UsefulReferences.eq.transform;
        }

        UsefulReferences.activeWeaponImg.texture = weapons[weaponIndex].image;
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
