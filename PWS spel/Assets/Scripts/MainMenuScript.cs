using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
}
