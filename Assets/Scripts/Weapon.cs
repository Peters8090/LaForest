using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public static Weapon axe = new Weapon("Axe");
    public static Weapon flashlight = new Weapon("Flashlight");
    public static Weapon sword = new Weapon("Sword");
    public static Weapon rock = new Weapon("Rock");

    public string name;
    public float damage;
    public Texture2D image;

    public Weapon(string name)
    {
        this.name = name;
        switch(name)
        {
            default:
                damage = 0;
                break;
            case "Axe":
                damage = 10f;
                break;
            case "Flashlight":
                damage = 5f;
                break;
            case "Sword":
                damage = 20f;
                break;
            case "Rock":
                damage = 10f;
                break;
        }
        image = (Texture2D)Resources.Load(name + "Img");
    }
}
