using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 100;

    public GameObject player;
    public GameObject weapon;
    public GameObject mag;
    EnemyWeaponScript weaponScript;
    EnemyMovementScript movementScript;

    public UnlockableDoorHandler unlockableDoorHandler;

    public Transform droppedWeaponsParent;

    void Start()
    {
        weaponScript = weapon.GetComponent<EnemyWeaponScript>();
        movementScript = GetComponent<EnemyMovementScript>();
    }

    bool dead = false;
    public void Hit(float damage)
    {
        health -= damage;

        if (movementScript.mode != "move") movementScript.mode = "cover";

        if (health <= 0 && !dead)
        {
            dead = true;
            weaponScript.isDropped = true;
            weapon.transform.parent = droppedWeaponsParent;
            mag.SetActive(true);
            weaponScript.yspeed = 0;
            int layer = LayerMask.NameToLayer("Interactable");
            weapon.layer = layer;
            weapon.tag = "enemy weapon";

            unlockableDoorHandler.CheckUnlockDoor1();
      
            Destroy(gameObject);
        }
    }
}
