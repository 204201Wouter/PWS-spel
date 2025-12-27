using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
            Vector3 gravity = new Vector3(0f, -9.81f, 0f);
            velocity = (velocity + gravity * Time.fixedDeltaTime) / (1 + dragFactor * velocity.magnitude * Time.fixedDeltaTime);
            Vector3 nextPos = transform.position + velocity * Time.fixedDeltaTime;

            Ray ray = new Ray(transform.position, velocity.normalized);

            if (Physics.Raycast(ray, out RaycastHit hit, velocity.magnitude * Time.fixedDeltaTime, hitable))
            {
                dragFactor = 1f;
                transform.position = hit.point;


                EnemyScript enemyScript = hit.collider.GetComponent<EnemyScript>();
                if (enemyScript == null) enemyScript = hit.collider.GetComponentInParent<EnemyScript>();
                if (enemyScript != null)
                {
                    enemyScript.Hit(damage);
                    Destroy(gameObject);
                }

                transform.SetParent(hit.transform);
                stopped = true;
                GetComponent<MeshRenderer>().enabled = true;
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
