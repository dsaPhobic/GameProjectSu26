using UnityEngine;

public class DragonArmorPassive : MonoBehaviour
{
    private const float HealthThreshold = 0.3f;
    private const float ShieldDuration = 15f;

    private PlayerStats _stats;
    private PlayerController _controller;
    private Sprite _shieldIcon;
    private bool _triggeredWhileLow;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _controller = GetComponent<PlayerController>();
    }

    public void Configure(Sprite shieldIcon)
    {
        _shieldIcon = shieldIcon != null
            ? shieldIcon
            : Resources.Load<Sprite>("UpgradeIcons/icon_upgrade_dragon_armor");
    }

    private void Update()
    {
        if (_stats == null || _controller == null || _stats.MaxHP <= 0) return;

        float hpRatio = (float)_stats.CurrentHP / _stats.MaxHP;
        if (hpRatio > HealthThreshold)
        {
            _triggeredWhileLow = false;
            return;
        }

        if (_triggeredWhileLow) return;

        _triggeredWhileLow = true;
        _controller.ActivateShield(ShieldDuration, _shieldIcon);
    }
}
