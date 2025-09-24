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
        /*ammoTypes.Add("normalAP", new(1.8f, 2, 0.5f, 1, 0, 15f, 1f, "normalAP")); // AP = armor piercing
        ammoTypes.Add("smallAP", new(0.9f, 1.5f, 0.2f, 1, 0, 10f, 0.6f, "smallAP"));
        ammoTypes.Add("bigAP", new(2.7f, 3, 1, 1, 0, 25f, 1.5f, "bigAP"));*/
        ammoTypes.Add("buckshot", new(0.5f, 0, 1, 6, 60f, 8f, 0.8f, "buckshot"));
        ammoTypes.Add("birdshot", new(0.2f, 0, 1, 20, 100f, 5f, 0.5f, "birdshot"));
        // waarden voor ammo zijn waarschijnlijk niet goed, moeten we ooit nog veranderen

        foreach (string ammoType in ammoTypes.Keys)
        {
            ammoAmounts.Add(ammoType, 100); // nu beginnen met 100 van elke kogel
        }

        scopes.Add("scopeding", new(4, "scopeding", GameObject.Find("scope")));
        scopes.Add("no scope", new(2, "no scope", null));
        // hier alle scopes

        // mag models
        GameObject ARmag = GameObject.Find("ar mag");
        GameObject drumMag = GameObject.Find("drum mag");
        GameObject sniperMag = GameObject.Find("sniper mag");

        magazines.Add("default magazine", new(30, 2, 0.1f, ammoTypes["normal"], "default magazine", ARmag));
        magazines.Add("normal drum", new(100, 5, 0.1f, ammoTypes["normal"], "normal drum", drumMag));
        magazines.Add("small", new(100, 2, 0.05f, ammoTypes["small"], "small", ARmag));
        magazines.Add("small drum", new(200, 5, 0.05f, ammoTypes["small"], "small drum", drumMag));
        magazines.Add("big", new(5, 4, 0.5f, ammoTypes["big"], "big", sniperMag));
        magazines.Add("buckshot", new(5, 4, 0.5f, ammoTypes["buckshot"], "buckshot", sniperMag));
        magazines.Add("buckshot drum", new(20, 5, 0.5f, ammoTypes["buckshot"], "buckshot drum", drumMag));
        magazines.Add("birdshot", new(5, 4, 0.5f, ammoTypes["birdshot"], "birdshot", sniperMag));
        magazines.Add("birdshot drum", new(20, 5, 0.5f, ammoTypes["birdshot"], "birdshot drum", drumMag));
        // hier alle magazines

        silencers.Add("silencerding", new(1, "silencerding", GameObject.Find("nog niet toegevoegd")));
        silencers.Add("no silencer", new(0, "no silencer", null));
        // hier alle silencers

        lasers.Add("laserding", new(Color.red, 0.1f, "laserding", GameObject.Find("nog niet toegevoegd")));
        lasers.Add("no laser", new(Color.red, 0, "no laser", null));
        // hier alle lasers

        // tijdelijk
        currentScope = scopes["no scope"];
        currentMagazine = magazines["default magazine"];
        currentSilencer = silencers["no silencer"];
        currentLaser = lasers["no laser"];

        GetComponent<ShootProjectile>().ChangeAttachment();
        Button defaultMagazine = guiScript.magazineSlot.transform.GetChild(0).GetComponent<Button>();
        defaultMagazine.onClick.AddListener(() => guiScript.ClickMagazine(magazines["default magazine"], defaultMagazine.gameObject));

        guiScript.NewMagazine(magazines["small drum"]);
        guiScript.NewScope(scopes["scopeding"]);

        foreach (ScopeAttachment scope in scopes.Values)
        {
            guiScript.SetActiveIfExists(scope.model, false);
        }

        foreach (MagazineAttachment magazine in magazines.Values)
        {
            guiScript.SetActiveIfExists(magazine.model, false);
        }

        foreach (SilencerAttachment silencer in silencers.Values)
        {
            guiScript.SetActiveIfExists(silencer.model, false);
        }

        foreach (LaserAttachment laser in lasers.Values)
        {
            guiScript.SetActiveIfExists(laser.model, false);
        }

        guiScript.SetActiveIfExists(currentScope.model, true);
        guiScript.SetActiveIfExists(currentMagazine.model, true);
        guiScript.SetActiveIfExists(currentSilencer.model, true);
        guiScript.SetActiveIfExists(currentLaser.model, true);
    }
}

public struct ScopeAttachment
{
    public float zoomFactor;
    public string name;
    public GameObject model;

    public ScopeAttachment(float zoomFactor, string name, GameObject model)
    {
        this.zoomFactor = zoomFactor;
        this.name = name;
        this.model = model;
    }
}

public struct MagazineAttachment
{
    public int capacity;
    public float reloadTime;
    public float shotCooldown;
    public AmmoType ammoType;
    public string name;
    public GameObject model;

    public MagazineAttachment(int capacity, float reloadTime, float shotCooldown, AmmoType ammoType, string name, GameObject model)
    {
        this.capacity = capacity;
        this.reloadTime = reloadTime;
        this.shotCooldown = shotCooldown;
        this.ammoType = ammoType;
        this.name = name;
        this.model = model;
    }
}

public struct SilencerAttachment 
{
    public float soundVolume;
    public string name;
    public GameObject model;

    public SilencerAttachment(float soundVolume, string name, GameObject model)
    {
        this.soundVolume = soundVolume;
        this.name = name;
        this.model = model;
    }
}

public struct LaserAttachment
{
    public Color color;
    public float radius;
    public string name;
    public GameObject model;

    public LaserAttachment(Color color, float radius, string name, GameObject model)
    {
        this.color = color;
        this.radius = radius;
        this.name = name;
        this.model = model;
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

