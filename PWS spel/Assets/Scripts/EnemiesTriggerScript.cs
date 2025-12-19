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
                foreach (Transform enemy in enemiesToTrigger)
                {
                    enemy.GetComponent<EnemyMovementScript>().enabled = true;
                    enemy.GetComponent<EnemyScript>().enabled = true;
                }
            }

            Destroy(GetComponent<BoxCollider>());
        }
    }
}
