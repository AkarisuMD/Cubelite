using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreen : Singleton<DeathScreen>
{
    public GameObject all, other, highscord;
    public RawImage fond;
    public TMP_Text time;

    private void Start()
    {
        all.SetActive(false); other.SetActive(false); highscord.SetActive(false);
    }

    public void Dead() => StartCoroutine(_Dead());
    IEnumerator _Dead()
    {
        all.SetActive(true); other.SetActive(false); highscord.SetActive(false);
        fond.color = new Color(0, 0, 0, 0);
        while (fond.color.a < 0.96f)
        {
            fond.color += new Color(0, 0, 0, 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        other.SetActive(true); highscord.SetActive(false);
        Time.timeScale = 0f;
        SetTime(Timer.Instance.minute, Timer.Instance.seconde, Timer.Instance.centiseconde);
    }

    public void SetTime(int minute, int seconde, int centiseconde)
    {
        if (centiseconde < 10)
        {
            if (seconde < 10)
                time.text = new string(minute + ":0" + seconde + ".0" + centiseconde);
            else
                time.text = new string(minute + ":" + seconde + ".0" + centiseconde);
        }
        else
        {
            if (seconde < 10)
                time.text = new string(minute + ":0" + seconde + "." + centiseconde);
            else
                time.text = new string(minute + ":" + seconde + "." + centiseconde);
        }

        TimeData timeData = SaveTimer.loadBestTime();
        if (timeData == null)
        {
            highscord.SetActive(true);
            SaveTimer.SaveTime(new TimeData(minute, seconde, centiseconde));
        }
        else
        {
            if (timeData.Minutes > minute) return;
            if (timeData.Secondes > seconde) return;
            if (timeData.CentiSecondes > centiseconde) return;

            highscord.SetActive(true);
            SaveTimer.SaveTime(new TimeData(minute, seconde, centiseconde));
        }
    }

    public void Retry()
    {
        MusiqueManager.Instance.GetComponent<AudioSource>().Stop();
        GameManager.Instance.StartGame();
    }
    public void Quit()
    {
        MusiqueManager.Instance.GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
