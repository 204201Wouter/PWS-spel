using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerHealth;
    public float[] playerPosition;

    public float[] enemyHealths;
    public float[][] enemyPositions;
    public string[] enemyRooms;
    public string[] enemyModes;
    public string[] enemyMagazines;
    public string[] enemyScopes;
    public int[] enemyAmmos;

    public int loadedAmmo;
    public int[] ammoAmounts;

    public string currentMagazine;
    public string currentScope;
    public string[] availableMagazines;
    public string[] availableScopes;

    public bool mapObtained;
    public bool toolObtained;
    public bool bombObtained;
    public bool engine1Disabled;
    public bool engine2Disabled;
    public bool enginesDisabled;
    public bool bombPlanted;
    public int bombTimer;
    public bool gravityDisabled;

    public int enemiesKilled;
    public bool door1locked;
    public bool door2locked;

    public bool computerRoomEnabled;
    public bool storageRoomEnabled;
    public bool engineRoomEnabled;
    public bool officeRoomEnabled;
    public bool gravityRoomEnabled;
    public bool escapePodsEnabled;

    public bool liftRoom1Enabled;
    public bool liftRoom2aEnabled;
    public bool liftRoom2bEnabled;
    public bool liftRoom3aEnabled;
    public bool liftRoom3bEnabled;

    public int currentObjectiveIndex;
    public int currentAmountDone;

    public float volume;
    public float difficulty;

    public SaveData (PlayerHealth player, Transform enemyParent, WeaponScript weaponScript, InteractScript interactScript, UnlockableDoorHandler unlockableDoorHandler, Transform enableEnemiesTriggers, GuiScript guiScript)
    {
        playerHealth = player.health;
        playerPosition = new float[3];
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = player.transform.position.y;
        playerPosition[2] = player.transform.position.z;

        EnemyScript[] enemies = enemyParent.GetComponentsInChildren<EnemyScript>();

        enemyHealths = new float[enemies.Length];
        enemyPositions = new float[enemies.Length][];
        enemyRooms = new string[enemies.Length];
        enemyModes = new string[enemies.Length];
        enemyMagazines = new string[enemies.Length];
        enemyScopes = new string[enemies.Length];
        enemyAmmos = new int[enemies.Length];

        int i = 0;
        foreach (EnemyScript enemy in enemies)
        {
            enemyHealths[i] = enemy.health;
            enemyPositions[i] = new float[3];
            enemyPositions[i][0] = enemy.transform.position.x;
            enemyPositions[i][1] = enemy.transform.position.y;
            enemyPositions[i][2] = enemy.transform.position.z;

            EnemyMovementScript enemyMovementScript = enemy.GetComponent<EnemyMovementScript>();
            enemyRooms[i] = enemyMovementScript.room;
            enemyModes[i] = enemyMovementScript.mode;

            EnemyWeaponScript enemyWeaponScript = enemy.GetComponentInChildren<EnemyWeaponScript>();
            enemyMagazines[i] = enemyWeaponScript.magazineAttachment.name;
            enemyScopes[i] = enemyWeaponScript.scopeAttachment.name;
            enemyAmmos[i] = enemyWeaponScript.ammo;

            i++;
        }

        loadedAmmo = player.GetComponentInChildren<ShootProjectile>().projectilesPerShot;
        ammoAmounts = new int[5];
        ammoAmounts[0] = weaponScript.ammoAmounts["normal"];
        ammoAmounts[1] = weaponScript.ammoAmounts["small"];
        ammoAmounts[2] = weaponScript.ammoAmounts["big"];
        ammoAmounts[3] = weaponScript.ammoAmounts["buckshot"];
        ammoAmounts[4] = weaponScript.ammoAmounts["birdshot"];

        currentMagazine = weaponScript.currentMagazine.name;
        currentScope = weaponScript.currentScope.name;

        availableMagazines = new string[weaponScript.availableMagazines.Count];
        i = 0;
        foreach (string magazine in weaponScript.availableMagazines)
        {
            availableMagazines[i] = magazine;
            i++;
        }

        availableScopes = new string[weaponScript.availableScopes.Count];
        i = 0;
        foreach (string scope in weaponScript.availableScopes)
        {
            availableScopes[i] = scope;
            i++;
        }

        mapObtained = interactScript.mapDownloaded;
        toolObtained = interactScript.toolObtained;
        bombObtained = interactScript.bombObtained;
        engine1Disabled = interactScript.engine1Disabled;
        engine2Disabled = interactScript.engine2Disabled;
        enginesDisabled = InteractScript.enginesDisabled;
        bombPlanted = interactScript.bombPlanted;
        bombTimer = interactScript.timer;
        gravityDisabled = InteractScript.gravityDisabled;

        enemiesKilled = unlockableDoorHandler.enemiesKilled;
        door1locked = unlockableDoorHandler.door1.locked;
        door2locked = unlockableDoorHandler.door2.locked;

        computerRoomEnabled = enableEnemiesTriggers.GetChild(0).TryGetComponent(out BoxCollider _);
        storageRoomEnabled = enableEnemiesTriggers.GetChild(1).TryGetComponent(out BoxCollider _);
        engineRoomEnabled = enableEnemiesTriggers.GetChild(2).TryGetComponent(out BoxCollider _);
        officeRoomEnabled = enableEnemiesTriggers.GetChild(3).TryGetComponent(out BoxCollider _);
        gravityRoomEnabled = enableEnemiesTriggers.GetChild(4).TryGetComponent(out BoxCollider _);
        escapePodsEnabled = enableEnemiesTriggers.GetChild(5).TryGetComponent(out BoxCollider _);

        liftRoom1Enabled = enableEnemiesTriggers.GetChild(6).TryGetComponent(out BoxCollider _);
        liftRoom2aEnabled = enableEnemiesTriggers.GetChild(7).TryGetComponent(out BoxCollider _);
        liftRoom2bEnabled = enableEnemiesTriggers.GetChild(8).TryGetComponent(out BoxCollider _);
        liftRoom3aEnabled = enableEnemiesTriggers.GetChild(9).TryGetComponent(out BoxCollider _);
        liftRoom3bEnabled = enableEnemiesTriggers.GetChild(10).TryGetComponent(out BoxCollider _);

        currentObjectiveIndex = guiScript.currentObjectiveIndex;
        currentAmountDone = guiScript.currentAmountDone;

        volume = AudioListener.volume;
        difficulty = PlayerHealth.difficulty;
    }
}
