using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class ProjectileScript : MonoBehaviour
{
    public LayerMask hitable;
    public Vector3 velocity;

    public float dragFactor;




    void FixedUpdate()
    {

        Vector3 gravity = new Vector3(0f, -9.81f, 0f); 
        velocity = (velocity+ gravity * 0.02f) /(1+ dragFactor * velocity.magnitude * 0.02f) ;
        Vector3 nextPos = transform.position + velocity * 0.02f;

        Ray ray = new Ray(transform.position, velocity.normalized );

        if (Physics.Raycast(ray, out RaycastHit hit, velocity.magnitude * 0.02f, hitable))
        {
            dragFactor = 1f;
            transform.position = hit.point;


            EnemyScript enemyScript = hit.collider.GetComponent<EnemyScript>();
            if (enemyScript != null)
            {
                enemyScript.health--;
                print("hit");
            }
        }

        else 
        {
            dragFactor = 0.000823f;
        }



        transform.position = nextPos;

        if (velocity.magnitude <= 1f || transform.position.y < 0)
        {
            Destroy(gameObject);
        }

    }
}
