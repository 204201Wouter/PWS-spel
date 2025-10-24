using UnityEngine;
using UnityEngine.EventSystems;

public class AttachmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GuiOpenScript guiOpenScript;
    public MagazineAttachment magazineAttachment;

    public void OnPointerEnter(PointerEventData eventData)
    {
        guiOpenScript.selectedMagazineStats.gameObject.SetActive(true);
        guiOpenScript.ChangeMagazineStats(magazineAttachment, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        guiOpenScript.selectedMagazineStats.gameObject.SetActive(false);
    }
}
