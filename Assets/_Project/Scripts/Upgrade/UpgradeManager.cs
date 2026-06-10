using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
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
                break;
            case UpgradeEffect.UnlockDrone:
                _drone?.gameObject.SetActive(true);
                break;
            case UpgradeEffect.ReduceCooldown:
                _player?.ReduceDashCooldown(upgrade.value);
                break;
        }

        AudioManager.Instance?.PlaySFX("sfx_levelup");
        GameManager.Instance?.ResumeGame();
    }

    public List<string> GetAppliedIds() => _appliedIds;
}
