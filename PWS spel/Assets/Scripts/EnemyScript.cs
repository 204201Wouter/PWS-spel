using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public float health = 200;

    public GameObject player;
    public GameObject weapon;
    EnemyWeaponScript weaponScript;

    public Transform droppedWeaponsParent;

    void Start()
    {
     //   weapon = transform.GetChild(2).gameObject;
        weaponScript = weapon.GetComponent<EnemyWeaponScript>();
    }

    bool dead = false;
    public void Hit(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;
            weaponScript.isDropped = true;
            weapon.transform.parent = droppedWeaponsParent;
            weapon.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
