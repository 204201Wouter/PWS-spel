using UnityEngine;
using System.Collections;
using TMPro;


public class InteractScript : MonoBehaviour
{
    public UnlockableDoorHandler unlockableDoorHandler;
    public EnemySpawnScript enemySpawnScript;
    public Transform spawnLocationsGravDisabled;

    readonly float requiredComputerInteractLength = 5f;
    float computerInteractLength = 0f;

    readonly float requiredEngineInteractLength = 5f;
    float engineInteractLength = 0f;

    readonly float requiredGravityGeneratorInteractLength = 5f;
    float gravityGeneratorInteractLength = 0f;

    readonly float reach = 5f;

    public LayerMask layerMask;

    public GuiOpenScript guiOpenScript;
    public WeaponScript weaponScript;

    public GameObject computerInteractPopup;
    public RectTransform computerLoadingBar;
    public GameObject mapDownloadedPopup;
    public bool mapDownloaded = false;

    public GameObject storageBoxInteractPopup;
    public GameObject toolObtainedPopup;
    public bool toolObtained = false;
    public GameObject bombObtainedPopup;
    public bool bombObtained = false;
    public GameObject toolMarker;
    public GameObject bombMarker;
    public GameObject miniToolMarker;
    public GameObject miniBombMarker;

    public GameObject engineInteractPopup;
    public RectTransform engineLoadingBar;
    public GameObject enginesDisabledPopup;
    bool engine1disabled;
    bool engine2disabled;
    public static bool enginesDisabled = false;

    public GameObject fuelTankInteractPopup;
    public GameObject bombPlantedPopup;
    public bool bombPlanted = false;
    public GameObject bombTimer;

    public GameObject gravityGeneratorInteractPopup;
    public RectTransform gravityGeneratorLoadingBar;
    public GameObject gravityDisabledPopup;
    public static bool gravityDisabled = false;

    public GameObject pickUpWeaponPopup;
    
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, reach, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("computer") && !mapDownloaded)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    computerInteractLength += Time.fixedDeltaTime;
                    if (computerInteractLength >= requiredComputerInteractLength)
                    {
                        mapDownloaded = true;
                        StartCoroutine(TextPopup(mapDownloadedPopup));
                        unlockableDoorHandler.door2.locked = false;
                        guiOpenScript.minimap.SetActive(true);
                        guiOpenScript.UpdateObjective("Download the map from a computer");
                    }
                }
                else computerInteractLength = 0;

                computerLoadingBar.sizeDelta = new(Mathf.Clamp(computerInteractLength / requiredComputerInteractLength * 95, 1, 95), 25);
                computerInteractPopup.SetActive(true);
            }
            else
            {
                computerInteractLength = 0;
                computerInteractPopup.SetActive(false);
            }


            if (hit.collider.gameObject.CompareTag("enemy weapon"))
            {
                pickUpWeaponPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    EnemyWeaponScript enemyWeaponScript = hit.collider.GetComponent<EnemyWeaponScript>();

                    guiOpenScript.NewScope(enemyWeaponScript.scopeAttachment);
                    guiOpenScript.NewMagazine(enemyWeaponScript.magazineAttachment);

                    if (!weaponScript.availableSilencers.Contains(enemyWeaponScript.silencerAttachment.name)) weaponScript.availableSilencers.Add(enemyWeaponScript.silencerAttachment.name);
                    if (!weaponScript.availableLasers.Contains(enemyWeaponScript.laserAttachment.name)) weaponScript.availableLasers.Add(enemyWeaponScript.laserAttachment.name);

                    weaponScript.ammoAmounts[enemyWeaponScript.magazineAttachment.ammoType.name] += enemyWeaponScript.ammo;

                    Destroy(hit.collider.gameObject);
                }
            }
            else pickUpWeaponPopup.SetActive(false);

            if (hit.collider.gameObject.CompareTag("tool storage box") && !toolObtained && mapDownloaded)
            {
                storageBoxInteractPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    toolObtained = true;
                    toolMarker.SetActive(false);
                    miniToolMarker.SetActive(false);
                    StartCoroutine(TextPopup(toolObtainedPopup));
                    guiOpenScript.UpdateObjective("Open the 2 marked storage boxes");
                }
            }
            else if (hit.collider.gameObject.CompareTag("bomb storage box") && !bombObtained && mapDownloaded)
            {
                storageBoxInteractPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    bombObtained = true;
                    bombMarker.SetActive(false);
                    miniBombMarker.SetActive(false);
                    StartCoroutine(TextPopup(bombObtainedPopup));
                    guiOpenScript.UpdateObjective("Open the 2 marked storage boxes");
                }
            }
            else storageBoxInteractPopup.SetActive(false);


            if (hit.collider.gameObject.CompareTag("engine") && !enginesDisabled && toolObtained)
            {
                bool engine1 = hit.collider.transform.parent.gameObject.name == "engine 1";
                if ((engine1 && !engine1disabled) || (!engine1 && !engine2disabled))
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        engineInteractLength += Time.fixedDeltaTime;
                        if (engineInteractLength >= requiredEngineInteractLength)
                        {
                            if (engine1) engine1disabled = true;
                            else engine2disabled = true;
                            enginesDisabled = engine1disabled && engine2disabled;
                            guiOpenScript.UpdateObjective("Disable the engines");

                            if (enginesDisabled) StartCoroutine(TextPopup(enginesDisabledPopup));
                        }
                    }
                    else engineInteractLength = 0;

                    engineLoadingBar.sizeDelta = new(Mathf.Clamp(engineInteractLength / requiredEngineInteractLength * 95, 1, 95), 25);
                    engineInteractPopup.SetActive(true);
                }
                else
                {
                    engineInteractLength = 0;
                    engineInteractPopup.SetActive(false);
                }
            }
            else
            {
                engineInteractPopup.SetActive(false);
                engineInteractLength = 0;
            }


            if (hit.collider.gameObject.CompareTag("fuel tank") && bombObtained && enginesDisabled && !bombPlanted)
            {
                fuelTankInteractPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    bombPlanted = true;
                    bombTimer.SetActive(true);
                    StartCoroutine(TextPopup(bombPlantedPopup));
                    StartCoroutine(BombTimer());
                    guiOpenScript.UpdateObjective("Plant the bomb at the fuel tanks");

                    if (gravityDisabled) unlockableDoorHandler.UnlockEscapePods();
                }
            }
            else fuelTankInteractPopup.SetActive(false);


            if (hit.collider.gameObject.CompareTag("gravity generator") && !gravityDisabled && toolObtained)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    gravityGeneratorInteractLength += Time.fixedDeltaTime;
                    if (gravityGeneratorInteractLength >= requiredGravityGeneratorInteractLength)
                    {
                        gravityDisabled = true;
                        StartCoroutine(TextPopup(gravityDisabledPopup));
                        guiOpenScript.UpdateObjective("Disable the gravity generator");

                        if (bombPlanted) unlockableDoorHandler.UnlockEscapePods();
                    }
                }
                else gravityGeneratorInteractLength = 0;

                gravityGeneratorLoadingBar.sizeDelta = new(Mathf.Clamp(gravityGeneratorInteractLength / requiredGravityGeneratorInteractLength * 95, 1, 95), 25);
                gravityGeneratorInteractPopup.SetActive(true);
            }
            else
            {
                gravityGeneratorInteractLength = 0;
                gravityGeneratorInteractPopup.SetActive(false);
            }
        }
        else
        {
            computerInteractLength = 0;
            computerInteractPopup.SetActive(false);
            pickUpWeaponPopup.SetActive(false);
            storageBoxInteractPopup.SetActive(false);
            engineInteractLength = 0;
            engineInteractPopup.SetActive(false);
            fuelTankInteractPopup.SetActive(false);
            gravityGeneratorInteractPopup.SetActive(false);
            gravityGeneratorInteractLength = 0;
        }
    }

    IEnumerator TextPopup(GameObject popup)
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(4f);
        for (float a = 1; a >= 0; a -= 0.01f)
        {
            popup.GetComponent<TextMeshProUGUI>().color = new(1, 1, 1, a);
            yield return null;
        }
        popup.SetActive(false);
    }

    IEnumerator BombTimer()
    {
        int timer = 300;

        while (timer >= 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = timer % 60;
            string secondsStr;
            if (seconds < 10) secondsStr = "0" + seconds.ToString();
            else secondsStr = seconds.ToString();
            bombTimer.GetComponent<TextMeshProUGUI>().text = "Detonation in " + minutes.ToString() + ":" + secondsStr;

            yield return new WaitForSeconds(1);

            timer--;
        }
    }
 }
