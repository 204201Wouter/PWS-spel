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


    void Start()
    {
        if (MouseLook.testmode == "aimtimedistance" || MouseLook.testmode == "aimtimeangle" || MouseLook.testmode == "aimtimecombined") health = 1;
        if (MouseLook.testmode == "accuracycombined") health = 10000000000000000000;
    }
    public EnemyMovementScript EnemyMovementScript;
    public void Hit(float damage)
    {
        health -= damage;



        if (MouseLook.testmode == "accuracycombined")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{Mathf.RoundToInt((transform.position - player.transform.position).magnitude * 100)},{Mathf.RoundToInt(EnemyMovementScript.LateralAcc.magnitude*100)}, {Mathf.RoundToInt((EnemyMovementScript.lateralVelocity).magnitude * 100)}, {1}");
            string path = Path.Combine(Application.dataPath, "table.csv");
            File.AppendAllText(path, sb.ToString());

        }

        if (health <= 0 && !dead)
        {
            dead = true;
            Instantiate(gun, transform.position, transform.rotation);
            

            StringBuilder sb = new StringBuilder();

            if (MouseLook.testmode == "aimtimedistance") sb.AppendLine($"{Mathf.RoundToInt((transform.position-player.transform.position).magnitude * 100)},{Mathf.RoundToInt((Time.time- MouseLook.StartTime) * 100)}");
            if (MouseLook.testmode == "aimtimeangle") sb.AppendLine($"{Mathf.RoundToInt(MouseLook.StartAngle * 100)},{Mathf.RoundToInt((Time.time - MouseLook.StartTime) * 100)}");
            if (MouseLook.testmode == "aimtimecombined") sb.AppendLine($"{Mathf.RoundToInt((transform.position - player.transform.position).magnitude * 100)},{Mathf.RoundToInt(MouseLook.StartAngle * 100)},{Mathf.RoundToInt((Time.time - MouseLook.StartTime) * 100)}");




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
