using TMPro;
using UnityEngine;

public class DayCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable() => GameEvents.OnDayChanged += OnDayChanged;
    private void OnDisable() => GameEvents.OnDayChanged -= OnDayChanged;

    private void OnDayChanged(int day)
    {
        if (_text != null) _text.text = $"Day {day}";
    }
}
