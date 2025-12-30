using UnityEngine;

public class EnemiesTriggerScript : MonoBehaviour
{
    public Transform enemiesToTrigger;
    public bool isLiftRoomTrigger;
    public Transform liftRoom;
    public EnemySpawnScript enemySpawnScript;
    public int enemySpawnAmount;
    public string requirement;
    bool canTrigger = true;
    public Transform nodes;
    public Transform cover;
    public GameObject originalEnemy;
    public Transform parent;
    public float zwidth;
    public float xwidth;
    public LayerMask groundMask;

    void OnTriggerEnter(Collider other)
    {
        switch (requirement)
        {
            case "gravity": canTrigger = InteractScript.gravityDisabled; break;
            case "engine": canTrigger = InteractScript.enginesDisabled; break;
            default: canTrigger = true; break;
        }

        if (other.gameObject.name == "Player" && canTrigger)
        {
            if (isLiftRoomTrigger)
            {
                StartCoroutine(enemySpawnScript.SpawnEnemies(liftRoom, enemySpawnAmount));
            }
            else
            {
                
                for (int i = 0; i<enemySpawnAmount; i++)
                {   
                    bool validSpawn = false;

                    while (validSpawn);
                        Vector3 pos = parent.position+Vector3.left*xwidth*(Random.value-0.5f)+Vector3.forward*zwidth*(Random.value-0.5f);
                        if (!Physics.CheckSphere(pos, 0.4f, groundMask))
                        {
                            validSpawn = true;
                            GameObject enemy = Instantiate(originalEnemy, pos, transform.rotation, parent);
                            enemy.GetComponent<EnemyMovementScript>().enabled = true;
                            enemy.GetComponent<EnemyScript>().enabled = true;

                            enemy.GetComponent<EnemyMovementScript>().mode = "guard";
                            enemy.GetComponent<EnemyMovementScript>().nodes = nodes;
                            enemy.GetComponent<EnemyMovementScript>().cover = cover;
                            
                        }

                }

            }

            Destroy(GetComponent<BoxCollider>());
        }
    }
}
