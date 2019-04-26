using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlScript : MonoBehaviour
{
    GameObject framerateUI;
    public GameObject[] gui;

    void Start()
    {
        framerateUI = UsefulReferences.ui.transform.Find("Framerate").gameObject;
    }
    
    void Update()
    {
        if (!UsefulReferences.initialized)
        {
            framerateUI.SetActive(false);
            return;
        }

        if(!PauseMenu.menu)
        {
            framerateUI.SetActive(GameSettings.showFPS);
        } else
            framerateUI.SetActive(false);
        
        foreach(var element in gui)
        {
            if (GameSettings.showGUI)
            {
                //to enable the canvas
                element.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                //to disable the canvas
                element.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            }
        }
    }
}
