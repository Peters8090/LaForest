using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public string name;
    public Texture2D image;

    public Weapon(string name)
    {
        this.name = name;
        image = (Texture2D)Resources.Load(name + "Img");
    }
}
