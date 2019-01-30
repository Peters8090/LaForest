using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public string name;
    public Sprite image;

    public Weapon(string name)
    {
        this.name = name;
        Texture2D imgTexture;
        imgTexture = (Texture2D)Resources.Load(name + "Img");
        this.image = Sprite.Create(imgTexture, new Rect(0f, 0f, imgTexture.width, imgTexture.height), new Vector2(0.5f, 0.5f), 100f);
    }
}
