using UnityEngine;
using System.Collections.Generic;

public class Weapon
{
    public static Dictionary<WeaponType, float> weaponDamages = new Dictionary<WeaponType, float>() { { WeaponType.Axe, 20f } , { WeaponType.Flashlight, 5f } , { WeaponType.Rock, 10f } , { WeaponType.Sword, 30f } };

    /*
    #region weapon templates

    /// <summary>
    /// Returns an axe template
    /// </summary>
    /// <returns></returns>
    public static Weapon axeTemplate()
    {
        return new Weapon(WeaponType.Axe, 10f);
    }

    /// <summary>
    /// Returns an flashlight template
    /// </summary>
    /// <returns></returns>
    public static Weapon flashlightTemplate()
    {
        return new Weapon(WeaponType.Flashlight, 5f);
    }

    /// <summary>
    /// Returns an sword template
    /// </summary>
    /// <returns></returns>
    public static Weapon swordTemplate()
    {
        return new Weapon(WeaponType.Sword, 20f);
    }

    /// <summary>
    /// Returns an rock template
    /// </summary>
    /// <returns></returns>
    public static Weapon rockTemplate()
    {
        return new Weapon(WeaponType.Rock, 10f);
    }

    #endregion
    */
    public string name;
    public float damage;
    public Texture2D image;
    public WeaponType weaponType;
    public int weaponObjPvID;

    public enum WeaponType
    {
        Axe, Flashlight, Sword, Rock
    }

    public string Serialize()
    {
        return ((int)weaponType).ToString() + "," + damage.ToString() + "," + weaponObjPvID.ToString();
    }

    public static Weapon Deserialize(string serializedWeapon)
    {
        string[] weaponParams = serializedWeapon.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        return new Weapon((WeaponType)int.Parse(weaponParams[0]), float.Parse(weaponParams[1]), int.Parse(weaponParams[2]));
    }

    public Weapon(WeaponType weaponType, float damage, int weaponObjPvID)
    {
        this.weaponType = weaponType;
        Debug.Log(weaponType.ToString());
        this.name = weaponType.ToString();
        this.damage = (float)damage;
        this.weaponObjPvID = weaponObjPvID;
        image = (Texture2D)Resources.Load(name + "Img");
    }
}
