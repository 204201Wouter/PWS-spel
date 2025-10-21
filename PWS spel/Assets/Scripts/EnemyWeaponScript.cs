using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;

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

    void Start()
    {
        scopeAttachment = weaponScript.scopes.Values.ToArray()[Random.Range(0, weaponScript.scopes.Count)];
        magazineAttachment = weaponScript.magazines.Values.ToArray()[Random.Range(0, weaponScript.magazines.Count)];
        silencerAttachment = weaponScript.silencers.Values.ToArray()[Random.Range(0, weaponScript.silencers.Count)];
        laserAttachment = weaponScript.lasers.Values.ToArray()[Random.Range(0, weaponScript.lasers.Count)];

        ammo = Random.Range(20, 150);
    }

    void Update()
    {
        if (isDropped && !Physics.CheckSphere(transform.position, 0.09f, ground))
        {

            //90 10
            transform.position -= Vector3.up* yspeed * Time.deltaTime;
            yspeed += Time.deltaTime*10f;
            if (yspeed > 2) yspeed = 2;

            transform.rotation = Quaternion.Euler(3.806f, transform.eulerAngles.y, -81.497f);
        }
    }
}
