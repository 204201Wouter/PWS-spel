using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;
    public GameObject player;

    public float speed;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = Instantiate(originalProjectile, transform.position, transform.rotation, projectileParent);
            projectile.GetComponent<ProjectileScript>().enabled = true;
            projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed;
        }
    }
}
