using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class Tests : MonoBehaviour
{
    public static bool initialized = false;

    void Awake()
    {
        if (!initialized && SceneManager.GetActiveScene().name == "Main")
        {
            Menu.simulatePlay = true;
            SceneManager.LoadScene("Menu");
        }
    }
}