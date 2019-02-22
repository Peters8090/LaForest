using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulMethods : MonoBehaviour
{
    /// <summary>
    /// Finds the parent of parents
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static GameObject FindTopParent(GameObject go)
    {
        GameObject parent = go;
        while (parent.transform.parent != null)
        {
            parent = parent.transform.parent.gameObject;
        }
        return parent;
    }
}
