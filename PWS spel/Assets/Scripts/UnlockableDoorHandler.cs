using UnityEngine;

public class UnlockableDoorHandler : MonoBehaviour
{
    public OpenDoorScript door1;
    public OpenDoorScript door2;
    public OpenDoorScript escapeDoor1;
    public OpenDoorScript escapeDoor2;
    public OpenDoorScript escapeDoor3;

    public int enemiesKilled = 0;
    public GuiScript guiScript;
    public InteractScript interactScript;

    public void CheckUnlockDoor1()
    {
        // unlock door 1 als alle enemies in de eerste liftroom dood zijn
        enemiesKilled++;
        if (enemiesKilled >= 10) door1.locked = false;
        if (enemiesKilled <= 10) guiScript.UpdateObjective("Kill enemies");
        if (enemiesKilled == 1) StartCoroutine(interactScript.TextPopup(interactScript.pickUpWeaponsPopup));
    }

    public void UnlockEscapePods()
    {
        escapeDoor1.locked = false;
        escapeDoor2.locked = false;
        escapeDoor3.locked = false;
    }
}
