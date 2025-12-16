using UnityEngine;
using UnityEngine.EventSystems;

public class AttachmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GuiOpenScript guiOpenScript;
    public MagazineAttachment magazineAttachment;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!magazineAttachment.Equals(default(MagazineAttachment)))
        {
            guiOpenScript.selectedMagazineStats.gameObject.SetActive(true);
            guiOpenScript.ChangeMagazineStats(magazineAttachment, false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!magazineAttachment.Equals(default(MagazineAttachment)))
        {
            guiOpenScript.selectedMagazineStats.gameObject.SetActive(false);
        }
    }
}
