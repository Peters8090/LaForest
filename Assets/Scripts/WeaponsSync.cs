using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsSync : MonoBehaviour
{
    public Weapon activeWeapon;
    Weapon prevActiveWeapon;
    GameObject eq;

    void Start()
    {
        if (gameObject == UsefulReferences.player)
            enabled = false;
        eq = transform.Find("ybot/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Equipment").gameObject;
    }
    
    void Update()
    {
        if (prevActiveWeapon != activeWeapon)
            ChangeWeapon();
        prevActiveWeapon = activeWeapon;
    }

    void ChangeWeapon()
    {
        Destroy(eq.transform.GetChild(0).gameObject);
        GameObject go = Instantiate((GameObject)Resources.Load(activeWeapon.name));
        go.transform.parent = eq.transform;
        go.name = activeWeapon.name;
        go.transform.localPosition = ((GameObject)Resources.Load(activeWeapon.name)).transform.position;
        go.transform.localRotation = ((GameObject)Resources.Load(activeWeapon.name)).transform.rotation;
        go.transform.localScale = ((GameObject)Resources.Load(activeWeapon.name)).transform.localScale;
    }
}
