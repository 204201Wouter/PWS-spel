using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public Image magazineImage;
    public Image ammoImage;
    public Transform equippedMagazineStats;
    public Transform selectedMagazineStats;

    public ShootProjectile shootscript;
    public MouseLook mouseLook;
    public Movement movement;
    public WeaponScript weaponScript;

    public GameObject settingsMenu;
    public Slider volumeSlider;

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

        shootscript.UpdateAmmoText();

        magazineSlot.transform.GetChild(0).GetComponent<AttachmentButtonScript>().magazineAttachment = weaponScript.currentMagazine;

        volumeSlider.value = AudioListener.volume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !settingsMenu.activeSelf)
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

        if (Input.GetKeyDown(KeyCode.Escape) && !gui.activeSelf)
        {
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                movement.canMove = true;
                mouseLook.canLook = true;
                shootscript.canShoot = true;
            }
            else
            {
                settingsMenu.SetActive(true);
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
    void OpenInventory(GameObject newInventory)
    {
        activeInventory.SetActive(false);
        newInventory.SetActive(true);
        activeInventory = newInventory;
        inventoryText.text = newInventory.name;
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

        ChangeMagazineStats(magazine, true);

        SetActiveIfExists(weaponScript.currentMagazine.model, false);
        weaponScript.availableMagazines.Add(weaponScript.currentMagazine.name);
        weaponScript.currentMagazine = magazine;
        weaponScript.availableMagazines.Remove(magazine.name);
        SetActiveIfExists(magazine.model, true);


        magazineSlot.transform.GetChild(0).SetParent(magazineInventory.transform);
        button.transform.SetParent(magazineSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);

        magazineImage.sprite = magazine.sprite;
        ammoImage.sprite = magazine.ammoType.sprite;
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
        }
        SetActiveIfExists(weaponScript.currentScope.model, false);
        weaponScript.currentScope = scope;
        weaponScript.availableScopes.Remove(scope.name);
        SetActiveIfExists(scope.model, true);

        button.transform.SetParent(sightSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);
    }

    void UnequipScope(GameObject button)
    {
        weaponScript.availableScopes.Add(weaponScript.currentScope.name);
        SetActiveIfExists(weaponScript.currentScope.model, false);
        weaponScript.currentScope = weaponScript.scopes["no scope"];
        shootscript.zoom = 2;
        SetActiveIfExists(weaponScript.currentScope.model, true);

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
        if (!weaponScript.availableMagazines.Contains(magazine.name) && weaponScript.currentMagazine.name != magazine.name)
        {
            weaponScript.availableMagazines.Add(magazine.name);
            GameObject button = Instantiate(attachmentButton);
            button.transform.SetParent(magazineInventory.transform);
            button.GetComponent<Button>().onClick.AddListener(() => ClickMagazine(magazine, button));
            button.transform.GetChild(0).GetComponent<Image>().sprite = magazine.sprite;
            button.transform.GetChild(1).GetComponent<Image>().sprite = magazine.ammoType.sprite;
            Destroy(button.transform.GetChild(2).gameObject);
            button.GetComponent<AttachmentButtonScript>().magazineAttachment = magazine;
        }
    }

    public void NewScope(ScopeAttachment scope)
    {
        if (!weaponScript.availableScopes.Contains(scope.name) && weaponScript.currentScope.name != scope.name)
        {
            weaponScript.availableScopes.Add(scope.name);
            GameObject button = Instantiate(attachmentButton);
            button.transform.SetParent(sightInventory.transform);
            button.GetComponent<Button>().onClick.AddListener(() => ClickScope(scope, button));
            if (scope.sprite != null) button.transform.GetChild(2).GetComponent<Image>().sprite = scope.sprite;
            Destroy(button.transform.GetChild(0).gameObject);
            Destroy(button.transform.GetChild(1).gameObject);
        }
    }

    public void ChangeMagazineStats(MagazineAttachment magazine, bool currentMagazine)
    {
        if (currentMagazine)
        {
            equippedMagazineStats.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.ammoType.name[0].ToString().ToUpper() + magazine.ammoType.name[1..];
            equippedMagazineStats.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.ammoType.damage.ToString();
            equippedMagazineStats.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = Mathf.Round(1 / magazine.shotCooldown).ToString();
            equippedMagazineStats.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.capacity.ToString();
            equippedMagazineStats.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.reloadTime.ToString();
        }
        else
        {
            print(magazine.name);
            print(magazine.ammoType.name);
            selectedMagazineStats.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.ammoType.name[0].ToString().ToUpper() + magazine.ammoType.name[1..];
            selectedMagazineStats.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.ammoType.damage.ToString();
            selectedMagazineStats.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = Mathf.Round(1 / magazine.shotCooldown).ToString();
            selectedMagazineStats.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.capacity.ToString();
            selectedMagazineStats.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.reloadTime.ToString();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SetActiveIfExists(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }
}
