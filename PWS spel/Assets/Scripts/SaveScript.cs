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

        string data = "henk";
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public string Load()
    {
        string path = Application.persistentDataPath + "/save.shoot";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
