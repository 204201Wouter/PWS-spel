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

    public List<string> availableScopes = new();
    public List<string> availableMagazines = new();
    public List<string> availableSilencers = new();
    public List<string> availableLasers = new();

    public ScopeAttachment currentScope;
    public MagazineAttachment currentMagazine;
    public SilencerAttachment currentSilencer;
    public LaserAttachment currentLaser;

    public Dictionary<string, int> ammoAmounts = new();

    public GuiOpenScript guiScript;

    void Awake()
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

        scopes.Add("scopeding", new(4, "scopeding"));
        scopes.Add("no scope", new(2, "no scope"));
        // hier alle scopes

        magazines.Add("magazineding", new(30, 1.5f, 0.2f, ammoTypes["birdshot"], "magazineding"));
        magazines.Add("default magazine", new(30, 1.5f, 0.2f, ammoTypes["normal"], "default magazine"));
        // hier alle magazines

        silencers.Add("silencerding", new(1, "silencerding"));
        silencers.Add("no silencer", new(0, "no silencer"));
        // hier alle silencers

        lasers.Add("laserding", new(Color.red, 0.1f, "laserding"));
        lasers.Add("no laser", new(Color.red, 0, "no laser"));
        // hier alle lasers

        // tijdelijk
        currentScope = scopes["no scope"];
        currentMagazine = magazines["default magazine"];
        currentSilencer = silencers["no silencer"];
        currentLaser = lasers["no laser"];

        GetComponent<ShootProjectile>().ChangeAttachment();
        Button defaultMagazine = guiScript.magazineSlot.transform.GetChild(0).GetComponent<Button>();
        defaultMagazine.onClick.AddListener(() => guiScript.ClickMagazine(magazines["default magazine"], defaultMagazine.gameObject));

        guiScript.NewMagazine(magazines["magazineding"]);
        guiScript.NewScope(scopes["scopeding"]);
    }
}

public struct ScopeAttachment
{
    public float zoomFactor;
    public string name;

    public ScopeAttachment(float zoomFactor, string name)
    {
        this.zoomFactor = zoomFactor;
        this.name = name;
    }
}

public struct MagazineAttachment
{
    public int capacity;
    public float reloadTime;
    public float shotCooldown;
    public AmmoType ammoType;
    public string name;

    public MagazineAttachment(int capacity, float reloadTime, float shotCooldown, AmmoType ammoType, string name)
    {
        this.capacity = capacity;
        this.reloadTime = reloadTime;
        this.shotCooldown = shotCooldown;
        this.ammoType = ammoType;
        this.name = name;
    }
}

public struct SilencerAttachment 
{
    public float soundVolume;
    public string name;

    public SilencerAttachment(float soundVolume, string name)
    {
        this.soundVolume = soundVolume;
        this.name = name;
    }
}

public struct LaserAttachment
{
    public Color color;
    public float radius;
    public string name;

    public LaserAttachment(Color color, float radius, string name)
    {
        this.color = color;
        this.radius = radius;
        this.name = name;
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

