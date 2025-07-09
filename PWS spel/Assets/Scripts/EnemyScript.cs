using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour
{
    public float health = 1000;

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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
