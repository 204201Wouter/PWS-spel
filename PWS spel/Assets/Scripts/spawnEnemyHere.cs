using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class spawnEnemyHere : MonoBehaviour
{
    public GameObject originalEnemy;
    public Transform nodes;
    public Transform cover;
    void Start()
    {


    

        GameObject enemy = Instantiate(originalEnemy, transform.position, transform.rotation, transform.parent);

 
        enemy.GetComponent<EnemyMovementScript>().mode = "guard";
        enemy.GetComponent<EnemyMovementScript>().nodes = nodes;
        enemy.GetComponent<EnemyMovementScript>().cover = cover;



        Destroy(gameObject);
    }
    
}