using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : Singleton<Timer>
{
    public bool Actif = true;
    public TMP_Text timer;
    public int minute, seconde, centiseconde;
    private void Awake()
    {
        minute = 0;
        seconde = 0;
        centiseconde = 0;
    }

    public void TimerStart() => StartCoroutine(Clock());
    IEnumerator Clock()
    {
        while (Actif)
        {
            yield return new WaitForSeconds(0.01f);
            centiseconde++;
            if (centiseconde == 60)
            {
                centiseconde = 0;
                seconde++;
                if (seconde == 60)
                {
                    seconde = 0;
                    minute++;
                }
            }

            if (centiseconde < 10)
            {
                if (seconde < 10)
                    timer.text = new string(minute + ":0" + seconde + ".0" + centiseconde);
                else
                    timer.text = new string(minute + ":" + seconde + ".0" + centiseconde);
            }
            else
            {
                if (seconde < 10)
                    timer.text = new string(minute + ":0" + seconde + "." + centiseconde);
                else
                    timer.text = new string(minute + ":" + seconde + "." + centiseconde);
            }
                
        }
    }
}
