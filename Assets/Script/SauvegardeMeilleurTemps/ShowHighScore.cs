using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowHighScore : MonoBehaviour
{
    public TMP_Text time;
    private void OnEnable()
    {
        TimeData timeData = SaveTimer.loadBestTime();
        if (timeData == null)
        {
            time.text = new string("NaNN:NaNN.NaNN");
        }
        else
        {
            if (timeData.CentiSecondes < 10)
            {
                if (timeData.Secondes < 10)
                    time.text = new string(timeData.Minutes + ":0" + timeData.Secondes + ".0" + timeData.CentiSecondes);
                else
                    time.text = new string(timeData.Minutes + ":" + timeData.Secondes + ".0" + timeData.CentiSecondes);
            }
            else
            {
                if (timeData.Secondes < 10)
                    time.text = new string(timeData.Minutes + ":0" + timeData.Secondes + "." + timeData.CentiSecondes);
                else
                    time.text = new string(timeData.Minutes + ":" + timeData.Secondes + "." + timeData.CentiSecondes);
            }
        }
    }
}
