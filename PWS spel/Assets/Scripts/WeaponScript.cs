using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour
{
    public Dictionary<string, AmmoType> ammoTypes = new();
    public Dictionary<string, ScopeAttachment> scopes = new();
    public Dictionary<string, MagazineAttachment> magazines = new();

    public List<string> availableScopes = new();
    public List<string> availableMagazines = new();
    public List<string> availableSilencers = new();
    public List<string> availableLasers = new();

    public ScopeAttachment currentScope;
    public MagazineAttachment currentMagazine;

    public Dictionary<string, int> ammoAmounts = new();

    public GuiScript guiScript;

    public Sprite ARMagSprite;
    public Sprite sniperMagSprite;
    public Sprite drumMagSprite;
    public Sprite ammoNormalSprite;
    public Sprite ammoSmallSprite;
    public Sprite ammoBigSprite;
    public Sprite buckshotSprite;
    public Sprite birdshotSprite;

    public Sprite scopeSpriteBig;
    public Sprite redDotSpriteBig;
    public Sprite ARMagSpriteBig;
    public Sprite sniperMagSpriteBig;
    public Sprite drumMagSpriteBig;
    public Sprite ironSightSpriteBig;

    public Sprite scopeSprite;
    public Sprite redDotSprite;

    public GameObject scopeObject;
    public GameObject reddotObject;
    public GameObject armagObject;
    public GameObject drummagObject;
    public GameObject snipermagObject;

    void Awake() // awake runt eerder dan start en dat moet hier
    {
        // Initialiseer alle waarden van de ammo, magazines en scopes
        ammoTypes.Add("normal", new(20, 0.5f, 1, 0, 1f, "normal", ammoNormalSprite));
        ammoTypes.Add("small", new(10, 0.2f, 1, 0, 0.6f, "small", ammoSmallSprite));
        ammoTypes.Add("big", new(100, 2, 1, 0, 1.5f, "big", ammoBigSprite));
        ammoTypes.Add("buckshot", new(10f, 2, 6, 60f, 0.8f, "buckshot", buckshotSprite));
        ammoTypes.Add("birdshot", new(4f, 2, 20, 100f, 0.5f, "birdshot", birdshotSprite));

        ammoAmounts.Add("normal", 200);
        ammoAmounts.Add("small", 400);
        ammoAmounts.Add("big", 20);
        ammoAmounts.Add("buckshot", 500);
        ammoAmounts.Add("birdshot", 500);

        scopes.Add("scope", new(4, "scope", scopeObject, scopeSprite, scopeSpriteBig)); 
        scopes.Add("red dot", new(2, "red dot", reddotObject, redDotSprite, redDotSpriteBig));
        scopes.Add("no scope", new(1.3f, "no scope", null, null, null));

        // magazine models
        GameObject ARMag = armagObject;
        GameObject drumMag = drummagObject;
        GameObject sniperMag = snipermagObject;

        magazines.Add("default magazine", new(30, 2, 0.1f, ammoTypes["normal"], "default magazine", ARMag, ARMagSprite, ARMagSpriteBig));
        magazines.Add("normal drum", new(100, 5, 0.1f, ammoTypes["normal"], "normal drum", drumMag, drumMagSprite, drumMagSpriteBig));
        magazines.Add("small", new(100, 2, 0.05f, ammoTypes["small"], "small", ARMag, ARMagSprite, ARMagSpriteBig));
        magazines.Add("small drum", new(200, 5, 0.05f, ammoTypes["small"], "small drum", drumMag, drumMagSprite, drumMagSpriteBig));
        magazines.Add("big", new(5, 4, 1f, ammoTypes["big"], "big", sniperMag, sniperMagSprite, sniperMagSpriteBig));
        magazines.Add("buckshot", new(5, 4, 0.5f, ammoTypes["buckshot"], "buckshot", sniperMag, sniperMagSprite, sniperMagSpriteBig));
        magazines.Add("buckshot drum", new(20, 5, 0.5f, ammoTypes["buckshot"], "buckshot drum", drumMag, drumMagSprite, drumMagSpriteBig));
        magazines.Add("birdshot", new(5, 4, 0.5f, ammoTypes["birdshot"], "birdshot", sniperMag, sniperMagSprite, sniperMagSpriteBig));
        magazines.Add("birdshot drum", new(20, 5, 0.5f, ammoTypes["birdshot"], "birdshot drum", drumMag, drumMagSprite, drumMagSpriteBig));

        if (MainMenuScript.newGame)
        {
            // als dit eennieuw spel is, begin met deze attachments equipped
            currentScope = scopes["no scope"];
            currentMagazine = magazines["default magazine"];

            GetComponent<ShootProjectile>().ChangeAttachment();
            Button defaultMagazine = guiScript.magazineSlot.transform.GetChild(1).GetComponent<Button>();
            defaultMagazine.onClick.AddListener(() => guiScript.ClickMagazine(magazines["default magazine"], defaultMagazine.gameObject));
        }

        // Zet alle models uit
        foreach (ScopeAttachment scope in scopes.Values)
        {
            guiScript.SetActiveIfExists(scope.model, false);
        }

        foreach (MagazineAttachment magazine in magazines.Values)
        {
            guiScript.SetActiveIfExists(magazine.model, false);
        }

        if (MainMenuScript.newGame)
        {
            // zet de gebruikte models aan
            guiScript.SetActiveIfExists(currentScope.model, true);
            guiScript.SetActiveIfExists(currentMagazine.model, true);
        }
    }
}

public struct ScopeAttachment
{
    // de waarden van scopes
    public float zoomFactor;
    public string name;
    public GameObject model;
    public Sprite sprite;
    public Sprite spriteBig;

    public ScopeAttachment(float zoomFactor, string name, GameObject model, Sprite sprite, Sprite spriteBig)
    {
        this.zoomFactor = zoomFactor;
        this.name = name;
        this.model = model;
        this.sprite = sprite;
        this.spriteBig = spriteBig;
    }
}

public struct MagazineAttachment
{
    // de waarden van magazines
    public int capacity;
    public float reloadTime;
    public float shotCooldown;
    public AmmoType ammoType;
    public string name;
    public GameObject model;
    public Sprite sprite;
    public Sprite spriteBig;

    public MagazineAttachment(int capacity, float reloadTime, float shotCooldown, AmmoType ammoType, string name, GameObject model, Sprite sprite, Sprite spriteBig)
    {
        this.capacity = capacity;
        this.reloadTime = reloadTime;
        this.shotCooldown = shotCooldown;
        this.ammoType = ammoType;
        this.name = name;
        this.model = model;
        this.sprite = sprite;
        this.spriteBig = spriteBig;
    }
}

public struct AmmoType
{
    // de waarden van soorten ammo
    public float damage;
    public float recoil;
    public int amount;
    public float spread;
    public float size;
    public string name;
    public Sprite sprite;

    public AmmoType(float damage, float recoil, int amount, float spread, float size, string name, Sprite sprite)
    {
        this.damage = damage;
        this.recoil = recoil;
        this.amount = amount;
        this.spread = spread;
        this.size = size;
        this.name = name;
        this.sprite = sprite;
    }
}

