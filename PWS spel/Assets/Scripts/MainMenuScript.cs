using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainMenuScript : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider difficultySlider;
    public TextMeshProUGUI currentDifficultyText;

    public AudioClip clickSound;
    public AudioSource audioSource;

    public static bool newGame = true;
    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        difficultySlider.value = PlayerHealth.difficulty;
        ChangeDifficulty();
    }

    public void NewGame()
    {
        audioSource.PlayOneShot(clickSound);
        newGame = true;
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

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shoot"))
        {
            newGame = false;
            print("load button pressed");
            SceneManager.LoadScene("Game");
        }
    }
}
