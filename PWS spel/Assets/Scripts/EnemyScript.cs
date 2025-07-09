using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using static UnityEngine.Rendering.DebugUI;
using System;

public class EnemyScript : MonoBehaviour
{
    public float health = 1000;

    public GameObject player;
    public GameObject gun;

    public MouseLook MouseLook;

    bool dead = false;
    public void Hit(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;
            Instantiate(gun, transform.position, transform.rotation);
            

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Time,Angle");


            foreach (int time in MouseLook.table.Keys)
            {
                sb.AppendLine($"{time},{MouseLook.table[time]}");
            }
 

            string path = Path.Combine(Application.dataPath, "table.csv");
            File.WriteAllText(path, sb.ToString());


            Destroy(gameObject);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
