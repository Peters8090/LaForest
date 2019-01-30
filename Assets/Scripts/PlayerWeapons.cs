using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour
{
    int weaponIndex;
    List<Weapon> weapons;
    GameObject eq;
    public bool disarmed = false;

    Sprite axeImg;
    Texture2D axeImgTexture;
    Sprite flashlightImg;
    Texture2D flashlightImgTexture;
    Sprite swordImg;
    Texture2D swordImgTexture;

    Weapon axeTemplate;
    Weapon flashlightTemplate;
    Weapon swordTemplate;

    void Start()
    {
        axeImgTexture = (Texture2D) Resources.Load("axeImage");
        axeImg = Sprite.Create(axeImgTexture, new Rect(0f, 0f, axeImgTexture.width, axeImgTexture.height), new Vector2(0.5f, 0.5f), 100f);

        flashlightImgTexture = (Texture2D)Resources.Load("flashlightImage");
        flashlightImg = Sprite.Create(flashlightImgTexture, new Rect(0f, 0f, flashlightImgTexture.width, flashlightImgTexture.height), new Vector2(0.5f, 0.5f), 100f);

        swordImgTexture = (Texture2D)Resources.Load("swordImage");
        swordImg = Sprite.Create(swordImgTexture, new Rect(0f, 0f, swordImgTexture.width, swordImgTexture.height), new Vector2(0.5f, 0.5f), 100f);

        axeTemplate = new Weapon("Axe", axeImg);
        flashlightTemplate = new Weapon("Flashlight", flashlightImg);
        swordTemplate = new Weapon("Sword", swordImg);

        weapons = new List<Weapon>() { axeTemplate, flashlightTemplate, swordTemplate };
        eq = UsefulReferences.eq;
    }
    
    void Update()
    {
        GetNumberKeys();
        if(eq.transform.GetChild(0).name != weapons[weaponIndex].name && !disarmed)
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
        
        UsefulReferences.ui.transform.Find("ActiveWeapon").gameObject.GetComponent<Image>().sprite = weapons[weaponIndex].image;

        if(Input.GetButtonDown("Fire1"))
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
