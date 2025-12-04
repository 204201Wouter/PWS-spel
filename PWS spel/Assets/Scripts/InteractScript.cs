using UnityEngine;
using System.Collections;
using TMPro;


public class InteractScript : MonoBehaviour
{
    public UnlockableDoorHandler unlockableDoorHandler;

    readonly float requiredComputerInteractLength = 5f;
    float computerInteractLength = 0f;

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

    public GameObject gravityGeneratorInteractPopup;
    public RectTransform gravityGeneratorLoadingBar;
    public GameObject gravityDisabledPopup;
    public bool gravityDisabled = false;

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


            if (hit.collider.gameObject.CompareTag("storage box") && !toolObtained && mapDownloaded)
            {
                storageBoxInteractPopup.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    toolObtained = true;
                    StartCoroutine(TextPopup(toolObtainedPopup));
                }
            }
            else storageBoxInteractPopup.SetActive(false);


            if (hit.collider.gameObject.CompareTag("gravity generator") && !gravityDisabled && toolObtained)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    gravityGeneratorInteractLength += Time.fixedDeltaTime;
                    if (gravityGeneratorInteractLength >= requiredGravityGeneratorInteractLength)
                    {
                        gravityDisabled = true;
                        GetComponentInParent<Movement>().velocity = Vector3.zero;
                        StartCoroutine(TextPopup(gravityDisabledPopup));
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
            gravityGeneratorInteractPopup.SetActive(false);
            gravityGeneratorInteractLength = 0;
        }
    }

    IEnumerator TextPopup(GameObject popup)
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(2f);
        for (float a = 1; a >= 0; a -= 0.01f)
        {
            popup.GetComponent<TextMeshProUGUI>().color = new(1, 1, 1, a);
            yield return null;
        }
        popup.SetActive(false);
    }
}
