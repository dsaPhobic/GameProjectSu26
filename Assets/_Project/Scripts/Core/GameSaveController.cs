using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveController : MonoBehaviour
{
    private enum StartMode
    {
        NewGame,
        LoadGame,
        ResumeGame
    }

    private static GameSaveController _instance;
    private static StartMode _nextStartMode = StartMode.NewGame;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance == null)
            new GameObject("GameSaveController").AddComponent<GameSaveController>();
    }

    public static void StartNewGame()
    {
        _nextStartMode = StartMode.NewGame;
        SaveSystem.DeleteSave();
        PlayerStats.ResetSharedStateForNewGame();
    }

    public static void ContinueGame()
    {
        _nextStartMode = StartMode.LoadGame;
    }

    public static void ResumeGame()
    {
        _nextStartMode = StartMode.ResumeGame;
    }

    public static void SaveCurrentGame()
    {
        _instance?.SaveCurrentGameInternal();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        SaveCurrentGameInternal();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsGameplayScene(scene.name) || _nextStartMode == StartMode.NewGame)
            return;

        bool restorePlayerPosition = _nextStartMode == StartMode.LoadGame;
        _nextStartMode = StartMode.NewGame;
        StartCoroutine(LoadWhenSceneReady(restorePlayerPosition));
    }

    private IEnumerator LoadWhenSceneReady(bool restorePlayerPosition)
    {
        GameSaveData data = SaveSystem.Load();
        if (data == null) yield break;

        PlayerStats stats = null;
        PlayerController player = null;
        FarmManager farm = null;
        PowerUpSpawner powerUpSpawner = null;
        WaveManager waveManager = null;

        for (int i = 0; i < 60; i++)
        {
            player = ServiceLocator.Get<PlayerController>();
            if (player == null) player = FindObjectOfType<PlayerController>();

            stats = player != null ? player.GetComponent<PlayerStats>() : FindObjectOfType<PlayerStats>();
            farm = ServiceLocator.Get<FarmManager>();
            if (farm == null) farm = FindObjectOfType<FarmManager>();
            powerUpSpawner = FindObjectOfType<PowerUpSpawner>();
            waveManager = ServiceLocator.Get<WaveManager>();
            if (waveManager == null) waveManager = FindObjectOfType<WaveManager>();

            if (stats != null && player != null && farm != null && powerUpSpawner != null && waveManager != null)
                break;

            yield return null;
        }

        if (stats == null || player == null) yield break;

        int damage = data.playerDamage > 0 ? data.playerDamage : stats.Damage;
        float attackSpeed = data.playerAttackSpeed > 0f ? data.playerAttackSpeed : stats.AttackSpeed;
        float moveSpeed = data.playerMoveSpeed > 0f ? data.playerMoveSpeed : stats.MoveSpeed;

        stats.LoadState(data.playerMaxHP, data.playerHP, data.playerLevel, data.playerXP, data.gold, damage, attackSpeed, moveSpeed);
        if (restorePlayerPosition)
            RestorePlayerPosition(data, player);

        if (data.playerDashCooldown > 0f)
            player.LoadDashCooldown(data.playerDashCooldown);

        if (data.hasCurrentTool)
            player.GetComponent<PlayerToolHandler>()?.LoadTool(data.currentTool);

        var upgradeManager = ServiceLocator.Get<UpgradeManager>();
        if (upgradeManager == null) upgradeManager = FindObjectOfType<UpgradeManager>();
        upgradeManager?.LoadAppliedIds(data.appliedUpgradeIds, data.hasDragonArmor, data.hasDrone);

        farm?.LoadSaveData(data.tiles);

        var dayNightCycle = FindObjectOfType<DayNightCycle>();
        DayPhase savedPhase = ResolveSavedPhase(data);
        bool shouldStartNightWave = savedPhase != DayPhase.Night;
        dayNightCycle?.LoadState(data.currentDay, savedPhase, data.phaseRemaining, shouldStartNightWave);

        RestoreEnemies(data, waveManager);

        RestorePowerUps(data, player, powerUpSpawner);
    }

    private void SaveCurrentGameInternal()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "GameScene" && sceneName != "ShopInterior")
            return;

        PlayerStats stats = FindObjectOfType<PlayerStats>();
        PlayerController player = stats != null ? stats.GetComponent<PlayerController>() : FindObjectOfType<PlayerController>();
        if (stats == null || player == null) return;

        var upgradeManager = ServiceLocator.Get<UpgradeManager>();
        if (upgradeManager == null) upgradeManager = FindObjectOfType<UpgradeManager>();

        GameSaveData existingData = SaveSystem.Load();

        var appliedIds = upgradeManager != null
            ? upgradeManager.GetAppliedIds()
            : existingData?.appliedUpgradeIds ?? new System.Collections.Generic.List<string>();

        var farm = ServiceLocator.Get<FarmManager>();
        if (farm == null) farm = FindObjectOfType<FarmManager>();

        var dayNightCycle = FindObjectOfType<DayNightCycle>();
        var powerUpSpawner = FindObjectOfType<PowerUpSpawner>();
        var waveManager = ServiceLocator.Get<WaveManager>();
        if (waveManager == null) waveManager = FindObjectOfType<WaveManager>();
        var savedTiles = farm != null
            ? farm.GetSaveData()
            : existingData?.tiles ?? new System.Collections.Generic.List<TileSaveData>();

        int currentDay = dayNightCycle != null ? dayNightCycle.CurrentDay : existingData?.currentDay ?? 1;
        DayPhase currentPhase = dayNightCycle != null ? dayNightCycle.CurrentPhase : existingData?.currentPhase ?? DayPhase.Dawn;
        float phaseRemaining = dayNightCycle != null ? dayNightCycle.PhaseRemaining : existingData?.phaseRemaining ?? 0f;
        var droppedPowerUps = powerUpSpawner != null
            ? GetDroppedPowerUps()
            : existingData?.droppedPowerUps ?? new List<DroppedPowerUpSaveData>();
        var savedEnemies = waveManager != null
            ? GetLivingEnemies()
            : existingData?.enemies ?? new List<EnemySaveData>();

        bool preserveGameplayPosition = sceneName == "ShopInterior" && existingData != null;
        var data = new GameSaveData
        {
            playerHP = stats.CurrentHP,
            playerMaxHP = stats.MaxHP,
            hasPlayerPosition = preserveGameplayPosition ? existingData.hasPlayerPosition : true,
            playerX = preserveGameplayPosition ? existingData.playerX : player.transform.position.x,
            playerY = preserveGameplayPosition ? existingData.playerY : player.transform.position.y,
            playerZ = preserveGameplayPosition ? existingData.playerZ : player.transform.position.z,
            playerDamage = stats.Damage,
            playerAttackSpeed = stats.AttackSpeed,
            playerMoveSpeed = stats.MoveSpeed,
            playerDashCooldown = player.DashCooldown,
            hasCurrentTool = true,
            currentTool = player.GetComponent<PlayerToolHandler>()?.CurrentTool ?? ToolType.Gun,
            playerLevel = stats.Level,
            playerXP = stats.XP,
            gold = stats.Gold,
            appliedUpgradeIds = appliedIds,
            hasDragonArmor = appliedIds.Contains("upgrade_dragon_armor"),
            hasDrone = appliedIds.Contains("upgrade_drone") || HasActiveDrone(),
            tiles = savedTiles,
            activePowerUpBuffs = GetActivePowerUpBuffs(player, existingData),
            droppedPowerUps = droppedPowerUps,
            enemies = savedEnemies,
            currentDay = currentDay,
            hasDayNightState = dayNightCycle != null,
            currentPhase = currentPhase,
            currentPhaseIndex = (int)currentPhase,
            phaseRemaining = phaseRemaining,
            totalEnemiesKilled = existingData?.totalEnemiesKilled ?? 0,
            totalGoldEarned = existingData?.totalGoldEarned ?? 0
        };

        SaveSystem.Save(data);
    }

    private static bool IsGameplayScene(string sceneName)
    {
        return sceneName == "GameScene" || sceneName == "GameScene2";
    }

    private static DayPhase ResolveSavedPhase(GameSaveData data)
    {
        if (data.hasDayNightState)
            return (DayPhase)Mathf.Clamp(data.currentPhaseIndex, 0, 3);

        if (data.enemies != null && data.enemies.Count > 0)
            return DayPhase.Night;

        return data.currentPhase;
    }

    private static void RestoreEnemies(GameSaveData data, WaveManager waveManager)
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy != null && !enemy.IsDead)
                Destroy(enemy.gameObject);
        }

        if (waveManager == null || data.enemies == null)
            return;

        foreach (EnemySaveData saved in data.enemies)
            waveManager.SpawnSavedEnemy(saved.type, new Vector3(saved.x, saved.y, saved.z), saved.hp);

        waveManager.SetActiveEnemyCount(data.enemies.Count);
    }

    private static void RestorePlayerPosition(GameSaveData data, PlayerController player)
    {
        if (player == null || !data.hasPlayerPosition) return;

        player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = Vector2.zero;

        var cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow?.SetTarget(player.transform, snap: true);
    }

    private static void RestorePowerUps(GameSaveData data, PlayerController player, PowerUpSpawner spawner)
    {
        if (player == null) return;

        var buffs = player.GetComponent<PlayerPowerUpBuffs>();
        if (buffs == null)
            buffs = player.gameObject.AddComponent<PlayerPowerUpBuffs>();

        List<PowerUpData> powerUpDataSet = spawner != null ? spawner.GetPowerUpDataSet() : new List<PowerUpData>();
        buffs.Restore(data.activePowerUpBuffs, powerUpDataSet);

        foreach (PowerUp powerUp in FindObjectsOfType<PowerUp>())
        {
            if (powerUp != null && powerUp.CanBeSaved)
                Destroy(powerUp.gameObject);
        }

        if (spawner == null || data.droppedPowerUps == null) return;

        foreach (DroppedPowerUpSaveData dropped in data.droppedPowerUps)
            spawner.SpawnSaved(dropped.type, new Vector3(dropped.x, dropped.y, dropped.z));
    }

    private static List<ActivePowerUpBuffSaveData> GetActivePowerUpBuffs(PlayerController player, GameSaveData existingData)
    {
        var buffs = player != null ? player.GetComponent<PlayerPowerUpBuffs>() : null;
        if (buffs != null)
            return buffs.GetSaveData();

        return existingData?.activePowerUpBuffs ?? new List<ActivePowerUpBuffSaveData>();
    }

    private static List<DroppedPowerUpSaveData> GetDroppedPowerUps()
    {
        var data = new List<DroppedPowerUpSaveData>();
        foreach (PowerUp powerUp in FindObjectsOfType<PowerUp>())
        {
            if (powerUp == null || !powerUp.CanBeSaved) continue;

            Vector3 position = powerUp.transform.position;
            data.Add(new DroppedPowerUpSaveData
            {
                type = powerUp.Type,
                x = position.x,
                y = position.y,
                z = position.z
            });
        }

        return data;
    }

    private static List<EnemySaveData> GetLivingEnemies()
    {
        var data = new List<EnemySaveData>();
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy == null || enemy.IsDead || enemy.Data == null) continue;

            Vector3 position = enemy.transform.position;
            data.Add(new EnemySaveData
            {
                type = enemy.Data.enemyType,
                hp = enemy.CurrentHP,
                x = position.x,
                y = position.y,
                z = position.z
            });
        }

        return data;
    }

    private static bool HasActiveDrone()
    {
        foreach (Drone drone in FindObjectsOfType<Drone>(true))
        {
            if (drone != null && drone.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }
}
