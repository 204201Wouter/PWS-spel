using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour
{
    public Dictionary<string, AmmoType> ammoTypes = new();
    public Dictionary<string, ScopeAttachment> scopes = new();
    public Dictionary<string, MagazineAttachment> magazines = new();
    public Dictionary<string, SilencerAttachment> silencers = new();
    public Dictionary<string, LaserAttachment> lasers = new();

    public Texture2D normalScopeImage;

    public List<string> availableScopes = new();
    public List<string> availableMagazines = new();
    public List<string> availableSilencers = new();
    public List<string> availableLasers = new();

    public ScopeAttachment currentScope;
    public MagazineAttachment currentMagazine;
    public SilencerAttachment currentSilencer;
    public LaserAttachment currentLaser;

    public Dictionary<string, int> ammoAmounts = new();

    void Start()
    {
        ammoTypes.Add("normal", new(2, 0, 0.5f, 1, 0, 15, 1f, "normal"));
        ammoTypes.Add("small", new(1, 0, 0.2f, 1, 0, 10, 0.6f, "small"));
        ammoTypes.Add("big", new(3, 0, 1, 1, 0, 25, 1.5f, "big"));
        ammoTypes.Add("normalAP", new(1.8f, 2, 0.5f, 1, 0, 15f, 1f, "normalAP")); // AP = armor piercing
        ammoTypes.Add("smallAP", new(0.9f, 1.5f, 0.2f, 1, 0, 10f, 0.6f, "smallAP"));
        ammoTypes.Add("bigAP", new(2.7f, 3, 1, 1, 0, 25f, 1.5f, "bigAP"));
        ammoTypes.Add("buckshot", new(0.5f, 0, 1, 6, 60f, 8f, 0.8f, "buckshot"));
        ammoTypes.Add("birdshot", new(0.2f, 0, 1, 20, 100f, 5f, 0.5f, "birdshot"));
        // waarden voor ammo zijn waarschijnlijk niet goed, moeten we ooit nog veranderen

        foreach (string ammoType in ammoTypes.Keys)
        {
            ammoAmounts.Add(ammoType, 100); // nu beginnen met 100 van elke kogel
        }

        scopes.Add("scopeding", new(2, normalScopeImage));
        // hier alle scopes

        magazines.Add("magazineding", new(30, 1.5f, 0.2f, ammoTypes["birdshot"]));
        // hier alle magazines

        silencers.Add("silencerding", new(1));
        // hier alle silencers

        lasers.Add("laserding", new(Color.red, 0.1f));
        // hier alle lasers

        // tijdelijk
        availableScopes.Add("scopeding");
        availableMagazines.Add("magazineding");
        availableSilencers.Add("silencerding");
        availableLasers.Add("laserding");
        currentScope = scopes["scopeding"];
        currentMagazine = magazines["magazineding"];
        currentSilencer = silencers["silencerding"];
        currentLaser = lasers["laserding"];

        GetComponent<ShootProjectile>().ChangeAttachment();
    }
}

public struct ScopeAttachment
{
    public float zoomFactor;
    public Texture2D scopeImage;

    public ScopeAttachment(float zoomFactor, Texture2D scopeImage)
    {
        this.zoomFactor = zoomFactor;
        this.scopeImage = scopeImage;
    }
}

public struct MagazineAttachment
{
    public int capacity;
    public float reloadTime;
    public float shotCooldown;
    public AmmoType ammoType;

    public MagazineAttachment(int capacity, float reloadTime, float shotCooldown, AmmoType ammoType)
    {
        this.capacity = capacity;
        this.reloadTime = reloadTime;
        this.shotCooldown = shotCooldown;
        this.ammoType = ammoType;
    }
}

public struct SilencerAttachment 
{
    public float soundVolume; 

    public SilencerAttachment(float soundVolume)
    {
        this.soundVolume = soundVolume;
    }
}

public struct LaserAttachment
{
    public Color color;
    public float radius;

    public LaserAttachment(Color color, float radius)
    {
        this.color = color;
        this.radius = radius;
    }
}

public struct AmmoType
{
    public float damage;
    public float armorPiercing;
    public float recoil;
    public int amount;
    public float spread;
    public float range;
    public float size;
    public string name;

    public AmmoType(float damage, float armorPiercing, float recoil, int amount, float spread, float range, float size, string name)
    {
        this.damage = damage;
        this.armorPiercing = armorPiercing;
        this.recoil = recoil;
        this.amount = amount;
        this.spread = spread;
        this.range = range;
        this.size = size;
        this.name = name;
    }
}
// ik weet niet of deze modifiers goed zijn, ik heb ze maar een beetje voor het idee neergezet

