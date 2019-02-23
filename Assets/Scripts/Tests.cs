using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;

public class Tests : MonoBehaviour
{
    public static bool tests = false;

    void Awake()
    {
        if (tests)
            tests = Application.isEditor;
        else
            GameObject.Find("Player").SetActive(false);
    }
}