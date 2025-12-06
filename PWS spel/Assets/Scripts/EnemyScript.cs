using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyScript : MonoBehaviour
{
    public float health = 200;

    public GameObject player;
    public GameObject weapon;
    public GameObject mag;
    EnemyWeaponScript weaponScript;

    public UnlockableDoorHandler unlockableDoorHandler;

    public Transform droppedWeaponsParent;

    void Start()
    {
        weaponScript = weapon.GetComponent<EnemyWeaponScript>();
    }

    bool dead = false;
    public void Hit(float damage)
    {
        health -= damage;

        GetComponent<EnemyMovementScript>().mode = "cover";

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

            unlockableDoorHandler.enemiesKilled++;
            unlockableDoorHandler.CheckUnlockDoor1();
      
            Destroy(gameObject);
        }
    }
}
