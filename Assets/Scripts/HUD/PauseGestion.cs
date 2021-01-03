using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseGestion : MonoBehaviour
{
    public GameObject UIPause;
    public static bool isPaused;

    private void Start()
    {
        UIPause.SetActive(false);
        isPaused = false;
        Time.timeScale = isPaused ? 0 : 1;

    }

    // Update is called once per frame
    void Update()
    {
       // if (isPaused)
            if (Input.GetButtonDown("Cancel"))
            {
                isPaused = !isPaused;
                UIPause.SetActive(!UIPause.activeSelf);
                UnityEngine.UI.Button button = UIPause.GetComponentInChildren<UnityEngine.UI.Button>();
                if (button) button.Select();
                Time.timeScale = isPaused ? 0 : 1;
            }
    }
}
