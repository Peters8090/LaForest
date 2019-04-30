using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//author: https://novack.itch.io/borderless

public class WindowScript : MonoBehaviour, IDragHandler
{
    public Vector2Int defaultWindowSize;
    public Vector2Int borderSize;

    private Vector2 _deltaValue = Vector2.zero;
    private bool _maximized;
    

    void Update()
    {
        SceneManager.activeSceneChanged += ResetFramedVar;

        if (!Application.isEditor && SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            transform.parent.gameObject.GetComponent<Canvas>().enabled = !Screen.fullScreen;

            BorderlessWindow.SetFramelessWindow();
        } else
        {
            transform.parent.gameObject.GetComponent<Canvas>().enabled = false;
        }

    }

    void ResetFramedVar(Scene current, Scene next)
    {
        BorderlessWindow.framed = false;
    }

    public void OnBorderBtnClick()
    {
        if (BorderlessWindow.framed)
            return;

        BorderlessWindow.SetFramedWindow();        
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width + borderSize.x, Screen.height + borderSize.y); // Compensating the border offset.
    }

    public void OnNoBorderBtnClick()
    {
        if (!BorderlessWindow.framed)
            return;

        BorderlessWindow.SetFramelessWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width - borderSize.x, Screen.height - borderSize.y);
    }

    public void ResetWindowSize()
    {
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, defaultWindowSize.x, defaultWindowSize.y);
    }

    public void OnCloseBtnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
    }

    public void OnMinimizeBtnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        BorderlessWindow.MinimizeWindow();
    }

    public void OnMaximizeBtnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (_maximized)
            BorderlessWindow.RestoreWindow();
        else
            BorderlessWindow.MaximizeWindow();

        _maximized = !_maximized;
    }

    public void OnDrag(PointerEventData data)
    {
        if (BorderlessWindow.framed)
            return;

        _deltaValue += data.delta;
        if (data.dragging)
        {
            BorderlessWindow.MoveWindowPos(_deltaValue, Screen.width, Screen.height);
        }
    }
}
