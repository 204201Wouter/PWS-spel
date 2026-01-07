using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
    public LayerMask hitable;
    public Vector3 velocity;
    public float damage;
    bool stopped = false;
    public GameObject bulletImpactObj;

    public float dragFactor;

    void FixedUpdate()
    {
        if (!stopped)
        {
            // bepaal waar het projectile de volgende fixedupdate (constant aantal updates per seconde) komt
            Vector3 gravity = new(0f, -9.81f, 0f);
            velocity = (velocity + gravity * Time.fixedDeltaTime) / (1 + dragFactor * velocity.magnitude * Time.fixedDeltaTime);
            Vector3 nextPos = transform.position + velocity * Time.fixedDeltaTime;

            Ray ray = new(transform.position, velocity.normalized);

            // als het projectile tussen deze fixedupdate en de volgende iets raakt
            if (Physics.Raycast(ray, out RaycastHit hit, velocity.magnitude * Time.fixedDeltaTime, hitable))
            {
                dragFactor = 1f;
                transform.position = hit.point;

                // als het een enemie is ga erheen
                EnemyScript enemyScript = hit.collider.GetComponent<EnemyScript>();
                if (enemyScript == null) enemyScript = hit.collider.GetComponentInParent<EnemyScript>();
                if (enemyScript != null)
                {
                    enemyScript.Hit(damage);
                    Destroy(gameObject);
                }

                // ga op de plek zitten en stop met bewegen
                transform.SetParent(hit.transform);
                stopped = true;
                GetComponent<MeshRenderer>().enabled = true;

                // despawn na ongeveer 5 seconden
                Destroy(gameObject, 5f + Random.Range(-0.5f, 0.5f));
            }
            else
            {
                dragFactor = 0.000823f;
                transform.position = nextPos;
            }

            if (transform.position.y < 0 || transform.position.y > 15)
            {
                Destroy(gameObject);
            }
        }
    }


}
