using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveTimer
{
    public static void SaveTime(TimeData timeData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/time.aka";
        FileStream stream = new FileStream(path, FileMode.Create);

        TimeData data = new TimeData(Timer.Instance.minute, Timer.Instance.seconde, Timer.Instance.centiseconde);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static TimeData loadBestTime()
    {
        string path = Application.persistentDataPath + "/time.aka";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            TimeData data = formatter.Deserialize(stream) as TimeData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Doesn't Exist");
            return null;
        }
    }
}
