using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMain : MonoBehaviour
{
    public GameObject slider;
    public GameObject text;

    void Start()
    {
        StartCoroutine("AsynchronousLoad");
    }

    void Update()
    {
        
    }

    IEnumerator AsynchronousLoad()
    {
        string scene = "Main";
        yield return null;

        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            slider.GetComponent<Slider>().value = progress;
            
            if (ao.progress == 0.9f)
            {
                text.GetComponent<Text>().text = "Game loaded";
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
