using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveScript
{
    // Deze code komt voornamelijk uit deze video: https://www.youtube.com/watch?v=XOjd_qU2Ido

    public static void Save(SaveData data)
    {
        BinaryFormatter formatter = new();
        string path = Application.persistentDataPath + "/save.shoot";
        FileStream stream = new(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData Load()
    {
        string path = Application.persistentDataPath + "/save.shoot";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
