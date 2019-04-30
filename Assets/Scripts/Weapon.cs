using UnityEngine;
using System.Collections.Generic;

public class Weapon
{
    /// <summary>
    /// Enum with weapon types
    /// </summary>
    public enum WeaponType
    {
        Axe, Flashlight, Sword, Rock
    }

    #region WeaponTools
    

    /// <summary>
    /// Key is the hit bone name, value is the multiplier of the damage (this Dictionary can be used to recognize sensitive places of player's body f.e. head
    /// </summary>
    public static Dictionary<string, float> hitboxDamageMultiplier = new Dictionary<string, float>() { { "mixamorig:Head", 4f } };
    
    /// <summary>
    /// Array of weapons, which don't play any animation
    /// </summary>
    public static WeaponType[] nonAnimWeapons = new WeaponType[1] { WeaponType.Rock };


    #endregion

    #region WeaponParameters


    /// <summary>
    /// Returns actual damage (including weapon upgrades) of any game weapon
    /// </summary>
    /// <returns></returns>
    public static Dictionary<WeaponType, float> weaponDamages()
    {
        Dictionary<WeaponType, float> damages = new Dictionary<WeaponType, float>() { { WeaponType.Axe, 20f }, { WeaponType.Flashlight, 5f }, { WeaponType.Rock, 12.5f }, { WeaponType.Sword, 40f } };
        //here changes like weapon upgrades
        return damages;
    }
    

    #endregion
    
    /// <summary>
    /// Name of the weapon
    /// </summary>
    public string name;

    /// <summary>
    /// Type of the weapon
    /// </summary>
    public WeaponType weaponType;

    /// <summary>
    /// PhotonView ID of component attached to the gameObject of weapon
    /// </summary>
    public int weaponObjPvID;
    
    public string Serialize()
    {
        return ((int)weaponType).ToString() + "," + weaponObjPvID.ToString();
    }

    public static Weapon Deserialize(string serializedWeapon)
    {
        string[] weaponParams = serializedWeapon.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        return new Weapon((WeaponType)int.Parse(weaponParams[0]), int.Parse(weaponParams[1]));
    }

    public Weapon(WeaponType weaponType, int weaponObjPvID)
    {
        this.name = weaponType.ToString();
        this.weaponType = weaponType;
        this.weaponObjPvID = weaponObjPvID;
    }
}
