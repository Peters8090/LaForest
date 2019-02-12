using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
    public static bool menu = false;
    GameObject pauseMenuUIObj;
    Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();

    void Start()
    {
        pauseMenuUIObj = GameObject.Find("UI").transform.Find("Pause Menu").gameObject;

        //inserts to the dictionary all pause menu buttons; resume, info, settings, main menu, exit
        for (int i = 0; i < pauseMenuUIObj.transform.Find("Buttons").childCount; i++)
        {
            buttons.Add((pauseMenuUIObj.transform.Find("Buttons").GetChild(i).name), (pauseMenuUIObj.transform.Find("Buttons").GetChild(i).gameObject));
        }

        //inserts to the dictionary all pause menu panels; resume, info, settings, main menu, exit
        for (int i = 0; i < pauseMenuUIObj.transform.Find("Panels").childCount; i++)
        {
            panels.Add((pauseMenuUIObj.transform.Find("Panels").GetChild(i).name), (pauseMenuUIObj.transform.Find("Panels").GetChild(i).gameObject));
        }

        //panels["Settings"].SetActive(true);
    }

    void Update()
    {
        if (UsefulReferences.initialized)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                menu = false;
            }
            else if (Input.GetButtonDown("Menu"))
            {
                menu = !menu;
            }
        } else
        {
            SetPanelsActive("");
            menu = false;
        }

        bool CheckPanelsActive()
        {
            int activeCounter = 0;
            foreach(var item in panels)
            {
                if (item.Value.activeSelf)
                {
                    activeCounter++;
                }
            }
            if (activeCounter == panels.Count)
                return true;
            else
                return false;
        }

        if(!menu)
        {
            if (!CheckPanelsActive())
            {
                SetBtnColor("");
                SetPanelsActive("");
            }
        }
        
        pauseMenuUIObj.SetActive(menu);
    }

    public void Resume()
    {
        SetBtnColor("Resume");
        SetPanelsActive("Resume");
        menu = false;
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

    public void MainMenu()
    {
        SetBtnColor("MainMenu");
        SetPanelsActive("MainMenu");
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        GetComponent<MainMenu>().SetGame(true);
        UsefulReferences.initialized = false;
    }

    public void Exit()
    {
        SetBtnColor("Exit");
        SetPanelsActive("Exit");
        Application.Quit();
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
