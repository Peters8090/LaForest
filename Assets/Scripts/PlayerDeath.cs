using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    float timer = 0;
    float timerMax = 1.5f;
    public bool died = false;
    Slider slider;

    void Start()
    {
        slider = UsefulReferences.deathUI.transform.Find("Slider").GetComponent<Slider>();
    }
    
    void Update()
    {
        if(died)
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
    }

    public void Die()
    {
        died = true;
    }
}
