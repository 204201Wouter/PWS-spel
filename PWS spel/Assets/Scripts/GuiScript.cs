using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GuiScript : MonoBehaviour
{
    public Image sightBig;
    public Image magazineBig;
    public GameObject gui;
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

    public InteractScript interactScript;

    public GameObject settingsMenu;
    public Slider volumeSlider;
    public Slider difficultySlider;
    public TextMeshProUGUI currentDifficultyText;

    public GameObject deathScreen;

    public GameObject map;
    public RectTransform playerMarker;
    public GameObject minimap;
    public RectTransform minimapImage;
    public RectTransform miniPlayerMarker;

    public AudioSource audioSource;
    public AudioClip clickSound;

    public TextMeshProUGUI objectiveText;
    int currentObjectiveIndex = 0;
    readonly List<string> objectives = new();
    readonly List<int> objectiveRequiredAmounts = new();
    int currentAmountDone;

    bool menuOpen = false;

    void Start()
    {
        gui.SetActive(false);
        sightInventory.SetActive(false);
        magazineInventory.SetActive(true);
        sightSlot.onClick.AddListener(() => OpenInventory(sightInventory));
        magazineSlot.onClick.AddListener(() => OpenInventory(magazineInventory));

        activeInventory = magazineInventory;

        shootscript.UpdateAmmoText();

        magazineSlot.transform.GetChild(1).GetComponent<AttachmentButtonScript>().magazineAttachment = weaponScript.currentMagazine;

        volumeSlider.value = AudioListener.volume;
        difficultySlider.value = PlayerHealth.difficulty;
        ChangeDifficulty();

        objectives.Add("Kill enemies");
        objectiveRequiredAmounts.Add(10);
        objectives.Add("Download the map from a computer");
        objectiveRequiredAmounts.Add(1);
        objectives.Add("Open the 2 marked storage boxes");
        objectiveRequiredAmounts.Add(2);
        objectives.Add("Disable the engines");
        objectiveRequiredAmounts.Add(2);
        objectives.Add("Plant the bomb at the fuel tanks");
        objectiveRequiredAmounts.Add(1);
        objectives.Add("Disable the gravity generator");
        objectiveRequiredAmounts.Add(1);
        objectives.Add("Escape the spaceship");
        objectiveRequiredAmounts.Add(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (gui.activeSelf)
            {
                gui.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                movement.canMove = true;
                mouseLook.canLook = true;
                shootscript.canShoot = true;
                menuOpen = false;
            }
            else if (!menuOpen)
            {
                gui.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                movement.canMove = false;
                mouseLook.canLook = false;
                shootscript.canShoot = false;
                menuOpen = true;
            }
            audioSource.PlayOneShot(clickSound);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                movement.canMove = true;
                mouseLook.canLook = true;
                shootscript.canShoot = true;
                menuOpen = false;
            }
            else if (!menuOpen)
            {
                settingsMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                movement.canMove = false;
                mouseLook.canLook = false;
                shootscript.canShoot = false;
                menuOpen = true;
            }
            audioSource.PlayOneShot(clickSound);
        }

        if (Input.GetKeyDown(KeyCode.M) && interactScript.mapDownloaded)
        {
            if (map.activeSelf)
            {
                map.SetActive(false);
                minimap.SetActive(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                movement.canMove = true;
                mouseLook.canLook = true;
                shootscript.canShoot = true;
                menuOpen = false;
            }
            else if (!menuOpen)
            {
                map.SetActive(true);
                minimap.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                movement.canMove = false;
                mouseLook.canLook = false;
                shootscript.canShoot = false;
                menuOpen = true;
            }
            audioSource.PlayOneShot(clickSound);
        }

        if (interactScript.mapDownloaded)
        {
            Vector2 markerPos = new((transform.position.x - 135f) * 1.743f, (transform.position.z + 57f) * 1.743f);
            float markerRot = -transform.rotation.eulerAngles.y - 135;
            if (map.activeSelf)
            {
                playerMarker.anchoredPosition = markerPos;
                playerMarker.rotation = Quaternion.Euler(0, 0, markerRot);
            }
            else
            {
                minimapImage.anchoredPosition = 2f * markerPos;
                miniPlayerMarker.rotation = Quaternion.Euler(0, 0, markerRot);
            }
        }
    }

    void OpenInventory(GameObject newInventory)
    {
        activeInventory.SetActive(false);
        newInventory.SetActive(true);
        activeInventory = newInventory;
        inventoryText.text = newInventory.name;
        audioSource.PlayOneShot(clickSound);
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

        magazineSlot.transform.GetChild(1).SetParent(magazineInventory.transform);
        button.transform.SetParent(magazineSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);

        magazineImage.sprite = magazine.sprite;
        ammoImage.sprite = magazine.ammoType.sprite;
        magazineBig.sprite = magazine.spriteBig;
        audioSource.PlayOneShot(clickSound);
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
        sightBig.sprite = scope.spriteBig;
        sightBig.color = Color.white; // deze regel weg als er een iron sight sprite is

        if (weaponScript.currentScope.name != "no scope")
        {
            weaponScript.availableScopes.Add(weaponScript.currentScope.name);
            sightSlot.transform.GetChild(1).SetParent(sightInventory.transform);
        }
        SetActiveIfExists(weaponScript.currentScope.model, false);
        weaponScript.currentScope = scope;
        weaponScript.availableScopes.Remove(scope.name);
        SetActiveIfExists(scope.model, true);

        button.transform.SetParent(sightSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);
        audioSource.PlayOneShot(clickSound);
    }

    void UnequipScope(GameObject button)
    {
        weaponScript.availableScopes.Add(weaponScript.currentScope.name);
        SetActiveIfExists(weaponScript.currentScope.model, false);
        weaponScript.currentScope = weaponScript.scopes["no scope"];
        shootscript.zoom = weaponScript.currentScope.zoomFactor;
        sightBig.sprite = weaponScript.currentScope.spriteBig;
        sightBig.color = Color.clear; // deze regel weg als er een iron sight sprite is
        SetActiveIfExists(weaponScript.currentScope.model, true);

        button.transform.SetParent(sightInventory.transform);
        audioSource.PlayOneShot(clickSound);
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
            button.transform.localScale = Vector3.one;
            button.GetComponent<Button>().onClick.AddListener(() => ClickMagazine(magazine, button));
            button.transform.GetChild(0).GetComponent<Image>().sprite = magazine.sprite;
            button.transform.GetChild(1).GetComponent<Image>().sprite = magazine.ammoType.sprite;
            Destroy(button.transform.GetChild(2).gameObject);
            button.GetComponent<AttachmentButtonScript>().magazineAttachment = magazine;
        }
    }

    public void NewScope(ScopeAttachment scope)
    {
        if (!weaponScript.availableScopes.Contains(scope.name) && weaponScript.currentScope.name != scope.name && scope.name != "no scope")
        {
            weaponScript.availableScopes.Add(scope.name);
            GameObject button = Instantiate(attachmentButton);
            button.transform.SetParent(sightInventory.transform);
            button.transform.localScale = Vector3.one;
            button.GetComponent<Button>().onClick.AddListener(() => ClickScope(scope, button));
            if (scope.sprite != null) button.transform.GetChild(2).GetComponent<Image>().sprite = scope.sprite;
            Destroy(button.transform.GetChild(1).gameObject);
            Destroy(button.transform.GetChild(0).gameObject);
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
            selectedMagazineStats.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.ammoType.name[0].ToString().ToUpper() + magazine.ammoType.name[1..];
            selectedMagazineStats.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.ammoType.damage.ToString();
            selectedMagazineStats.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = Mathf.Round(1 / magazine.shotCooldown).ToString();
            selectedMagazineStats.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.capacity.ToString();
            selectedMagazineStats.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = magazine.reloadTime.ToString();
        }
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        movement.canMove = false;
        mouseLook.canLook = false;
        shootscript.canShoot = false;
        menuOpen = true;
    }

    public void NextObjective()
    {
        currentAmountDone = 0;
        currentObjectiveIndex++;
        if (objectiveRequiredAmounts[currentObjectiveIndex] == 1)
        {
            objectiveText.text = objectives[currentObjectiveIndex];
        }
        else
        {
            objectiveText.text = objectives[currentObjectiveIndex] + " (" + currentAmountDone.ToString() + "/" + objectiveRequiredAmounts[currentObjectiveIndex].ToString() + ")";
        }
    }

    public void UpdateObjective(string objective)
    {
        if (objective == objectives[currentObjectiveIndex])
        {
            currentAmountDone++;
            if (currentAmountDone >= objectiveRequiredAmounts[currentObjectiveIndex])
            {
                NextObjective();
            }
            else
            {
                objectiveText.text = objectives[currentObjectiveIndex] + " (" + currentAmountDone.ToString() + "/" + objectiveRequiredAmounts[currentObjectiveIndex].ToString() + ")";
            }
        }
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

    public void ExitToMainMenu()
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("Menu");
    }

    public void SetActiveIfExists(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }
}
