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
    public Button sightSlot;
    public Button magazineSlot;
    public Image weapon;
    public Sprite primaryPNG;
    public Sprite secundaryPNG;
    public GameObject sightInventory;
    public GameObject magazineInventory;
    public GameObject attachmentButton;
    public TextMeshProUGUI inventoryText;
    GameObject activeInventory;

    public ShootProjectile shootscript;
    public MouseLook mouseLook;
    public Movement movement;
    public WeaponScript weaponScript;

    void Start()
    {
        gui.SetActive(false);
        sightInventory.SetActive(false);
        magazineInventory.SetActive(true);
        primaryButton.onClick.AddListener(() => ChangePicture(primaryPNG));
        secundaryButton.onClick.AddListener(() => ChangePicture(secundaryPNG));
        sightSlot.onClick.AddListener(() => OpenInventory(sightInventory));
        magazineSlot.onClick.AddListener(() => OpenInventory(magazineInventory));

        activeInventory = magazineInventory;
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
                movement.canMove = true;
                mouseLook.canLook = true;
                shootscript.canShoot = true;
            }
            else
            {
                gui.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                movement.canMove = false;
                mouseLook.canLook = false;
                shootscript.canShoot = false;
            }
        }
    }

    void ChangePicture(Sprite picture)
    {
        weapon.sprite = picture;
    }
    void OpenInventory(GameObject panel)
    {
        activeInventory.SetActive(false);
        panel.SetActive(true);
        activeInventory = panel;
        inventoryText.text = panel.name;
    }

    void EquipMagazine(MagazineAttachment magazine, GameObject button)
    {
        shootscript.shotCooldown = magazine.shotCooldown;
        shootscript.cap = magazine.capacity;
        shootscript.reloadTime = magazine.reloadTime;
        shootscript.damage = magazine.ammoType.damage;
        shootscript.amount = magazine.ammoType.amount;
        shootscript.spread = magazine.ammoType.spread;
        shootscript.size = magazine.ammoType.size;
        weaponScript.ammoAmounts[shootscript.ammoType] += shootscript.ammo;
        shootscript.ammoType = magazine.ammoType.name;
        shootscript.recoil = magazine.ammoType.recoil;
        shootscript.ammo = 0;
        shootscript.UpdateAmmoText();

        SetActiveIfExists(weaponScript.currentMagazine.model, false);
        weaponScript.availableMagazines.Add(weaponScript.currentMagazine.name);
        weaponScript.currentMagazine = magazine;
        weaponScript.availableMagazines.Remove(magazine.name);
        SetActiveIfExists(magazine.model, true);


        magazineSlot.transform.GetChild(0).SetParent(magazineInventory.transform);
        button.transform.SetParent(magazineSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);
    }

    public void ClickMagazine(MagazineAttachment magazine, GameObject button)
    {
        if (activeInventory == magazineInventory)
        {
            if (magazine.name != weaponScript.currentMagazine.name) EquipMagazine(magazine, button);
        }
        else OpenInventory(magazineInventory);
    }

    void EquipScope(ScopeAttachment scope, GameObject button)
    {
        shootscript.zoom = scope.zoomFactor;

        if (weaponScript.currentScope.name != "no scope")
        {
            weaponScript.availableScopes.Add(weaponScript.currentScope.name);
            sightSlot.transform.GetChild(0).SetParent(sightInventory.transform);
            SetActiveIfExists(weaponScript.currentScope.model, false);
        }
        weaponScript.currentScope = scope;
        weaponScript.availableScopes.Remove(scope.name);
        SetActiveIfExists(scope.model, true);

        button.transform.SetParent(sightSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);
    }

    void UnequipScope(GameObject button)
    {
        weaponScript.availableScopes.Add(weaponScript.currentScope.name);
        weaponScript.currentScope = weaponScript.scopes["no scope"];
        shootscript.zoom = 2;
        SetActiveIfExists(weaponScript.currentScope.model, false);

        button.transform.SetParent(sightInventory.transform);
    }

    void ClickScope(ScopeAttachment scope, GameObject button)
    {
        if (activeInventory == sightInventory)
        {
            if (scope.name != weaponScript.currentScope.name) EquipScope(scope, button);
            else UnequipScope(button);
        }
        else OpenInventory(sightInventory);
    }

    public void NewMagazine(MagazineAttachment magazine)
    {
        GameObject button = Instantiate(attachmentButton);
        button.transform.SetParent(magazineInventory.transform);
        button.GetComponent<Button>().onClick.AddListener(() => ClickMagazine(magazine, button));
        button.GetComponent<Image>().color = Random.ColorHSV();
    }

    public void NewScope(ScopeAttachment scope)
    {
        GameObject button = Instantiate(attachmentButton);
        button.transform.SetParent(sightInventory.transform);
        button.GetComponent<Button>().onClick.AddListener(() => ClickScope(scope, button));
        button.GetComponent<Image>().color = Random.ColorHSV();
    }

    public void SetActiveIfExists(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }
}
