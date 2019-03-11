using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerDeath : MonoBehaviourPunCallbacks
{
    float timer = 0;
    float timerMax = 5f;
    public bool died = false;
    //to detect the moment of dying
    bool prevDied = false;
    Slider slider;

    void Start()
    {
        slider = UsefulReferences.deathUI.transform.Find("Slider").GetComponent<Slider>();
    }

    void Update()
    {
        if (died)
        {
            if (timer < timerMax)
            {
                timer += Time.deltaTime;
                slider.value = timer / timerMax;
            }
            else
            {
                UsefulReferences.playerHealth.Regenerate();
                timer = 0;
                died = false;
            }
        }
        prevDied = died;
    }
}
