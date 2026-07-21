using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class TileSaveData
{
    public int x;
    public int y;
    public TileState state;
    public CropType cropType;
    public int cropStage;
    public int cropHP;
}

[Serializable]
public class ActivePowerUpBuffSaveData
{
    public PowerUpType type;
    public float remaining;
    public float duration;
    public float magnitude;
    public int appliedIntAmount;
    public float appliedFloatAmount;
}

[Serializable]
public class DroppedPowerUpSaveData
{
    public PowerUpType type;
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class EnemySaveData
{
    public EnemyType type;
    public int hp;
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class GameSaveData
{
    public bool hasPlayerPosition;
    public float playerX;
    public float playerY;
    public float playerZ;
    public int playerHP;
    public int playerMaxHP;
    public int playerDamage;
    public float playerAttackSpeed;
    public float playerMoveSpeed;
    public float playerDashCooldown;
    public bool hasCurrentTool;
    public ToolType currentTool;
    public List<string> unlockedGunIds = new();
    public string equippedGunId;
    public int playerLevel;
    public int playerXP;
    public int gold;
    public List<string> appliedUpgradeIds = new();
    public bool hasDragonArmor;
    public bool hasDrone;
    public List<TileSaveData> tiles = new();
    public List<ActivePowerUpBuffSaveData> activePowerUpBuffs = new();
    public List<DroppedPowerUpSaveData> droppedPowerUps = new();
    public List<EnemySaveData> enemies = new();
    public int currentDay;
    public bool hasDayNightState;
    public DayPhase currentPhase;
    public int currentPhaseIndex;
    public float phaseRemaining;
    public float dayTimer;
    public int totalEnemiesKilled;
    public int totalGoldEarned;
    public string saveTime;
}

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(GameSaveData data)
    {
        data.saveTime = DateTime.Now.ToString("o");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(SavePath)) return null;
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }

    public static bool HasSave() => File.Exists(SavePath);
}
