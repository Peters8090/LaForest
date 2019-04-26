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
    float timer = 0f;
    Text fpsText;
    
    void Start()
    {
        fpsText = GetComponent<Text>();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= delay)
        {
            timer = 0f;
            //convert frames per delay seconds to frames per second
            fpsText.text = Mathf.RoundToInt(frames / delay).ToString();
            //reset measured frames
            frames = 0;
        }
        else
        {
            frames++;
        }
    }
}
