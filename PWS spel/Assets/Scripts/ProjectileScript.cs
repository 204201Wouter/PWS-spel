using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public LayerMask notPlayer;
    public Vector3 velocity;
    void Update()
    {
        transform.position += velocity;
        if (Physics.CheckSphere(transform.position, 0.5f, notPlayer))
        {
            //Destroy(gameObject);
        }
    }
}
