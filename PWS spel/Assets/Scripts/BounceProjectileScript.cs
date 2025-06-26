using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BounceProjectileScript : MonoBehaviour
{
    public LayerMask hitable;
    public Vector3 velocity;

    public float dragFactor;

    public float fuse = -1;


    public GameObject originalProjectile;
    public Transform projectileParent;

    void FixedUpdate()
    {

        Vector3 gravity = new Vector3(0f, -9.81f, 0f); 
        velocity = (velocity+ gravity * 0.02f) /(1+ dragFactor * velocity.magnitude * 0.02f) ;
        Vector3 nextPos = transform.position + velocity * 0.02f;



        Vector3 bounceVel = velocity * 0.02f;

        Vector3 velOutLen;
        Vector3 velocityNormal;
        Vector3 velocityTangential;
        Vector3 newVelocityNormal;
        Vector3 velOut;



        while (Physics.SphereCast(new Ray(transform.position, bounceVel.normalized), 0.25f, out RaycastHit hit, bounceVel.magnitude, hitable) && bounceVel.magnitude > 0)
        {

            hit.point += hit.normal.normalized * 0.25f;
            Vector3 velIn = hit.point-transform.position;
            transform.position = hit.point;



            velOutLen = (bounceVel - velIn);
            velocityNormal = Vector3.Dot(velOutLen, hit.normal) * hit.normal;
            velocityTangential = (velOutLen - velocityNormal)*0.92f;
            newVelocityNormal = -velocityNormal * 0.6f;
            velOut = velocityTangential + newVelocityNormal;


            bounceVel = velOut;




            velocityNormal = Vector3.Dot(velocity, hit.normal) * hit.normal;
            velocityTangential = (velocity - velocityNormal)*0.92f;
            newVelocityNormal = -velocityNormal * 0.6f;
            velocity = velocityTangential + newVelocityNormal;



            nextPos = transform.position + velOut;


        }



        transform.position = nextPos; 


        if (Time.time >= fuse+5 && fuse != -1)
        {
            for (int i = 0; i<500; i++)
            {
                Vector3 dir = Random.onUnitSphere;
                Ray ray = new Ray(transform.position, dir);

                if (Physics.Raycast(ray, out RaycastHit hit, 100, hitable))
                {
             

                    EnemyScript enemyScript = hit.collider.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.Hit(1);
                        print("hit");
                    }
                    PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.Hit(1);
                        print("hit");
                    }
                }

                //   Vector3 Direction = Random.onUnitSphere;
              //  Vector3 dir = (player.transform.position - transform.position).normalized;

             //   Debug.DrawRay(transform.position, Direction * 100, Color.red, 2f);

                   // return !Physics.Raycast(transform.position, dir, (player.transform.position - transform.position).magnitude, groundMask);


                //   GameObject projectile = Instantiate(originalProjectile, transform.position, Quaternion.LookRotation(Direction), projectileParent);
                //   projectile.GetComponent<ProjectileScript>().enabled = true;
                //  projectile.GetComponent<ProjectileScript>().velocity = Direction * 1000 + velocity;
            }

            Destroy(gameObject);
        }

    }

  
}
