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

    public void CheckUnlockDoor1()
    {
        enemiesKilled++;
        if (enemiesKilled >= 10) door1.locked = false;
        if (enemiesKilled <= 10) guiScript.UpdateObjective("Kill enemies");
    }

    public void UnlockEscapePods()
    {
        escapeDoor1.locked = false;
        escapeDoor2.locked = false;
        escapeDoor3.locked = false;
    }
}
