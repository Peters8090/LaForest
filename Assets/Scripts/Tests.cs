using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class Tests : MonoBehaviour
{
    public static bool tests = true;

    void Awake()
    {
        if (tests)
            tests = Application.isEditor;
    }
}