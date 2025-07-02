using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;


public class GuiOpenScript : MonoBehaviour
{
    public GameObject gui;
    public Button primaryButton;
    public Button secundaryButton;
    public Button sightButton;
    public Button magazineButton;
    public Image weapon;
    public Sprite primaryPNG;
    public Sprite secundaryPNG;
    public GameObject SightInventory;
    public GameObject MagazineInventory;

    public ShootProjectile shootscript;

    void Start()
    {

        gui.SetActive(false);
        SightInventory.SetActive(false);
        MagazineInventory.SetActive(true);
        primaryButton.onClick.AddListener(() => changePicture(primaryPNG));
        secundaryButton.onClick.AddListener(() => changePicture(secundaryPNG));
        sightButton.onClick.AddListener(() => openInventory(SightInventory));
        magazineButton.onClick.AddListener(() => openInventory(MagazineInventory));


        Button[] children;
        children = SightInventory.GetComponentsInChildren<Button>();
        children[0].onClick.AddListener(() => equipScope(1.5f));
        children[1].onClick.AddListener(() => equipScope(2));
        children[2].onClick.AddListener(() => equipScope(5));

        children = MagazineInventory.GetComponentsInChildren<Button>();
        children[0].onClick.AddListener(() => equipMagazine(30, 0, 0));
        children[1].onClick.AddListener(() => equipMagazine(15, 0, 1));

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
    void openInventory(GameObject panel)
    {
        SightInventory.SetActive(false);
        MagazineInventory.SetActive(false);
        panel.SetActive(true);

    }

    void equipMagazine(int cap, int size, int bullet)
    {

        shootscript.cap = cap;

    }

    void equipScope(float zoom)
    {

        shootscript.zoom = zoom;

    }
}
