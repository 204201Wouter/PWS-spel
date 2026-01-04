using UnityEngine;
using System.Linq;

public class EnemyWeaponScript : MonoBehaviour
{
    public WeaponScript weaponScript;

    public bool isDropped = false;
    public ScopeAttachment scopeAttachment;
    public MagazineAttachment magazineAttachment;
    public SilencerAttachment silencerAttachment;
    public LaserAttachment laserAttachment;
    public int ammo;
    public LayerMask ground; 
    public float yspeed = 0;
    public MagazineScript magazinescript;

    void Start()
    {
        scopeAttachment = weaponScript.scopes.Values.ToArray()[Random.Range(0, weaponScript.scopes.Count)];
        magazineAttachment = weaponScript.magazines.Values.ToArray()[Random.Range(0, weaponScript.magazines.Count)];
        silencerAttachment = weaponScript.silencers.Values.ToArray()[Random.Range(0, weaponScript.silencers.Count)];
        laserAttachment = weaponScript.lasers.Values.ToArray()[Random.Range(0, weaponScript.lasers.Count)];
        
        GameObject sightobj = transform.Find("sight1").gameObject;
        GameObject scopeobj = transform.Find("scope1").gameObject;
        sightobj.SetActive(false);
        scopeobj.SetActive(false);
        if (scopeAttachment.name == "red dot")
        {
            sightobj.SetActive(true);
        }
        if (scopeAttachment.name == "scope")
        {
            scopeobj.SetActive(true);
        }
        Transform gunParent = transform.parent.parent.Find("Bone.016").Find("ar mag.002");
        GameObject medmag = gunParent.Find("ar mag1").gameObject;
        GameObject bigmag = gunParent.Find("drum mag1").gameObject;
        GameObject smallmag = gunParent.Find("sniper mag1").gameObject;
        medmag.SetActive(false);
        bigmag.SetActive(false);
        smallmag.SetActive(false);
        if (magazineAttachment.reloadTime == 2)
        {
            medmag.SetActive(true);
        }
        if (magazineAttachment.reloadTime == 5)
        {
            bigmag.SetActive(true);
        }
        if (magazineAttachment.reloadTime == 4)
        {
            smallmag.SetActive(true);
        }

        GameObject medmag2 = transform.Find("ar mag.002").Find("ar mag1").gameObject;
        GameObject bigmag2 = transform.Find("ar mag.002").Find("drum mag1").gameObject;
        GameObject smallmag2 = transform.Find("ar mag.002").Find("sniper mag1").gameObject;
        medmag2.SetActive(false);
        bigmag2.SetActive(false);
        smallmag2.SetActive(false);
        if (magazineAttachment.reloadTime == 2)
        {
            medmag2.SetActive(true);
        }
        if (magazineAttachment.reloadTime == 5)
        {
            bigmag2.SetActive(true);
        }
        if (magazineAttachment.reloadTime == 4)
        {
            smallmag2.SetActive(true);
        }    

        ammo = Random.Range(20, 150);

        magazinescript.reloadTime = magazineAttachment.reloadTime;
        magazinescript.cap = magazineAttachment.capacity;
        magazinescript.shotCooldown = magazineAttachment.shotCooldown;
        magazinescript.ammoType = magazineAttachment.ammoType;
    }

    void Update()
    {
        if (isDropped && !Physics.CheckSphere(transform.position, 0.09f, ground) && !InteractScript.gravityDisabled)
        {
            transform.position -= Time.deltaTime * yspeed * Vector3.up;
            yspeed += Time.deltaTime * 10f;
            if (yspeed > 2) yspeed = 2;
        }
        if (isDropped) transform.rotation = Quaternion.Euler(3.806f, transform.eulerAngles.y, -81.497f);
    }
}
