using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;

    public GameObject originalGrenade;
    public Transform grenadeParent;

    public GameObject player;
    public WeaponScript weaponScript;

    public float speed;
    public int ammo;
    public string ammoType;
    public float damage;
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

    public int cap;
    public float zoom;


    void Update()
    {
        if (((Input.GetMouseButtonDown(0) && !automatic) || (Input.GetMouseButton(0) && automatic)) && ammo > 0 && Time.time > lastShot + shotCooldown)
        {
      
            GameObject projectile = Instantiate(originalProjectile, transform.position, transform.rotation, projectileParent);
            projectile.GetComponent<ProjectileScript>().enabled = true;
            projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed + GetComponentInParent<Movement>().velocity;
            projectile.GetComponent<ProjectileScript>().damage = damage;
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

            AmmoText.text = "Ammo: " + ammo.ToString() + "/" + weaponScript.ammoAmounts[ammoType].ToString();
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
            if (GetComponent<Camera>().fieldOfView > 60 * 1 / zoom) 
            {
                GetComponent<Camera>().fieldOfView -= 2;
                Mouselook.mouseSensitivity -= 4;
            }
                
            sight.SetActive(true);
        }
        else
        {
            if (GetComponent<Camera>().fieldOfView < 60)
            {
                GetComponent<Camera>().fieldOfView += 2;
                Mouselook.mouseSensitivity += 4;
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
                AmmoText.text = "Ammo: " + ammo.ToString() + "/" + weaponScript.ammoAmounts[ammoType].ToString();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject Grenade = Instantiate(originalGrenade, transform.position, transform.rotation, grenadeParent);

            Grenade.GetComponent<BounceProjectileScript>().enabled = true;
            Grenade.GetComponent<BounceProjectileScript>().velocity = transform.forward * 20f+ GetComponentInParent<Movement>().velocity;
            Grenade.GetComponent<BounceProjectileScript>().fuse = Time.time;
        }

        if (Time.time > reloadTime + reloadStart && reloadStart != -1) 
        {
            if (weaponScript.ammoAmounts[ammoType] >= cap)
            {
                weaponScript.ammoAmounts[ammoType] -= cap - ammo;
                ammo = cap;
            }
            else
            {
                ammo = weaponScript.ammoAmounts[ammoType];
                weaponScript.ammoAmounts[ammoType] = 0;
            }
            AmmoText.text = "Ammo: " + ammo.ToString() + "/" + weaponScript.ammoAmounts[ammoType].ToString();
            reloadStart = -1;
        }
    }

    // roep deze functie aan als attachment word veranderd
    public void ChangeAttachment()
    {
        shotCooldown = weaponScript.currentMagazine.shotCooldown;
        ammoType = weaponScript.currentMagazine.ammoType.name;
        cap = weaponScript.currentMagazine.capacity;
        reloadTime = weaponScript.currentMagazine.reloadTime;
        damage = weaponScript.currentMagazine.ammoType.damage;
        // andere modifiers nog toevoegen
    }
}
