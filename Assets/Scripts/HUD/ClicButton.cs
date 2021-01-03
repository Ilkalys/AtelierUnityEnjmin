using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ClicButton : MonoBehaviour
{

    public void LoadSceneMode(string sceneName)
    {
        if (sceneName != "MainScene")
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (System.DateTime.Now.DayOfYear > PlayerPrefs.GetInt("DayOfYear", -1) || System.DateTime.Now.Year > PlayerPrefs.GetInt("Year", -1))
            {
                PlayerPrefs.SetInt("DayOfYear", System.DateTime.Now.DayOfYear);
                PlayerPrefs.SetInt("Year", System.DateTime.Now.Year);

                SceneManager.LoadScene(sceneName);
            }
        }

    }

    public void TurnOffPause(GameObject UI)
    {
        PauseGestion.isPaused = false;
        Time.timeScale = 1;
        UI.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void switchPanel(GameObject PanelToActiv)
    {
        PanelToActiv.SetActive(true);
        UnityEngine.UI.Button but = PanelToActiv.GetComponentInChildren<UnityEngine.UI.Button>();
        if (but) but.Select();
        this.gameObject.SetActive(false);
    }
}
