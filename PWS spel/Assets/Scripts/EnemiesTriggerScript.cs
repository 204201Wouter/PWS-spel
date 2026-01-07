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
    public Transform enemyParent;
    public Transform room;
    public float zwidth;
    public float xwidth;
    public LayerMask groundMask;

    void OnTriggerEnter(Collider other)
    {
        canTrigger = requirement switch
        {
            "gravity" => InteractScript.gravityDisabled,
            "engine" => InteractScript.enginesDisabled,
            _ => true,
        };

        if (other.gameObject.name == "Player" && canTrigger)
        {
            if (isLiftRoomTrigger)
            {
                StartCoroutine(enemySpawnScript.SpawnEnemies(liftRoom, enemySpawnAmount));
            }
            else
            {
                for (int i = 0; i < enemySpawnAmount; i++)
                {   
                    Vector3 pos = room.position + (Random.value - 0.5f) * xwidth * Vector3.left + (Random.value - 0.5f) * zwidth * Vector3.forward;

                    // als ik hier een while loop van maak crasht het spel, dus nu probeert hij gewoon 20 keer om een positie te krijgen die niet in een object zit
                    for (int j = 0; j < 20; j++)
                    {
                        if (Physics.CheckSphere(pos, 0.4f, groundMask)) pos = room.position + (Random.value - 0.5f) * xwidth * Vector3.left + (Random.value - 0.5f) * zwidth * Vector3.forward;
                        else break;
                    }

                    // spawn de enemy en initialiseer hem
                    GameObject enemy = Instantiate(originalEnemy, pos, transform.rotation, enemyParent);
                    enemy.GetComponent<EnemyMovementScript>().enabled = true;
                    enemy.GetComponent<EnemyScript>().enabled = true;

                    enemy.GetComponent<EnemyMovementScript>().mode = "guard";
                    enemy.GetComponent<EnemyMovementScript>().nodes = nodes;
                    enemy.GetComponent<EnemyMovementScript>().cover = cover;
                    enemy.GetComponent<EnemyMovementScript>().room = nodes.gameObject.name;
                    enemy.GetComponentInChildren<EnemyWeaponScript>().RandomizeAttachments();
                    enemy.GetComponentInChildren<EnemyWeaponScript>().InitializeValues();
                }
            }

            // schakel uit na triggeren
            Destroy(GetComponent<BoxCollider>());
        }
    }
}
