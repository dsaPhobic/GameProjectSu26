using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private Image _xpBar;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _dayText;

    private PlayerStats _stats;

    private void Start()
    {
        var player = ServiceLocator.Get<PlayerController>();
        if (player != null) _stats = player.GetComponent<PlayerStats>();

        GameEvents.OnPlayerHPChanged += UpdateHP;
        GameEvents.OnPlayerXPChanged += UpdateXP;
        GameEvents.OnGoldChanged += UpdateGold;
        GameEvents.OnDayChanged += UpdateDay;

        RefreshAll();
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerHPChanged -= UpdateHP;
        GameEvents.OnPlayerXPChanged -= UpdateXP;
        GameEvents.OnGoldChanged -= UpdateGold;
        GameEvents.OnDayChanged -= UpdateDay;
    }

    private void RefreshAll()
    {
        if (_stats == null) return;
        UpdateHP(_stats.CurrentHP);
        UpdateXP(_stats.XP);
        UpdateGold(_stats.Gold);
    }

    private void UpdateHP(int hp)
    {
        if (_hpBar != null && _stats != null)
            _hpBar.fillAmount = (float)hp / _stats.MaxHP;
    }

    private void UpdateXP(int xp)
    {
        if (_xpBar != null && _stats != null)
        {
            int needed = 100 * _stats.Level;
            _xpBar.fillAmount = (float)xp / needed;
        }
    }

    private void UpdateGold(int gold)
    {
        if (_goldText != null) _goldText.text = $"Gold: {gold}";
    }

    private void UpdateDay(int day)
    {
        if (_dayText != null) _dayText.text = $"Day {day}";
    }
}
