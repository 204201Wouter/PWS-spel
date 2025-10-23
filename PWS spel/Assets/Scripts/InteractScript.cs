using UnityEngine;
using System.Collections;
using TMPro;


public class InteractScript : MonoBehaviour
{
    readonly float requiredComputerInteractLength = 5f;
    float computerInteractLength = 0f;

    readonly float reach = 5f;

    public LayerMask layerMask;

    public GuiOpenScript guiOpenScript;
    public WeaponScript weaponScript;

    public GameObject computerInteractPopup;
    public RectTransform computerLoadingBar;
    public GameObject mapDownloadedPopup;
    bool mapDownloaded = false;

    public GameObject pickUpWeaponPopup;
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, reach, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("computer") && Input.GetKey(KeyCode.F) && !mapDownloaded)
            {
                computerInteractLength += Time.fixedDeltaTime;
                if (computerInteractLength >= requiredComputerInteractLength)
                {
                    mapDownloaded = true;
                    StartCoroutine(MapDownloadedPopup());
                }
            }
            else computerInteractLength = 0;

            if (hit.collider.gameObject.CompareTag("computer") && !mapDownloaded)
            {
                computerLoadingBar.sizeDelta = new(Mathf.Clamp(computerInteractLength / requiredComputerInteractLength * 95, 1, 95), 25);
                computerInteractPopup.SetActive(true);
            }
            else computerInteractPopup.SetActive(false);


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
        }
        else
        {
            computerInteractPopup.SetActive(false);
            pickUpWeaponPopup.SetActive(false);
        }
    }

    IEnumerator MapDownloadedPopup()
    {
        mapDownloadedPopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        for (float a = 1; a >= 0; a -= 0.01f)
        {
            mapDownloadedPopup.GetComponent<TextMeshProUGUI>().color = new(1, 1, 1, a);
            yield return null;
        }
        mapDownloadedPopup.SetActive(false);
    }
}
