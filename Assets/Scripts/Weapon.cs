using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public static Weapon axeTemplate = new Weapon("Axe");
    public static Weapon flashlightTemplate = new Weapon("Flashlight");
    public static Weapon swordTemplate = new Weapon("Sword");
    public static Weapon rockTemplate = new Weapon("Rock");

    public string name;
    public Texture2D image;

    public Weapon(string name)
    {
        this.name = name;
        image = (Texture2D)Resources.Load(name + "Img");
    }
}
