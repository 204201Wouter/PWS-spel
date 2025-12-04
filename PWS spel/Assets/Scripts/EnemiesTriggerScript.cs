using UnityEngine;

public class EnemiesTriggerScript : MonoBehaviour
{
    bool hasTriggered = false;

    public Transform enemiesToTrigger;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && !hasTriggered)
        {
            hasTriggered = true;
            foreach (Transform enemy in enemiesToTrigger)
            {
                enemy.GetComponent<EnemyMovementScript>().enabled = true;
                enemy.GetComponent<EnemyScript>().enabled = true;
            }
        }
    }
}
