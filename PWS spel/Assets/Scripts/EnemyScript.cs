using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public int health = 1000;

    public GameObject player;

    public void Hit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
