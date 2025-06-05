using UnityEngine;
using TMPro;

public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;
    public GameObject player;

    public float speed;
    public int ammo;
    public TextMeshProUGUI AmmoText;
    public GameObject sight;

    public MouseLook Mouselook;



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ammo > 0)
            {
                GameObject projectile = Instantiate(originalProjectile, transform.position, transform.rotation, projectileParent);
                projectile.GetComponent<ProjectileScript>().enabled = true;
                projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed + GetComponentInParent<Movement>().velocity;
                ammo--;

                AmmoText.text = "Ammo: " + ammo.ToString();
            }
        }

        if (Input.GetMouseButton(1))
        {   
            if (GetComponent<Camera>().fieldOfView > 60 * 1 / 10f) 
            {
                GetComponent<Camera>().fieldOfView -= 1;
                Mouselook.mouseSensitivity -= 2;
            }
                
            sight.SetActive(true);
        }
        else
        {
            if (GetComponent<Camera>().fieldOfView < 60)
            {
                GetComponent<Camera>().fieldOfView += 1;
                Mouselook.mouseSensitivity += 2;
            }

            sight.SetActive(false);
        }
            


        if (Input.GetKeyDown(KeyCode.R))
        {
            ammo = 30;
            AmmoText.text = "Ammo: " + ammo.ToString();


        }
    }
}
