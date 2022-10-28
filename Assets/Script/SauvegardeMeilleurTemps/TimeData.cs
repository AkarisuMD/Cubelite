using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeData
{
    public int Minutes;
    public int Secondes;
    public int CentiSecondes;
    public TimeData(int minutes, int secondes, int centiSecondes)
    {
        Minutes = minutes;
        Secondes = secondes;
        CentiSecondes = centiSecondes;
    }
}
