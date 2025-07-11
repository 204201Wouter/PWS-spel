using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using static UnityEngine.Rendering.DebugUI;
using System;
using static Unity.Burst.Intrinsics.X86.Avx;

public class EnemyScript : MonoBehaviour
{
    public float health = 1000;

    public GameObject player;
    public GameObject gun;


    public MouseLook MouseLook;
    public ShootProjectile ShootProjectile;

    bool dead = false;

    

    public EnemyMovementScript EnemyMovementScript;
    public void Hit(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;
            Instantiate(gun, transform.position, transform.rotation);
            

            StringBuilder sb = new StringBuilder();

            // sb.AppendLine($"{Mathf.RoundToInt((transform.position-player.transform.position).magnitude * 100)},{Mathf.RoundToInt((Time.time- MouseLook.StartTime) * 100)}");
            //  sb.AppendLine($"{Mathf.RoundToInt(MouseLook.StartAngle * 100)},{Mathf.RoundToInt((Time.time - MouseLook.StartTime) * 100)}");
            sb.AppendLine($"{Mathf.RoundToInt((transform.position - player.transform.position).magnitude * 100)},{Mathf.RoundToInt((5f / (100 - ShootProjectile.ammo)) * 100)}");


       
            /*
            int i = 0;
            foreach (int time in MouseLook.table.Keys)
            {
                if (i < EnemyMovementScript.turns.Count)
                {
                    sb.AppendLine($"{time},{MouseLook.table[time]},{EnemyMovementScript.turns[i]}");

                }
                else { sb.AppendLine($"{time},{MouseLook.table[time]}"); }
                i += 1;
            }
            */


            string path = Path.Combine(Application.dataPath, "table.csv");
            File.AppendAllText(path, sb.ToString());


            Destroy(gameObject);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
