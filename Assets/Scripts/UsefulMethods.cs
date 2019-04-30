using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulMethods
{
    public static Vector3 Vector3Abs(Vector3 v3)
    {
        return new Vector3(Mathf.Abs(v3.x), Mathf.Abs(v3.y), Mathf.Abs(v3.z));
    }

    /// <summary>
    /// Finds child with specific name, while searching includes all childs, not like transform.Find
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChild(Transform parent, string name)
    {
        foreach (var child in parent.GetComponentsInChildren<Transform>())
        {
            if(child.name == name)
            {
                return child;
            }
        }
        return null;
    }
}
