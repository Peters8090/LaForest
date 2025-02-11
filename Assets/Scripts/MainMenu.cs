﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static bool menu;
    GameObject mainMenuUIObj;
    GameObject mainMenuCameraObj;
    Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();

    void Start()
    {
        mainMenuUIObj = GameObject.Find("UI").transform.Find("Main Menu").gameObject;
        SetUpGame(true);

        //inserts to the dictionary all main menu buttons; play, info, settings, copyrights, exit
        for (int i = 0; i < mainMenuUIObj.transform.Find("Buttons").childCount; i++)
        {
            buttons.Add((mainMenuUIObj.transform.Find("Buttons").GetChild(i).name), (mainMenuUIObj.transform.Find("Buttons").GetChild(i).gameObject));
        }

        //inserts to the dictionary all main menu panels; play, info, settings, copyrights, exit
        for (int i = 0; i < mainMenuUIObj.transform.Find("Panels").childCount; i++)
        {
            panels.Add((mainMenuUIObj.transform.Find("Panels").GetChild(i).name), (mainMenuUIObj.transform.Find("Panels").GetChild(i).gameObject));
        }

        //setup game on its start with saved game settings
        panels["Settings"].GetComponent<GameSettings>().Start();
    }

    void Update()
    {
        if (Tests.tests)
        {
            Play();
        }
    }
        
    public void Play()
    {
        SetBtnColor("Play");
        SetPanelsActive("Play");
        GameObject.Find("MultiplayerGameControlObject").GetComponent<MultiplayerGameControlScript>().Play();
    }

    public void Info()
    {
        SetBtnColor("Info");
        SetPanelsActive("Info");
    }

    public void Settings()
    {
        SetBtnColor("Settings");
        SetPanelsActive("Settings");
    }

    public void Copyrights()
    {
        SetBtnColor("Copyrights");
        SetPanelsActive("Copyrights");
    }

    public void Exit()
    {
        SetBtnColor("Exit");
        SetPanelsActive("Exit");
        Application.Quit();
    }

    /// <summary>
    /// Set all game objects active, to make them not disturb us while main menu is active
    /// </summary>
    /// <param name="how"></param>
    public void SetUpGame(bool how)
    {
        GameObject.Find("UI").transform.Find("Main Menu").gameObject.SetActive(how);
        UsefulReferences.activeWeaponImg.transform.parent.gameObject.SetActive(!how);
        GameObject.Find("Environment").transform.Find("Main Menu Objects").gameObject.SetActive(how);
        SetBtnColor("");
        SetPanelsActive("");
        menu = how;
    }

    /// <summary>
    /// Set the color of the button unique
    /// </summary>
    /// <param name="btnName"></param>
    void SetBtnColor(string btnName)
    {
        foreach (var item in buttons)
        {
            if (item.Value.name == btnName)
                buttons[btnName].GetComponent<Image>().color = new Color(buttons[btnName].GetComponent<Image>().color.r, buttons[btnName].GetComponent<Image>().color.g, buttons[btnName].GetComponent<Image>().color.b, 0.5f);
            else
                buttons[item.Value.name].GetComponent<Image>().color = new Color(buttons[item.Value.name].GetComponent<Image>().color.r, buttons[item.Value.name].GetComponent<Image>().color.g, buttons[item.Value.name].GetComponent<Image>().color.b, 1f);
        }
    }

    /// <summary>
    /// Set main menu panels active (lastly clicked is active)
    /// </summary>
    /// <param name="panelName"></param>
    void SetPanelsActive(string panelName)
    {
        foreach (var item in panels)
        {
            if (item.Value.name == panelName)
                panels[panelName].SetActive(true);
            else
                panels[item.Value.name].SetActive(false);
        }
    }
}
