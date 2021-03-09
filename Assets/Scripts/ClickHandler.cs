using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NavnoireCoding.Managers;

public class ClickHandler : MonoBehaviour
{

    public void LoadLevel(String name)
    {
        Time.timeScale = 1f;
        GameManager.Instance.StartGame(name);
    }

    public void ReplayLevel()
    {
        GameManager.Instance.ReplayGame();
    }

    public void SetLevelData(LevelData_SO data)
    {
        GameManager.Instance.SetLevelData(data);
    }

    public void TogglePauseState()
    {
        GameManager.Instance.TogglePauseState();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/TrailsScreenshot_" + Time.timeSinceLevelLoad + ".png", 2);

    }

}
