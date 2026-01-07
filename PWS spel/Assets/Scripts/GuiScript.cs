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
    public int currentObjectiveIndex = 0;
    readonly List<string> objectives = new();
    readonly List<int> objectiveRequiredAmounts = new();
    public int currentAmountDone;

    bool menuOpen = false;

    public Transform enemyParent;
    public UnlockableDoorHandler unlockableDoorHandler;
    public Transform enableEnemiesTriggers;

    public GameObject originalEnemy;
    public Transform liftroom1;
    public Transform liftroom2;
    public Transform liftroom3;
    public Transform allNodes;
    public Transform allCover;

    int setPlayerPos = 0; // dit is voor rare bug met de build die opeens positie van speler reset die ik niet snap na 2 uur zoeken
    Vector3 playerPosToSet;

    void Start()
    {
        // sluit de inventaris
        gui.SetActive(false);
        sightInventory.SetActive(false);
        magazineInventory.SetActive(true);
        sightSlot.onClick.AddListener(() => OpenInventory(sightInventory));
        magazineSlot.onClick.AddListener(() => OpenInventory(magazineInventory));

        activeInventory = magazineInventory;

        if (MainMenuScript.newGame)
        {
            shootscript.UpdateAmmoText();

            magazineSlot.transform.GetChild(1).GetComponent<AttachmentButtonScript>().magazineAttachment = weaponScript.currentMagazine;
        }

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

        if (!MainMenuScript.newGame) LoadGame();
    }

    void Update()
    {
        // open inventory als je op T drukt
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

        // open settings als je op escape drukt
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

        // open map als je op M drukt en de map gedownload hebt
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

        // update de marker op de map of de minimap
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
        if (setPlayerPos > 0)
        {
            transform.position = playerPosToSet;
            setPlayerPos--;
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
        // zet alle waarden van het nieuwe magazine goed
        shootscript.shotCooldown = magazine.shotCooldown;
        shootscript.cap = magazine.capacity;
        shootscript.reloadTime = magazine.reloadTime;
        shootscript.damage = magazine.ammoType.damage;
        shootscript.projectilesPerShot = magazine.ammoType.amount;
        shootscript.spread = magazine.ammoType.spread;
        shootscript.size = magazine.ammoType.size;
        weaponScript.ammoAmounts[shootscript.ammoType] += shootscript.ammo;
        shootscript.ammoType = magazine.ammoType.name;
        shootscript.recoil = magazine.ammoType.recoil;
        shootscript.ammo = 0;
        shootscript.UpdateAmmoText();

        ChangeMagazineStats(magazine, true);

        // zet het goede model aan
        SetActiveIfExists(weaponScript.currentMagazine.model, false);
        weaponScript.availableMagazines.Add(weaponScript.currentMagazine.name);
        weaponScript.currentMagazine = magazine;
        weaponScript.availableMagazines.Remove(magazine.name);
        SetActiveIfExists(magazine.model, true);

        // verplaats het item van het magazine in de inventory
        if (magazineSlot.transform.childCount > 1) magazineSlot.transform.GetChild(1).SetParent(magazineInventory.transform);
        button.transform.SetParent(magazineSlot.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);

        // zet de sprites in de inventory en HUD goed
        magazineImage.sprite = magazine.sprite;
        ammoImage.sprite = magazine.ammoType.sprite;
        magazineBig.sprite = magazine.spriteBig;
        audioSource.PlayOneShot(clickSound);
    }

    public void ClickMagazine(MagazineAttachment magazine, GameObject button)
    {
        // logica voor als je op een magazine item klikt
        if (activeInventory == magazineInventory)
        {
            if (magazine.name != weaponScript.currentMagazine.name && !movement.reloading) EquipMagazine(magazine, button);
        }
        else OpenInventory(magazineInventory);
    }

    void EquipScope(ScopeAttachment scope, GameObject button)
    {
        shootscript.zoom = scope.zoomFactor;
        sightBig.sprite = scope.spriteBig;
        sightBig.color = Color.white; // deze regel weg als er een iron sight sprite is

        if (weaponScript.currentScope.name != "no scope" && sightSlot.transform.childCount > 1)
        {
            // als je een andere scope geequipped had, haal die dan weg
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

    void UnequipScope(GameObject button) // deze functie is er niet voor magazines omdat je altijd een magazine moet hebben
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
        // logica voor als je op een scope item klikt
        if (activeInventory == sightInventory)
        {
            if (scope.name != weaponScript.currentScope.name) EquipScope(scope, button);
            else UnequipScope(button);
        }
        else OpenInventory(sightInventory);
    }
    public GameObject NewMagazine(MagazineAttachment magazine)
    {
        // als je deze magazine nog niet hebt, maak dan de item ervoor aan in de inventory
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
            return button;
        }
        return null;
    }

    public GameObject NewScope(ScopeAttachment scope)
    {
        // als je deze scope nog niet hebt, maak dan de item ervoor aan in de inventory
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
            return button;
        }
        return null;
    }

    public void ChangeMagazineStats(MagazineAttachment magazine, bool currentMagazine)
    {
        // laat de stats van de magazine waar je overheen hovert zien, en die van de magazine die geequipped is
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
        // laat het volgende objective zien
        currentAmountDone = 0;
        currentObjectiveIndex++;
        if (objectiveRequiredAmounts[currentObjectiveIndex] == 1)
        {
            objectiveText.text = objectives[currentObjectiveIndex];
        }
        else
        {
            objectiveText.text = objectives[currentObjectiveIndex] + " (0/" + objectiveRequiredAmounts[currentObjectiveIndex].ToString() + ")";
        }
    }

    public void UpdateObjective(string objective)
    {
        // als dit objective degene is die nu bezig is, verhoog de voortgang met 1
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

        if (GetComponent<PlayerHealth>().health > 0) // save alleen als je niet dood bent
        {
            SaveData data = new(GetComponent<PlayerHealth>(), enemyParent, weaponScript, interactScript, unlockableDoorHandler, enableEnemiesTriggers, this);
            SaveScript.Save(data);
        }

        SceneManager.LoadScene("Menu");
    }

    public void LoadGame()
    {
        // haal de data op uit het bestand
        SaveData data = SaveScript.Load();
        if (data == null)
        {
            print("no save data found");
            SceneManager.LoadScene("Menu");
            return;
        }

        // zet alle waarden uit het save bestand in het spel
        GetComponent<PlayerHealth>().health = data.playerHealth;
        transform.position = new(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        playerPosToSet = new(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        setPlayerPos = 2;

        for (int i = 0; i < data.enemyHealths.Length; i++)
        {
            // bepaal in welke kamer de enemy zit en neem de goede waarde voor die kamer
            Transform nodes;
            Transform cover;
            switch (data.enemyRooms[i])
            {
                case "Computer Room": 
                    nodes = allNodes.GetChild(0);
                    cover = allCover.GetChild(0);
                    break;
                case "Storage Room":
                    nodes = allNodes.GetChild(1);
                    cover = allCover.GetChild(1);
                    break;
                case "Office Room":
                    nodes = allNodes.GetChild(2);
                    cover = allCover.GetChild(2);
                    break;
                case "Gravity Room":
                    nodes = allNodes.GetChild(3);
                    cover = allCover.GetChild(3);
                    break;
                case "Engine Room":
                    nodes = allNodes.GetChild(4);
                    cover = allCover.GetChild(4);
                    break;
                case "Fuel Room":
                    nodes = allNodes.GetChild(5);
                    cover = allCover.GetChild(5);
                    break;
                case "Escape Pod Room":
                    nodes = allNodes.GetChild(6);
                    cover = allCover.GetChild(6);
                    break;
                case "liftroom (1)":
                    nodes = liftroom1.Find("nodes");
                    cover = liftroom1.Find("cover");
                    break;
                case "liftroom (2)":
                    nodes = liftroom2.Find("nodes");
                    cover = liftroom2.Find("cover");
                    break;
                case "liftroom (3)":
                    nodes = liftroom3.Find("nodes");
                    cover = liftroom3.Find("cover");
                    break;
                default:
                    print(data.enemyRooms[i]);
                    print(i);
                    nodes = null;
                    cover = null;
                    break;
            }

            // maak de enemy
            GameObject enemy = Instantiate(originalEnemy, new(data.enemyPositions[i][0], data.enemyPositions[i][1], data.enemyPositions[i][2]), Quaternion.identity, enemyParent);
            enemy.GetComponent<EnemyMovementScript>().enabled = true;
            enemy.GetComponent<EnemyScript>().enabled = true;

            if (data.enemyModes[i] != "move") enemy.GetComponent<EnemyMovementScript>().mode = data.enemyModes[i];
            else enemy.GetComponent<EnemyMovementScript>().mode = "guard";
            enemy.GetComponent<EnemyMovementScript>().nodes = nodes;
            enemy.GetComponent<EnemyMovementScript>().cover = cover;
            enemy.GetComponent<EnemyMovementScript>().room = nodes.gameObject.name;
            enemy.GetComponent<EnemyScript>().health = data.enemyHealths[i];
            enemy.GetComponentInChildren<EnemyWeaponScript>().ammo = data.enemyAmmos[i];
            enemy.GetComponentInChildren<EnemyWeaponScript>().magazineAttachment = weaponScript.magazines[data.enemyMagazines[i]];
            enemy.GetComponentInChildren<EnemyWeaponScript>().scopeAttachment = weaponScript.scopes[data.enemyScopes[i]];
            enemy.GetComponentInChildren<EnemyWeaponScript>().InitializeValues();
        }

        shootscript.ammo = data.loadedAmmo;
        weaponScript.ammoAmounts["normal"] = data.ammoAmounts[0];
        weaponScript.ammoAmounts["small"] = data.ammoAmounts[1];
        weaponScript.ammoAmounts["big"] = data.ammoAmounts[2];
        weaponScript.ammoAmounts["buckshot"] = data.ammoAmounts[3];
        weaponScript.ammoAmounts["birdshot"] = data.ammoAmounts[4];

        // maak de items voor de attachments aan
        GameObject button = NewMagazine(weaponScript.magazines[data.currentMagazine]);
        EquipMagazine(weaponScript.magazines[data.currentMagazine], button);

        // maak alleen knop aan voor de scope als het niet de no scope is
        if (data.currentScope != "no scope")
        {
            button = NewScope(weaponScript.scopes[data.currentScope]);
            EquipScope(weaponScript.scopes[data.currentScope], button);
        }
        else
        {
            weaponScript.currentScope = weaponScript.scopes["no scope"];
        }

        foreach (string magazine in data.availableMagazines)
        {
            weaponScript.availableMagazines.Add(magazine);
            NewMagazine(weaponScript.magazines[magazine]);
        }

        foreach (string scope in data.availableScopes)
        {
            weaponScript.availableScopes.Add(scope);
            NewScope(weaponScript.scopes[scope]);
        }

        interactScript.mapDownloaded = data.mapObtained;
        interactScript.toolObtained = data.toolObtained;
        interactScript.bombObtained = data.bombObtained;
        interactScript.engine1Disabled = data.engine1Disabled;
        interactScript.engine2Disabled = data.engine2Disabled;
        InteractScript.enginesDisabled = data.enginesDisabled;
        interactScript.bombPlanted = data.bombPlanted;
        interactScript.timer = data.bombTimer;
        if (data.bombPlanted) StartCoroutine(interactScript.BombTimer());

        InteractScript.gravityDisabled = data.gravityDisabled;

        unlockableDoorHandler.enemiesKilled = data.enemiesKilled;
        unlockableDoorHandler.door1.locked = data.door1locked;
        unlockableDoorHandler.door2.locked = data.door2locked;

        enableEnemiesTriggers.GetChild(0).GetComponent<BoxCollider>().enabled = data.computerRoomEnabled;
        enableEnemiesTriggers.GetChild(1).GetComponent<BoxCollider>().enabled = data.storageRoomEnabled;
        enableEnemiesTriggers.GetChild(2).GetComponent<BoxCollider>().enabled = data.engineRoomEnabled;
        enableEnemiesTriggers.GetChild(3).GetComponent<BoxCollider>().enabled = data.officeRoomEnabled;
        enableEnemiesTriggers.GetChild(4).GetComponent<BoxCollider>().enabled = data.gravityRoomEnabled;
        enableEnemiesTriggers.GetChild(5).GetComponent<BoxCollider>().enabled = data.escapePodsEnabled;

        enableEnemiesTriggers.GetChild(6).GetComponent<BoxCollider>().enabled = data.liftRoom1Enabled;
        enableEnemiesTriggers.GetChild(7).GetComponent<BoxCollider>().enabled = data.liftRoom2aEnabled;
        enableEnemiesTriggers.GetChild(8).GetComponent<BoxCollider>().enabled = data.liftRoom2bEnabled;
        enableEnemiesTriggers.GetChild(9).GetComponent<BoxCollider>().enabled = data.liftRoom3aEnabled;
        enableEnemiesTriggers.GetChild(10).GetComponent<BoxCollider>().enabled = data.liftRoom3bEnabled;

        currentObjectiveIndex = data.currentObjectiveIndex;
        currentAmountDone = data.currentAmountDone - 1;
        UpdateObjective(objectives[currentObjectiveIndex]);

        AudioListener.volume = data.volume;
        PlayerHealth.difficulty = data.difficulty;
        volumeSlider.value = AudioListener.volume;
        difficultySlider.value = PlayerHealth.difficulty;
        ChangeDifficulty();
    }

    public void SetActiveIfExists(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }
}
