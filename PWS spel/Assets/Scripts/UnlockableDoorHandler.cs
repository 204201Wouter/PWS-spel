using UnityEngine;

public class UnlockableDoorHandler : MonoBehaviour
{
    public OpenDoorScript door1;
    public OpenDoorScript door2;

    public int enemiesKilled = 0;

    public void CheckUnlockDoor1()
    {
        if (enemiesKilled >= 10) door1.locked = false;
    }
}
