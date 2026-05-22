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
}

[Serializable]
public class GameSaveData
{
    public int playerHP;
    public int playerMaxHP;
    public int playerLevel;
    public int playerXP;
    public int gold;
    public List<string> appliedUpgradeIds = new();
    public bool hasDragonArmor;
    public bool hasDrone;
    public List<TileSaveData> tiles = new();
    public int currentDay;
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
