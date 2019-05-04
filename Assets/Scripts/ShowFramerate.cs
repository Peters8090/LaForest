using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFramerate : MonoBehaviour
{
    /// <summary>
    /// Delay between framerate measures
    /// </summary>
    float delay = 0.8f;
    int frames = 0;
    float timer = 0;
    Text fpsText;
    public static int currentFPS = 60;
    
    void Start()
    {
        timer = delay;
        fpsText = GetComponent<Text>();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= delay)
        {
            timer = 0f;
            //convert frames per delay seconds to frames per second
            currentFPS = Mathf.RoundToInt(frames / delay);
            //reset measured frames
            frames = 0;
        }
        else
        {
            frames++;
        }

        fpsText.text = currentFPS.ToString();
    }
}
