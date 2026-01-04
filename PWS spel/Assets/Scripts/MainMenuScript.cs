using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
public class MainMenuScript : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider difficultySlider;
    public TextMeshProUGUI currentDifficultyText;

    public AudioClip clickSound;
    public AudioSource audioSource;
    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        difficultySlider.value = PlayerHealth.difficulty;
        ChangeDifficulty();
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

    public void ChangeDifficulty()
    {
        PlayerHealth.difficulty = difficultySlider.value;
        if (difficultySlider.value == 0) currentDifficultyText.text = "Creative";
        else if (difficultySlider.value < 0.15f) currentDifficultyText.text = "Very easy";
        else if (difficultySlider.value < 0.3f) currentDifficultyText.text = "Easy";
        else if (difficultySlider.value < 0.5f) currentDifficultyText.text = "Moderate";
        else if (difficultySlider.value < 0.8f) currentDifficultyText.text = "Hard";
        else currentDifficultyText.text = "Impossible";
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
