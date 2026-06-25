using TMPro;
using UnityEngine;

public class DayPhaseIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private static readonly Color _dawnColor  = new Color(1f, 0.8f, 0.6f);
    private static readonly Color _dayColor   = Color.white;
    private static readonly Color _duskColor  = new Color(1f, 0.5f, 0.2f);
    private static readonly Color _nightColor = new Color(0.5f, 0.6f, 1f);

    private void OnEnable()  => GameEvents.OnDayPhaseChanged += OnPhaseChanged;
    private void OnDisable() => GameEvents.OnDayPhaseChanged -= OnPhaseChanged;

    private void OnPhaseChanged(DayPhase phase)
    {
        if (_text == null) return;
        _text.text = phase switch
        {
            DayPhase.Dawn  => "Dawn",
            DayPhase.Day   => "Day",
            DayPhase.Dusk  => "Dusk",
            DayPhase.Night => "Night",
            _              => string.Empty
        };
        _text.color = phase switch
        {
            DayPhase.Dawn  => _dawnColor,
            DayPhase.Day   => _dayColor,
            DayPhase.Dusk  => _duskColor,
            DayPhase.Night => _nightColor,
            _              => Color.white
        };
    }
}
