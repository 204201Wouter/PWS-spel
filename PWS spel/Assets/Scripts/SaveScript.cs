using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveScript : MonoBehaviour
{
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.shoot";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = null;
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public SaveData Load()
    {
        string path = Application.persistentDataPath + "/save.shoot";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

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
