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
            mag.SetActive(true);
            weaponScript.yspeed = 0;
            int layer = LayerMask.NameToLayer("droppedweapon");
            weapon.layer = layer;

            foreach (Transform child in weapon.transform)
            {
                child.gameObject.layer = layer;
            }
      
            //    weapon.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
