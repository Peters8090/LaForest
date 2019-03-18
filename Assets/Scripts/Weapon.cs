using UnityEngine;

public class Weapon
{

    public static Weapon axe = new Weapon(WeaponType.Axe);
    public static Weapon flashlight = new Weapon(WeaponType.Flashlight);
    public static Weapon sword = new Weapon(WeaponType.Sword);
    public static Weapon rock = new Weapon(WeaponType.Rock);

    public string name;
    public float damage;
    public Texture2D image;
    public WeaponType weaponType;

    public enum WeaponType
    {
        Axe, Flashlight, Sword, Rock
    }

    public string Serialize()
    {
        return ((int)weaponType).ToString() + "." + damage.ToString();
    }

    public static Weapon Deserialize(string serializedWeapon)
    {
        string[] weaponParams = serializedWeapon.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
        return new Weapon((WeaponType) int.Parse(weaponParams[0]), float.Parse(weaponParams[1]));
    }

    public Weapon(WeaponType weaponType, float ?damage = null)
    {
        this.weaponType = weaponType;
        switch (weaponType)
        {
            default:
                name = "";
                damage = 0;
                break;
            case WeaponType.Axe:
                name = "Axe";
                damage = 10f;
                break;
            case WeaponType.Flashlight:
                name = "Flashlight";
                damage = 5f;
                break;
            case WeaponType.Sword:
                name = "Sword";
                damage = 20f;
                break;
            case WeaponType.Rock:
                name = "Rock";
                damage = 10f;
                break;
        }

        //if damage is not default
        if (damage != null)
            this.damage = (float) damage;
        image = (Texture2D)Resources.Load(name + "Img");
    }
}
