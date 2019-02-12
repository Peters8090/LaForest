using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class Tests : MonoBehaviour
{
    public static bool tests = false;

    void Awake()
    {
        tests = !Application.isEditor;
    }
}