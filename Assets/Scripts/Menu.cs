using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static bool simulatePlay;

    void Start()
    {
        
    }

    void Update()
    {
        if (simulatePlay)
        {
            Play();
            GameObject.Find("NickInputField").GetComponent<InputField>().text = "Peters";
        }
    }

    public void Play()
    {
        Tests.initialized = true;
        GameObject.Find("MultiplayerGameControlObject").GetComponent<Connecting>().Play();
    }
}
