using UnityEngine;
using UnityEngine.EventSystems;

public class AttachmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GuiScript guiScript;
    public MagazineAttachment magazineAttachment;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!magazineAttachment.Equals(default(MagazineAttachment)))
        {
            guiScript.selectedMagazineStats.gameObject.SetActive(true);
            guiScript.ChangeMagazineStats(magazineAttachment, false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!magazineAttachment.Equals(default(MagazineAttachment)))
        {
            guiScript.selectedMagazineStats.gameObject.SetActive(false);
        }
    }
}
