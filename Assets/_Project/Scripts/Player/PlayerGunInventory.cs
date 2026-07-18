using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunInventory : MonoBehaviour
{
    private static readonly HashSet<string> UnlockedGunIds = new();
    private static string _equippedGunId;
    private static bool _initialized;

    private readonly List<GunData> _catalog = new();
    private PlayerToolHandler _toolHandler;

    public GunData CurrentGun { get; private set; }
    public IReadOnlyList<GunData> Catalog => _catalog;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        ResetProgress();
    }

    public static void ResetProgress()
    {
        UnlockedGunIds.Clear();
        _equippedGunId = null;
        _initialized = false;
    }

    private void Awake()
    {
        _toolHandler = GetComponent<PlayerToolHandler>();
        LoadCatalog();
        InitializeDefaultUnlocks();
        RestoreEquippedGun();
    }

    private void LoadCatalog()
    {
        _catalog.Clear();
        _catalog.AddRange(Resources.LoadAll<GunData>("Guns")
            .Where(gun => gun != null && !string.IsNullOrWhiteSpace(gun.id))
            .OrderBy(gun => gun.price));
    }

    private void InitializeDefaultUnlocks()
    {
        if (_initialized) return;

        foreach (GunData gun in _catalog)
        {
            if (gun.unlockedByDefault)
                UnlockedGunIds.Add(gun.id);
        }

        GunData startingGun = _catalog.FirstOrDefault(gun => gun.unlockedByDefault) ??
                              _catalog.FirstOrDefault();
        _equippedGunId = startingGun != null ? startingGun.id : null;
        _initialized = true;
    }

    private void RestoreEquippedGun()
    {
        GunData gun = FindGun(_equippedGunId) ??
                      _catalog.FirstOrDefault(item => UnlockedGunIds.Contains(item.id));
        EquipInternal(gun, equipGunTool: false);
    }

    public bool IsUnlocked(GunData gun)
    {
        return gun != null && UnlockedGunIds.Contains(gun.id);
    }

    public bool UnlockAndEquip(GunData gun)
    {
        if (gun == null) return false;

        UnlockedGunIds.Add(gun.id);
        Equip(gun);
        return true;
    }

    public bool Equip(GunData gun)
    {
        return EquipInternal(gun, equipGunTool: true);
    }

    private bool EquipInternal(GunData gun, bool equipGunTool)
    {
        if (gun == null || !IsUnlocked(gun)) return false;

        CurrentGun = gun;
        _equippedGunId = gun.id;
        _toolHandler?.SetGunSprite(gun.icon);
        if (equipGunTool)
            _toolHandler?.EquipGun();
        GameEvents.RaiseGunEquipped(gun);
        return true;
    }

    public void EquipNextUnlocked()
    {
        List<GunData> unlocked = _catalog.Where(IsUnlocked).ToList();
        if (unlocked.Count == 0) return;

        int currentIndex = unlocked.IndexOf(CurrentGun);
        int nextIndex = (currentIndex + 1) % unlocked.Count;
        Equip(unlocked[nextIndex]);
    }

    public GunData FindGun(string id)
    {
        return _catalog.FirstOrDefault(gun => gun.id == id);
    }

    public List<string> GetUnlockedIds()
    {
        return UnlockedGunIds.ToList();
    }

    public string GetEquippedId()
    {
        return _equippedGunId;
    }

    public void LoadState(List<string> unlockedIds, string equippedId)
    {
        UnlockedGunIds.Clear();
        if (unlockedIds != null)
        {
            foreach (string id in unlockedIds)
            {
                if (!string.IsNullOrWhiteSpace(id))
                    UnlockedGunIds.Add(id);
            }
        }

        foreach (GunData gun in _catalog)
        {
            if (gun.unlockedByDefault)
                UnlockedGunIds.Add(gun.id);
        }

        _equippedGunId = equippedId;
        _initialized = true;
        RestoreEquippedGun();
    }
}
