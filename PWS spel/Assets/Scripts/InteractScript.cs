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

    public GuiScript guiScript;
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
    public bool engine1Disabled;
    public bool engine2Disabled;
    public static bool enginesDisabled = false;

    public GameObject fuelTankInteractPopup;
    public GameObject bombPlantedPopup;
    public bool bombPlanted = false;
    public GameObject bombTimer;
    public int timer = 300;

    public GameObject gravityGeneratorInteractPopup;
    public RectTransform gravityGeneratorLoadingBar;
    public GameObject gravityDisabledPopup;
    public static bool gravityDisabled = false;

    public GameObject pickUpWeaponPopup;

    public GameObject pickUpWeaponsPopup;
    public GameObject openInventoryPopup;
    public AudioSource audioSource;
    public AudioClip interactSound;

    void Start()
    {
        StartCoroutine(TextPopup(openInventoryPopup));
    }

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
                        audioSource.PlayOneShot(interactSound);
                        StartCoroutine(TextPopup(mapDownloadedPopup));
                        unlockableDoorHandler.door2.locked = false;
                        guiScript.minimap.SetActive(true);
                        guiScript.UpdateObjective("Download the map from a computer");
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

                    guiScript.NewScope(enemyWeaponScript.scopeAttachment);
                    guiScript.NewMagazine(enemyWeaponScript.magazineAttachment);

                    if (!weaponScript.availableSilencers.Contains(enemyWeaponScript.silencerAttachment.name)) weaponScript.availableSilencers.Add(enemyWeaponScript.silencerAttachment.name);
                    if (!weaponScript.availableLasers.Contains(enemyWeaponScript.laserAttachment.name)) weaponScript.availableLasers.Add(enemyWeaponScript.laserAttachment.name);

                    weaponScript.ammoAmounts[enemyWeaponScript.magazineAttachment.ammoType.name] += enemyWeaponScript.ammo;

                    Destroy(hit.collider.gameObject);
                    audioSource.PlayOneShot(interactSound);
                }
            }
            else pickUpWeaponPopup.SetActive(false);

            if (hit.collider.gameObject.CompareTag("tool storage box") && !toolObtained && mapDownloaded)
            {
                storageBoxInteractPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    toolObtained = true;
                    audioSource.PlayOneShot(interactSound);
                    toolMarker.SetActive(false);
                    miniToolMarker.SetActive(false);
                    StartCoroutine(TextPopup(toolObtainedPopup));
                    guiScript.UpdateObjective("Open the 2 marked storage boxes");
                }
            }
            else if (hit.collider.gameObject.CompareTag("bomb storage box") && !bombObtained && mapDownloaded)
            {
                storageBoxInteractPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    bombObtained = true;
                    audioSource.PlayOneShot(interactSound);
                    bombMarker.SetActive(false);
                    miniBombMarker.SetActive(false);
                    StartCoroutine(TextPopup(bombObtainedPopup));
                    guiScript.UpdateObjective("Open the 2 marked storage boxes");
                }
            }
            else storageBoxInteractPopup.SetActive(false);


            if (hit.collider.gameObject.CompareTag("engine") && !enginesDisabled && toolObtained)
            {
                bool engine1 = hit.collider.transform.parent.gameObject.name == "engine 1";
                if ((engine1 && !engine1Disabled) || (!engine1 && !engine2Disabled))
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        engineInteractLength += Time.fixedDeltaTime;
                        if (engineInteractLength >= requiredEngineInteractLength)
                        {
                            if (engine1) engine1Disabled = true;
                            else engine2Disabled = true;
                            audioSource.PlayOneShot(interactSound);
                            enginesDisabled = engine1Disabled && engine2Disabled;
                            guiScript.UpdateObjective("Disable the engines");

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
                    audioSource.PlayOneShot(interactSound);
                    bombTimer.SetActive(true);
                    StartCoroutine(TextPopup(bombPlantedPopup));
                    StartCoroutine(BombTimer());
                    guiScript.UpdateObjective("Plant the bomb at the fuel tanks");

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
                        audioSource.PlayOneShot(interactSound);
                        StartCoroutine(TextPopup(gravityDisabledPopup));
                        guiScript.UpdateObjective("Disable the gravity generator");

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

    public IEnumerator TextPopup(GameObject popup)
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
