using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulMethods
{
    public static Vector3 Vector3Abs(Vector3 v3)
    {
        return new Vector3(Mathf.Abs(v3.x), Mathf.Abs(v3.y), Mathf.Abs(v3.z));
    }
}
