using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public float health = 1000;

    public GameObject player;
    public GameObject gun;

    public void Hit(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
            Instantiate(gun, transform.position, transform.rotation);
        }
    }
}
