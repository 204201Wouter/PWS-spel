using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;
    public GameObject player;

    public float speed;
    public int ammo;
    public TextMeshProUGUI AmmoText;
    public GameObject sight;
    public float shotCooldown;
    public float reloadTime;

    public MouseLook Mouselook;
    public bool automatic;
    private float recoilXSaved;
    private float recoilYSaved;
    private float lastShot;
    private float reloadStart;


    void Update()
    {

        if (((Input.GetMouseButtonDown(0) && !automatic) || (Input.GetMouseButton(0) && automatic)) && ammo > 0 && Time.time > lastShot + shotCooldown)
        {
      
            GameObject projectile = Instantiate(originalProjectile, transform.position, transform.rotation, projectileParent);
            projectile.GetComponent<ProjectileScript>().enabled = true;
            projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed + GetComponentInParent<Movement>().velocity;
            ammo--;


            float recoilX = Random.Range(-50f, 50f);
                
            recoilXSaved += recoilX;
               
            Mouselook.recoilX = recoilX;

            float recoilY = Random.Range(-50f, 50f); 
            if (recoilYSaved < 200)
            {
                recoilY = Random.Range(-0f, 50f);
      
            }
                
      
            
            recoilYSaved += recoilY;
            Mouselook.recoilY = recoilY;

            lastShot = Time.time;

            reloadStart = -1;

            AmmoText.text = "Ammo: " + ammo.ToString();


            
        }
        else
        {

            Mouselook.recoilX = -recoilXSaved * 0.1f;
            Mouselook.recoilY = -recoilYSaved * 0.1f;
            recoilXSaved *= 0.9f;
            recoilYSaved *= 0.9f;

        }


        if (Input.GetMouseButton(1) && reloadStart == -1)
        {   
            if (GetComponent<Camera>().fieldOfView > 60 * 1 / 2f) 
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
            if (reloadStart == -1)
            {
                reloadStart = Time.time;
                AmmoText.text = "Reloading...";
            }
            else 
            { 
                reloadStart = -1;
                AmmoText.text = "Ammo: " + ammo.ToString();
            }

        }

        if (Time.time > reloadTime + reloadStart && reloadStart != -1) 
        {
            ammo = 30;
            AmmoText.text = "Ammo: " + ammo.ToString();
            reloadStart = -1;
        }

    }
}
