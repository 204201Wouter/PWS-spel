using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;

    public GameObject originalGrenade;
    public Transform grenadeParent;

    public GameObject player;
    public GameObject weapon;
    public WeaponScript weaponScript;
    public Movement movement;

    public float speed;
    public int ammo;
    public string ammoType;
    public float damage;
    public int amount;
    public float spread;
    public float size;
    public TextMeshProUGUI AmmoText;
    public Animator animator;
    public float shotCooldown;
    public float reloadTime;

    public MouseLook mouseLook;
    public bool automatic;
    private float recoilXSaved;
    private float recoilYSaved;
    private float lastShot;
    private float reloadStart = -1;

    public int cap;
    public float zoom;

    public bool canShoot = true;


    void Update()
    {

        if (((Input.GetMouseButtonDown(0) && !automatic) || (Input.GetMouseButton(0) && automatic)) && ammo > 0 && Time.time > lastShot + shotCooldown && canShoot)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject projectile = Instantiate(originalProjectile, transform.position, transform.rotation, projectileParent);
                projectile.GetComponent<ProjectileScript>().enabled = true;
                projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed + GetComponentInParent<Movement>().velocity + Random.onUnitSphere * spread;
                projectile.GetComponent<ProjectileScript>().damage = damage;
                projectile.transform.localScale *= size;
            }
            ammo--;

            float recoilX = Random.Range(-50f, 50f);
                
            recoilXSaved += recoilX;
               
            mouseLook.recoilX = recoilX;

            float recoilY = Random.Range(-50f, 50f); 
            if (recoilYSaved < 200)
            {
                recoilY = Random.Range(-0f, 50f);
            }                     
            
            recoilYSaved += recoilY;
            mouseLook.recoilY = recoilY;

            lastShot = Time.time;

            reloadStart = -1;

            UpdateAmmoText();

            weapon.transform.Rotate(Mathf.Rad2Deg * -0.05f, 0f, 0f);

            movement.firedGun = true;


            //   animator.SetTrigger("recoil");

        }
        else
        {
            mouseLook.recoilX = -recoilXSaved * 0.1f;
            mouseLook.recoilY = -recoilYSaved * 0.1f;
            recoilXSaved *= 0.9f;
            recoilYSaved *= 0.9f;
            if (Mathf.Abs(recoilXSaved) < 0.01f) recoilXSaved = 0;
            if (Mathf.Abs(recoilYSaved) < 0.01f) recoilYSaved = 0;

           // Debug.Log(weapon.transform.localEulerAngles.x);
            if (weapon.transform.localEulerAngles.x > 270) 
            //   weapon.transform.Rotate(Mathf.Rad2Deg * 0.01f, 0f, 0f);
            weapon.transform.Rotate(Mathf.Rad2Deg * 0.005f, 0f, 0f);
          //  Debug.Log(weapon.transform.localEulerAngles.x);
           // weapon.transform.Rotate(Mathf.Rad2Deg * -0.01f, 0f, 0f);
        }


        if (Input.GetMouseButton(1) && reloadStart == -1)
        {   
            if (GetComponent<Camera>().fieldOfView > 60 / zoom) 
            {
                GetComponent<Camera>().fieldOfView -= 2;
            }
            mouseLook.mouseSensitivity = 8 / zoom;
            animator.SetBool("IsAiming", true);
        }
        else
        {
            if (GetComponent<Camera>().fieldOfView < 60)
            {
                GetComponent<Camera>().fieldOfView += 2;
            }
            mouseLook.mouseSensitivity = 8;
            animator.SetBool("IsAiming", false);

            // animator.SetTrigger("Fire");
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
                UpdateAmmoText();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject Grenade = Instantiate(originalGrenade, transform.position, transform.rotation, grenadeParent);

            Grenade.GetComponent<BounceProjectileScript>().enabled = true;
            Grenade.GetComponent<BounceProjectileScript>().velocity = transform.forward * 20f + GetComponentInParent<Movement>().velocity;
            Grenade.GetComponent<BounceProjectileScript>().fuse = Time.time;
        }

        if (Time.time > reloadTime + reloadStart && reloadStart != -1) 
        {
            if (weaponScript.ammoAmounts[ammoType] + ammo >= cap)
            {
                weaponScript.ammoAmounts[ammoType] -= cap - ammo;
                ammo = cap;
            }
            else
            {
                ammo += weaponScript.ammoAmounts[ammoType];
                weaponScript.ammoAmounts[ammoType] = 0;
            }
            UpdateAmmoText();
            reloadStart = -1;
        }
    }

    public void UpdateAmmoText()
    {
        AmmoText.text = "Ammo: " + ammo.ToString() + "/" + weaponScript.ammoAmounts[ammoType].ToString();
    }

    public void ChangeAttachment()
    {
        shotCooldown = weaponScript.currentMagazine.shotCooldown;
        ammoType = weaponScript.currentMagazine.ammoType.name;
        cap = weaponScript.currentMagazine.capacity;
        reloadTime = weaponScript.currentMagazine.reloadTime;
        damage = weaponScript.currentMagazine.ammoType.damage;
        amount = weaponScript.currentMagazine.ammoType.amount;
        spread = weaponScript.currentMagazine.ammoType.spread;
        size = weaponScript.currentMagazine.ammoType.size;
        zoom = weaponScript.currentScope.zoomFactor;
        // andere modifiers nog toevoegen
    }
}
