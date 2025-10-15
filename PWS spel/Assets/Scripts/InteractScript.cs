using UnityEngine;
using System.Collections;
using TMPro;


public class InteractScript : MonoBehaviour
{
    readonly float requiredComputerInteractLength = 5f;
    float computerInteractLength = 0f;

    readonly float reach = 5f;

    public LayerMask layerMask;

    public GameObject computerInteractPopup;
    public RectTransform computerLoadingBar;
    public GameObject mapDownloadedPopup;
    bool mapDownloaded = false;
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, reach, layerMask))
        {
            computerLoadingBar.sizeDelta = new(Mathf.Clamp(computerInteractLength / requiredComputerInteractLength * 95, 1, 95), 25);
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

            if (hit.collider.gameObject.CompareTag("computer") && !mapDownloaded) computerInteractPopup.SetActive(true);
            else computerInteractPopup.SetActive(false);
        }
        else computerInteractPopup.SetActive(false);
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
