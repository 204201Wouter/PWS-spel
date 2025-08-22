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

    void Start()
    {
        scopeAttachment = weaponScript.scopes.Values.ToArray()[Random.Range(0, weaponScript.scopes.Count)];
        magazineAttachment = weaponScript.magazines.Values.ToArray()[Random.Range(0, weaponScript.magazines.Count)];
        silencerAttachment = weaponScript.silencers.Values.ToArray()[Random.Range(0, weaponScript.silencers.Count)];
        laserAttachment = weaponScript.lasers.Values.ToArray()[Random.Range(0, weaponScript.lasers.Count)];

        ammo = Random.Range(20, 150);
    }
}
