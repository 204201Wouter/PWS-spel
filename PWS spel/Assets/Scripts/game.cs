using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public Transform computer;
    public TextMeshProUGUI AmmoText;

    public Transform door1;
    public Transform door2;

    //float progression;
    //string objective;
    public int enemySpawnStage = 1;

    Vector3 startpos;
    Vector3 endpos;

    public Transform enemyParent;
    public GameObject originalEnemy;

    public Transform elevatorRoom1;
    public Transform elevatorRoom2;


    void Start()
    {
        //progression = 0;
        //objective = "computer";

        startpos = door1.position;
        endpos = door1.position - 5f * door1.up;

        StartCoroutine(SpawnEnemies(elevatorRoom1, 10));
    }

    void Update()
    {
        if (enemySpawnStage >= 1)
        {
            if (enemySpawnStage == 1)
            {
                if ((door1.position - startpos).magnitude < 5f)
                {
                    door1.position -= door1.up;
                    door2.position += door2.up;

                }
                else enemySpawnStage = 2;

                //    Debug.Log((transform.position - (startpos + transform.right * 5)).magnitude);
            }

            if (enemySpawnStage == 2)
            {
                foreach (Transform point in elevatorRoom1)
                {
                    GameObject enemy = Instantiate(originalEnemy, point.position, point.rotation, enemyParent);
                    enemy.GetComponent<EnemyScript>().enabled = true;
                    enemy.GetComponent<EnemyMovementScript>().enabled = true;
                }


                enemySpawnStage = 3;
            }
            if (enemySpawnStage >= 3 && enemySpawnStage < 1000)
            {
                enemySpawnStage++;
            }

            if (enemySpawnStage >= 1000)
            {
                if ((door1.position - endpos).magnitude < 5f)
                {
                    door1.position += door1.up;
                    door2.position -= door2.up;

                }
                else enemySpawnStage = 0;
            }
        }
    }

    IEnumerator SpawnEnemies(Transform spawnPositions, int enemyAmount)
    {
        int posIndex = Random.Range(0, 7);
        for (int i = 0; i < enemyAmount; i++)
        {
            Transform point = spawnPositions.GetChild(posIndex);
            GameObject enemy = Instantiate(originalEnemy, point.position, point.rotation, enemyParent);
            enemy.GetComponent<EnemyScript>().enabled = true;
            enemy.GetComponent<EnemyMovementScript>().enabled = true;

            posIndex += 5;
            posIndex %= 8;

            yield return new WaitForSeconds(3);
        }
    }
}
