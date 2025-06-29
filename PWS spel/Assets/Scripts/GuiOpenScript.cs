using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiOpenScript : MonoBehaviour
{
    public GameObject gui;
    public Button primaryButton;
    public Button secundaryButton;
    public Image weapon;
    public Sprite primaryPNG;
    public Sprite secundaryPNG;

    void Start()
    {

        gui.SetActive(false);
        primaryButton.onClick.AddListener(() => changePicture(primaryPNG));
        secundaryButton.onClick.AddListener(() => changePicture(secundaryPNG));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (gui.activeSelf)
            {
                gui.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                gui.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

        }

    }

    void changePicture(Sprite picture)
    {
        weapon.sprite = picture;

    }
}
