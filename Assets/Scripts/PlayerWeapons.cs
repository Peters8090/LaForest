using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour
{
    public int weaponIndex;
    public List<Weapon> weapons;
    GameObject eq;
    public bool disarmed = false;
    public bool canAttack = true;

    Sprite axeImg;
    Texture2D axeImgTexture;
    Sprite flashlightImg;
    Texture2D flashlightImgTexture;
    Sprite swordImg;
    Texture2D swordImgTexture;

    Weapon axeTemplate;
    Weapon flashlightTemplate;
    Weapon swordTemplate;

    Image activeWeaponImg;

    void Start()
    {
        axeTemplate = new Weapon("Axe");
        flashlightTemplate = new Weapon("Flashlight");
        swordTemplate = new Weapon("Sword");

        weapons = new List<Weapon>() { axeTemplate, flashlightTemplate, swordTemplate };
        eq = UsefulReferences.eq;
        activeWeaponImg = UsefulReferences.ui.transform.Find("ActiveWeapon").gameObject.GetComponent<Image>();
        activeWeaponImg.gameObject.SetActive(true);
    }
    
    void Update()
    {
        if(!disarmed)
            GetNumberKeys();
        if (eq.transform.GetChild(0).name != weapons[weaponIndex].name && !disarmed)
        {
            Destroy(eq.transform.GetChild(0).gameObject);
            GameObject go = Instantiate((GameObject) Resources.Load(weapons[weaponIndex].name));
            go.transform.parent = eq.transform;
            go.name = weapons[weaponIndex].name;
            go.transform.localPosition = ((GameObject)Resources.Load(weapons[weaponIndex].name)).transform.position;
            go.transform.localRotation = ((GameObject)Resources.Load(weapons[weaponIndex].name)).transform.rotation;
            go.transform.localScale = ((GameObject)Resources.Load(weapons[weaponIndex].name)).transform.localScale;
        } else if(disarmed && eq.transform.childCount == 1)
        {
            Destroy(eq.transform.GetChild(0).gameObject);
            GameObject go = Instantiate((GameObject) Resources.Load("EmptyGameObject"));
            go.transform.parent = UsefulReferences.eq.transform;
        }

        activeWeaponImg.sprite = weapons[weaponIndex].image;

        if(Input.GetButtonDown("Fire1") && canAttack)
        {
            UsefulReferences.playerAnimator.Play("Attack");
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
