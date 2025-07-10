using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public float health = 200;

    public GameObject player;
    public GameObject gun;

    bool dead = false;
    public void Hit(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;
            Instantiate(gun, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
