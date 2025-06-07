using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BounceProjectileScript : MonoBehaviour
{
    public LayerMask hitable;
    public Vector3 velocity;

    public float dragFactor;

    public float fuse = -1;




    void FixedUpdate()
    {

        Vector3 gravity = new Vector3(0f, -9.81f, 0f); 
        velocity = (velocity+ gravity * 0.02f) /(1+ dragFactor * velocity.magnitude * 0.02f) ;
        Vector3 nextPos = transform.position + velocity * 0.02f;

        Ray ray = new Ray(transform.position, velocity.normalized );

        if (Physics.SphereCast(ray, 0.25f, out RaycastHit hit, velocity.magnitude * 0.02f, hitable))
        {

            hit.point += hit.normal.normalized * 0.25f;
            Vector3 velIn = hit.point-transform.position;
            transform.position = hit.point;



            Vector3 velOutLen = (velocity * 0.02f - velIn);


            Vector3 velocityNormal = Vector3.Dot(velOutLen, hit.normal) * hit.normal;


            Vector3 velocityTangential = (velOutLen - velocityNormal)*0.92f;


            Vector3 newVelocityNormal = -velocityNormal * 0.6f;


            Vector3 velOut = velocityTangential + newVelocityNormal;


            //  Vector3 velOut = Vector3.Reflect(velOutLen, hit.normal);
            // moving = false;




            transform.position += velOut;



         //   velocity = Vector3.Reflect(velocity, hit.normal) * 0.6f;




            velocityNormal = Vector3.Dot(velocity, hit.normal) * hit.normal;


            velocityTangential = (velocity - velocityNormal)*0.92f;


            newVelocityNormal = -velocityNormal * 0.6f;


            velocity = velocityTangential + newVelocityNormal;







        }


        else
        {

            transform.position = nextPos; 

            
            
        }

        if (Time.time >= fuse+5 && fuse != -1)
        {
            Destroy(gameObject);
        }

    }
}
