using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
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
        var player = ServiceLocator.Get<PlayerController>();
        if (player != null)
        {
            _stats = player.GetComponent<PlayerStats>();
            _animator = player.GetComponent<PlayerAnimator>();
            _drone = player.GetComponentInChildren<Drone>(true);
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
        }

        AudioManager.Instance?.PlaySFX("sfx_levelup");
        GameManager.Instance?.ResumeGame();
    }

    public List<string> GetAppliedIds() => _appliedIds;
}
