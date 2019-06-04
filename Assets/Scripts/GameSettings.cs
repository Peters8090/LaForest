using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class GameSettings : MonoBehaviour
{
    public static float volume;
    public static int graphicsIndex;
    public static int resolutionIndex;
    public static bool showGUI;
    public static bool showFPS;
    public static bool fullscreen;
    public static string nick;
    
    Toggle showGUIToggle;
    Toggle showFPSToggle;
    Toggle fullscreenToggle;
    Dropdown resolutionDropdown;
    Dropdown graphicsDropdown;
    Slider volumeSlider;
    InputField nickInputField;
    
    public static Resolution[] resolutions;

    void Start()
    {
        showGUIToggle = transform.Find("First column/Toggles/Show GUI").gameObject.GetComponent<Toggle>();
        showFPSToggle = transform.Find("First column/Toggles/Show FPS").gameObject.GetComponent<Toggle>();
        fullscreenToggle = transform.Find("First column/Toggles/Fullscreen").gameObject.GetComponent<Toggle>();
        resolutionDropdown = transform.Find("Second column/Resolution/ResolutionDropdown").gameObject.GetComponent<Dropdown>();
        graphicsDropdown = transform.Find("Second column/Graphics/GraphicsDropdown").gameObject.GetComponent<Dropdown>();
        volumeSlider = transform.Find("First column/Volume/Slider").gameObject.GetComponent<Slider>();
        nickInputField = transform.Find("Second column/Nick/NickInputField").gameObject.GetComponent<InputField>();

        SetUpResolutionDropdown();
        
        volume = SaveLoad.sd.volume;
        graphicsIndex = SaveLoad.sd.graphicsIndex;
        resolutionIndex = SaveLoad.sd.resolutionIndex;
        showGUI = SaveLoad.sd.showGUI;
        showFPS = SaveLoad.sd.showFPS;
        fullscreen = SaveLoad.sd.fullscreen;
        nick = SaveLoad.sd.nick;


        showGUIToggle.isOn = showGUI;
        showFPSToggle.isOn = showFPS;
        fullscreenToggle.isOn = fullscreen;
        resolutionDropdown.value = resolutionIndex;
        graphicsDropdown.value = graphicsIndex;
        volumeSlider.value = volume;
        nickInputField.text = nick;

        Refresh();

        gameObject.SetActive(false);
    }
    
    public void Volume(float volume)
    {
        GameSettings.volume = volume;
    }

    public void Graphics(int graphicsIndex)
    {
        GameSettings.graphicsIndex = graphicsIndex;
        Refresh();
    }

    public void Resolution(int resolutionIndex)
    {
        GameSettings.resolutionIndex = resolutionIndex;
        Refresh();
    }

    public void ShowGUI(bool showGUI)
    {
        GameSettings.showGUI = showGUI;
    }

    public void ShowFPS(bool showFPS)
    {
        GameSettings.showFPS = showFPS;
    }

    public void Fullscreen(bool fullscreen)
    {
        GameSettings.fullscreen = fullscreen;
        Refresh();
    }

    public void Nick(string nick)
    {
        GameSettings.nick = nick;
    }

    /// <summary>
    /// Refreshes settings using static variables above
    /// </summary>
    public static void Refresh()
    {
        QualitySettings.SetQualityLevel(graphicsIndex);
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
        Screen.fullScreen = fullscreen;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        // replace the element at index with the last element
        arr[index] = arr[arr.Length - 1];
        // finally, let's decrement Array's size by one
        Array.Resize(ref arr, arr.Length - 1);
    }

    /// <summary>
    /// Add resolutions to the resolution dropdown
    /// </summary>
    void SetUpResolutionDropdown()
    {
        int currentResolutionIndex = 0;
        resolutions = GetAvailableResolutions();
        
        Array.Reverse(resolutions);
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Returns a list of resolutions, which have the highest possible refresh rate due to Unity 2018.3.0f2 bug (Screen.resolutions returns resolutions with different refresh rates, but same width and height, so we get duplicates of resolutions differenting from others just refresh rate)
    /// </summary>
    /// <returns></returns>
    Resolution[] GetAvailableResolutions()
    {
        //first list of resolutions, that includes all refresh rates
        List<Resolution> resList1 = new List<Resolution>();
        //list of resolutions, that includes highest refresh rates
        List<Resolution> resList2 = new List<Resolution>();
        //list without any duplicates (to eliminate any resolution duplicates, which appeared for no reason)
        List<Resolution> resList3 = new List<Resolution>();

        //set the resList equal to screen resolutions which includes all refresh rates
        resList1 = Screen.resolutions.ToList();
        
        foreach (var item in resList1)
        {
            //we create new list of resolutions, that will include same resolutions with different refresh rates
            List<Resolution> resolutions = new List<Resolution>();
            //set resolutions equal to all resolutions that have same width and height but different refresh rate
            resolutions = resList1.Where(r => r.width == item.width && r.height == item.height).ToList();
            //sort resolutions by refresh rates
            resolutions.OrderBy(a => a.refreshRate);
            //add the resolution with the highest refresh rate in the resolutions list
            resList2.Add(resolutions[resolutions.Count-1]);
        }

        //eliminate any resolution duplicates, which appeared for no reason
        resList3 = resList2.Distinct().ToList();

        //convert resList2 to array and return correct list of resolutions
        return resList3.ToArray();
    }
}
