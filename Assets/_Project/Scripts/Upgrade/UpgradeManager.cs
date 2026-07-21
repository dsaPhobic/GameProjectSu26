using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private const string DroneDamageId = "upgrade_drone_damage";
    private const int DroneDamageBonus = 10;

    private PlayerController _player;
    private PlayerStats _stats;
    private PlayerAnimator _animator;
    private Drone _drone;

    private readonly List<string> _appliedIds = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        ResolvePlayer();
    }

    private void ResolvePlayer()
    {
        _player = ServiceLocator.Get<PlayerController>();
        if (_player != null)
        {
            _stats = _player.GetComponent<PlayerStats>();
            _animator = _player.GetComponent<PlayerAnimator>();
            _drone = _player.GetComponentInChildren<Drone>(true);
        }
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<UpgradeManager>();
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;
        if (_player == null || _stats == null)
            ResolvePlayer();

        if (_appliedIds.Contains(upgrade.id)) return;
        _appliedIds.Add(upgrade.id);

        switch (upgrade.effect)
        {
            case UpgradeEffect.IncreaseDamage:
                _stats?.ModifyDamage(Mathf.RoundToInt(upgrade.value));
                break;
            case UpgradeEffect.IncreaseAttackSpeed:
                _stats?.ModifyAttackSpeed(upgrade.value);
                break;
            case UpgradeEffect.IncreaseMaxHP:
                _stats?.ModifyMaxHP(Mathf.RoundToInt(upgrade.value));
                break;
            case UpgradeEffect.IncreaseMoveSpeed:
                _stats?.ModifyMoveSpeed(upgrade.value);
                break;
            case UpgradeEffect.UnlockDragonArmor:
                _animator?.SetArmored(true);
                EnsureDragonArmorPassive(upgrade.icon);
                break;
            case UpgradeEffect.UnlockDrone:
                _drone?.gameObject.SetActive(true);
                break;
            case UpgradeEffect.ReduceCooldown:
                _player?.ReduceDashCooldown(upgrade.value);
                break;
            case UpgradeEffect.IncreaseDroneDamage:
                ApplyDroneDamage(Mathf.RoundToInt(upgrade.value));
                break;
        }

        AudioManager.Instance?.PlaySFX("sfx_levelup");
        GameManager.Instance?.ResumeGame();
    }

    public void LoadAppliedIds(List<string> ids, bool hasDragonArmor, bool hasDrone)
    {
        _appliedIds.Clear();
        if (ids != null)
            _appliedIds.AddRange(ids);

        if (_player == null || _stats == null)
            ResolvePlayer();

        _animator?.SetArmored(hasDragonArmor);
        if (hasDragonArmor)
            EnsureDragonArmorPassive(null);

        if (_drone != null)
            _drone.gameObject.SetActive(hasDrone);

        if (_appliedIds.Contains(DroneDamageId))
            ApplyDroneDamage(DroneDamageBonus);
    }

    public List<string> GetAppliedIds() => new(_appliedIds);

    public bool CanOfferUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return false;
        if (_appliedIds.Contains(upgrade.id)) return false;

        if (_player == null || _stats == null)
            ResolvePlayer();

        return upgrade.effect switch
        {
            UpgradeEffect.UnlockDrone => !HasActiveDrone(),
            UpgradeEffect.IncreaseDroneDamage => HasActiveDrone(),
            _ => true
        };
    }

    private bool HasActiveDrone()
    {
        foreach (Drone drone in FindObjectsOfType<Drone>(true))
        {
            if (drone != null && drone.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }

    private void EnsureDragonArmorPassive(Sprite icon)
    {
        if (_player == null)
            ResolvePlayer();

        if (_player == null) return;

        var passive = _player.GetComponent<DragonArmorPassive>();
        if (passive == null)
            passive = _player.gameObject.AddComponent<DragonArmorPassive>();

        passive.Configure(icon);
    }

    private void ApplyDroneDamage(int bonusDamage)
    {
        foreach (Drone drone in FindObjectsOfType<Drone>(true))
            drone?.ModifyDamage(bonusDamage);
    }
}
