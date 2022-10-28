using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : Singleton<Pause>
{
    public GameObject pausePanel;

    private void Start()
    {
        pausePanel.SetActive(false);
    }
    public void SetPause()
    {
        pause = true;
        Time.timeScale = 0.000001f;
        pausePanel.SetActive(true); 
    }
    public void Resume()
    {
        pause = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    public void Quit()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public bool pause;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pause)
        {
            SetPause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pause)
        {
            Resume();
        }
        
    }
}
