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
        //for instance when main menu is not active
        if (!UsefulReferences.initialized)
        {
            framerateUI.SetActive(false);
            return;
        }

        #region ShowFPS

        if (!PauseMenu.menu)
        {
            framerateUI.SetActive(GameSettings.showFPS);
        } else
            framerateUI.SetActive(false);

        #endregion

        #region ShowGUI

        //disable the canvas component in all gui gameObjects (because we can't change the gameObject's active)
        foreach (var canvas in gui)
        {
            canvas.GetComponent<Canvas>().enabled = GameSettings.showGUI;
        }

        foreach(var player3dText in GameObject.FindObjectsOfType<PlayerNickTextMesh>())
        {
            player3dText.gameObject.GetComponent<MeshRenderer>().enabled = GameSettings.showGUI;
        }

        #endregion
    }
}
