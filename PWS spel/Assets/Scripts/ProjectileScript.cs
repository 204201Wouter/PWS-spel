using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public LayerMask hitable;
    public Vector3 velocity;
    void Update()
    {
        transform.position += velocity;
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.1f, 0.1f, 0.2f), transform.rotation, hitable);
        if (colliders.Length > 0)
        {
            EnemyScript enemyScript = colliders[0].GetComponent<EnemyScript>();
            if (enemyScript != null)
            {
                enemyScript.health--;
                print("hit");
            }
            Destroy(gameObject);
        }
    }
}
