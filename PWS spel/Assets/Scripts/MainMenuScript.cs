using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class MainMenuScript : MonoBehaviour
{
    public Slider volumeSlider;

    public AudioClip clickSound;
    public AudioSource audioSource;
    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        
    }

    public void PlayGame()
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(clickSound);
        print("quit game");
        Application.Quit();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.shoot";
        FileStream stream = new FileStream(path, FileMode.Create);

        string data = "piet";
        formatter.Serialize(stream, data);
        stream.Close();

    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/save.shoot";
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();

            Debug.Log(data);
            
        } else {
            Debug.Log("nofile");
        }
        

    }
    

}
