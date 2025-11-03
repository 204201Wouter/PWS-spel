using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

using UnityEngine.UI;


public class ShootProjectile : MonoBehaviour
{
    public GameObject originalProjectile;
    public Transform projectileParent;

    public GameObject originalGrenade;
    public Transform grenadeParent;

    public GameObject player;
    public Transform weapon;
    public Transform weaponsight;
    public Transform weaponmag;
    public WeaponScript weaponScript;
    public Movement movement;

    public InteractScript interactScript;

    public float speed;
    public int ammo;
    public string ammoType;
    public float damage;
    public int amount;
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

        if (((Input.GetMouseButtonDown(0) && !automatic) || (Input.GetMouseButton(0) && automatic)) && ammo > 0 && Time.time > lastShot + shotCooldown && canShoot && reloadStart == -1)
        {

            for (int i = 0; i < amount; i++)
            {
                GameObject projectile = Instantiate(originalProjectile, transform.position, transform.rotation, projectileParent);
                projectile.GetComponent<ProjectileScript>().enabled = true;
                projectile.GetComponent<ProjectileScript>().velocity = transform.forward * speed + GetComponentInParent<Movement>().velocity + Random.onUnitSphere * spread;
                projectile.GetComponent<ProjectileScript>().damage = damage;
                projectile.transform.localScale *= size;
            }
            if (interactScript.gravityDisabled) movement.velocity += amount * damage * 0.003f * -transform.forward;
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



            UpdateAmmoText();

            // weapon.transform.Rotate(recoil * -10, 0f, 0f);
         //   weapon.transform.position += weapon.transform.up*recoil*0.2f;

         //   movement.firedGun = true;



            //  animator.Play("recoil", 2, 0f); // 1 = recoil layer index

            if (ammo % 6 == 0)
            {
                animator.ResetTrigger("recoil");
                animatorshadow.ResetTrigger("recoil");
                animator.SetTrigger("recoil");
                animatorshadow.SetTrigger("recoil");
            }

            if (ammo % 6 == 1)
            {
                animator.ResetTrigger("recoilb");
                animatorshadow.ResetTrigger("recoilb");
                animator.SetTrigger("recoilb");
                animatorshadow.SetTrigger("recoilb");
            }
            if (ammo % 6 == 2)
            {
                animator.ResetTrigger("recoilc");
                animatorshadow.ResetTrigger("recoilc");
                animator.SetTrigger("recoilc");
                animatorshadow.SetTrigger("recoilc");
            }

            if (ammo % 6 == 3)
            {
                animator.ResetTrigger("recoild");
                animatorshadow.ResetTrigger("recoild");
                animator.SetTrigger("recoild");
                animatorshadow.SetTrigger("recoild");
            }
            if (ammo % 6 == 4)
            {
                animator.ResetTrigger("recoile");
                animatorshadow.ResetTrigger("recoile");
                animator.SetTrigger("recoile");
                animatorshadow.SetTrigger("recoile");
            }

            if (ammo % 6 == 5)
            {
                animator.ResetTrigger("recoilf");
                animatorshadow.ResetTrigger("recoilf");
                animator.SetTrigger("recoilf");
                animatorshadow.SetTrigger("recoilf");
            }

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


          //  weapon.transform.position -= weapon.transform.up * recoil * 0.2f;

            // Debug.Log(weapon.transform.localEulerAngles.x);
            //  if (weapon.transform.localEulerAngles.x > 270) weapon.transform.Rotate(Mathf.Rad2Deg * 0.01f, 0f, 0f);
            //   weapon.transform.Rotate(Mathf.Rad2Deg * 0.01f, 0f, 0f);

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
            animatorshadow.SetBool("IsAiming", true);
        }
        else
        {
            if (GetComponent<Camera>().fieldOfView < 60)
            {
                GetComponent<Camera>().fieldOfView += 2;
            }
            mouseLook.mouseSensitivity = 8;
            if (reloadStart == -1)
            {
                animator.SetBool("IsAiming", false);
                animatorshadow.SetBool("IsAiming", false);
            }

            // animator.SetTrigger("Fire");
        }

        animator.SetFloat("reloadspeed", 6f/reloadTime);
        animatorshadow.SetFloat("reloadspeed", 6f/reloadTime);

        if (Input.GetKeyDown(KeyCode.R) && reloadStart == -1 && ammo != cap)
        {
            movement.reloading = true;
            reloadStart = Time.time;
            loadedAmmoText.text = "...";
            animator.SetTrigger("reload");
            animatorshadow.SetTrigger("reload");
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
        amount = weaponScript.currentMagazine.ammoType.amount;
        spread = weaponScript.currentMagazine.ammoType.spread;
        size = weaponScript.currentMagazine.ammoType.size;
        zoom = weaponScript.currentScope.zoomFactor;
        recoil = weaponScript.currentMagazine.ammoType.recoil;
        // andere modifiers nog toevoegen
    }
}
