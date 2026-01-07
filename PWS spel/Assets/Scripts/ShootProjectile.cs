using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;

    public GameObject player;
    public Transform weapon;
    public Transform weaponsight;
    public Transform weaponmag;
    public WeaponScript weaponScript;
    public Movement movement;

    public float speed;
    public int ammo = 0;
    public string ammoType = "normal";
    public float damage;
    public int projectilesPerShot;
    public float spread;
    public float size;
    public float recoil;
    public TextMeshProUGUI loadedAmmoText;
    public TextMeshProUGUI totalAmmoText;
    public Animator animator;
    public Animator animatorshadow;
    public float shotCooldown;
    public float reloadTime;

    public Image ammoimg;
    public Image magimg;
    public Image sight;

    public MouseLook mouseLook;
    public bool automatic;
    private float recoilXSaved;
    private float recoilYSaved;
    private float lastShot;
    private float reloadStart = -1;

    public int cap;
    public float zoom;
    public bool aiming;

    public bool canShoot = true;

    public AudioSource audioSource;
    public AudioClip shotsound;
    public AudioClip reloadsound;

    public GameObject fpsbody;

    public float recoilstrength;

    public float down;

    void Update()
    {
        // als je kan schiet en op lmb drukt, schiet
        if (Input.GetMouseButton(0) && ammo > 0 && Time.time > lastShot + shotCooldown && canShoot && reloadStart == -1)
        {
            audioSource.PlayOneShot(shotsound);

            for (int i = 0; i < projectilesPerShot; i++)
            {
                // maak een projectile voor elke kogel die je schiet (alleen meerdere bij de shotguns)
                GameObject projectile = Instantiate(originalProjectile, transform.position+Vector3.up*down, transform.rotation, projectileParent);
                projectile.GetComponent<ProjectileScript>().enabled = true;
                projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed + GetComponentInParent<Movement>().velocity + Random.onUnitSphere * spread;
                projectile.GetComponent<ProjectileScript>().damage = damage;
                projectile.transform.localScale *= size;
            }

            // als zwaartekracht uit is beweeh de speler naar achteren
            if (InteractScript.gravityDisabled) movement.velocity += projectilesPerShot * damage * 0.15f * -transform.forward;
            ammo--;

            // zorgt voor random recoil
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

            UpdateAmmoText();
            
            movement.recoil = recoilstrength;
        }
        else
        {
            // recoil gaat langzaam weg
            mouseLook.recoilX = -recoilXSaved * 0.1f;
            mouseLook.recoilY = -recoilYSaved * 0.1f;
            recoilXSaved *= 0.9f;
            recoilYSaved *= 0.9f;
            if (Mathf.Abs(recoilXSaved) < 0.01f) recoilXSaved = 0;
            if (Mathf.Abs(recoilYSaved) < 0.01f) recoilYSaved = 0;
        }

        if (movement.recoil > 0)
        {
            movement.recoil -= Time.deltaTime;
        }

        // aimen logica
        if (Input.GetMouseButton(1) && reloadStart == -1 && canShoot)
        {   
            // zoomt de camera geleidelijk in
            if (GetComponent<Camera>().fieldOfView > 60 / zoom) 
            {
                GetComponent<Camera>().fieldOfView -= 2;
            }
            if (GetComponent<Camera>().fieldOfView < 60 / zoom * 2 && zoom >= 4f) 
            {
                sight.enabled = true;
                fpsbody.SetActive(false);
            }
            mouseLook.mouseSensitivity = 8 / zoom;

            animator.SetBool("IsAiming", true);
            animatorshadow.SetBool("IsAiming", true);

            aiming = true;
        }
        else
        {
            fpsbody.SetActive(true);
            sight.enabled = false;
            // zoomt de camera geleidelijk uit
            if (GetComponent<Camera>().fieldOfView < 60)
            {
                GetComponent<Camera>().fieldOfView += 2;
            }
            else aiming = false;
            mouseLook.mouseSensitivity = 8;

            if (reloadStart == -1)
            {
                animator.SetBool("IsAiming", false);
                animatorshadow.SetBool("IsAiming", false);
            }
        }

        animator.SetFloat("reloadspeed", 6f/reloadTime);
        animatorshadow.SetFloat("reloadspeed", 6f/reloadTime);

        // start met reloaden als je op R drukt en het kan
        if ((Input.GetKeyDown(KeyCode.R) || (Input.GetMouseButtonDown(0) && ammo == 0)) && reloadStart == -1 && canShoot)
        {
            fpsbody.SetActive(true);
            audioSource.PlayOneShot(reloadsound);
            movement.reloading = true;
            reloadStart = Time.time;
            loadedAmmoText.text = "...";
            animator.SetTrigger("reload");
            animatorshadow.SetTrigger("reload");
        }
        
        // als reload klaar is
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
            movement.reloading = false;
            UpdateAmmoText();
            reloadStart = -1;
        }
    }

    public void UpdateAmmoText()
    {
        loadedAmmoText.text = ammo.ToString();
        totalAmmoText.text = weaponScript.ammoAmounts[ammoType].ToString();
    }

    public void ChangeAttachment()
    {
        shotCooldown = weaponScript.currentMagazine.shotCooldown;
        ammoType = weaponScript.currentMagazine.ammoType.name;
        cap = weaponScript.currentMagazine.capacity;
        reloadTime = weaponScript.currentMagazine.reloadTime;
        damage = weaponScript.currentMagazine.ammoType.damage;
        projectilesPerShot = weaponScript.currentMagazine.ammoType.amount;
        spread = weaponScript.currentMagazine.ammoType.spread;
        size = weaponScript.currentMagazine.ammoType.size;
        zoom = weaponScript.currentScope.zoomFactor;
        recoil = weaponScript.currentMagazine.ammoType.recoil;
    }
}
