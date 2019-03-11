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
        if (tests)
        {
            GameObject player;
            player = GameObject.Find("Player");

            Behaviour[] scripts = player.GetComponents<Behaviour>();
            Behaviour[] scriptsChildren = player.GetComponentsInChildren<Behaviour>();
            foreach (Behaviour script in scripts)
            {
                script.enabled = false;
            }
            foreach (Behaviour script in scriptsChildren)
            {
                script.enabled = false;
            }
        }
        else
            Destroy(GameObject.Find("Player"));

    }
}