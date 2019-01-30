using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public string name;
    public Sprite image;

    public Weapon(string name, Sprite image)
    {
        this.name = name;
        this.image = image;
    }
}
