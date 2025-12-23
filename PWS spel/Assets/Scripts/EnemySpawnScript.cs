using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnScript : MonoBehaviour
{
    public Transform enemyParent;
    public GameObject originalEnemy;

    public IEnumerator SpawnEnemies(Transform spawnPositions, int enemyAmount)
    {
        int posIndex = Random.Range(0, 7);
        Transform doors1 = spawnPositions.parent.Find("door1");
        Transform doors2 = spawnPositions.parent.Find("door2");
        Transform boxes = spawnPositions.parent.Find("Lifts");
        Transform nodes = spawnPositions.parent.Find("nodes");
        Transform cover = spawnPositions.parent.Find("cover");
        for (int i = 0; i < enemyAmount; i++)
        {
            Transform point = spawnPositions.GetChild(posIndex);
            GameObject enemy = Instantiate(originalEnemy, point.position, point.rotation, enemyParent);

            enemy.GetComponent<EnemyScript>().enabled = true;
            enemy.GetComponent<EnemyMovementScript>().enabled = true;
            enemy.GetComponent<EnemyMovementScript>().mode = "move";
            enemy.GetComponent<EnemyMovementScript>().lift = boxes.GetChild(posIndex).GetChild(0).GetComponent<BoxCollider>();
            enemy.GetComponent<EnemyMovementScript>().nodes = nodes;
            enemy.GetComponent<EnemyMovementScript>().cover = cover;

            if (point.localPosition.x < 0) enemy.GetComponent<EnemyMovementScript>().targetPos = point.position + 5 * point.right;
            else enemy.GetComponent<EnemyMovementScript>().targetPos = point.position - 5 * point.right;

            Transform door1 = doors1.GetChild(posIndex);
            Transform door2 = doors2.GetChild(posIndex);

            Vector3 startpos1 = door1.position;
            Vector3 startpos2 = door2.position;
            Vector3 endpos = door1.position - 5f * door1.up;

            while ((door1.position - startpos1).magnitude < 5f)
            {
                door1.position -= 0.5f * door1.up;
                door2.position += 0.5f * door2.up;
                yield return null;
            }

            posIndex += 5;
            posIndex %= 8;

            yield return new WaitForSeconds(3);

            while ((door1.position - endpos).magnitude < 5f)
            {
                door1.position += 0.5f * door1.up;
                door2.position -= 0.5f * door2.up;
                yield return null;
            }
            door1.position = startpos1;
            door2.position = startpos2;
        }
    }
}
