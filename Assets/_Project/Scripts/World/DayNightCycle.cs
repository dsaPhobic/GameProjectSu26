using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float _dawnDuration = 15f;
    [SerializeField] private float _dayDuration = 90f;
    [SerializeField] private float _duskDuration = 15f;
    [SerializeField] private float _nightDuration = 120f;

    [SerializeField] private Image _overlayImage;
    [SerializeField] private Color _dawnColor = new Color(1f, 0.5f, 0f, 0.3f);
    [SerializeField] private Color _dayColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private Color _duskColor = new Color(0.8f, 0.3f, 0f, 0.35f);
    [SerializeField] private Color _nightColor = new Color(0f, 0f, 0.3f, 0.65f);

    private int _currentDay = 1;
    private WaveManager _waveManager;

    private void Start()
    {
        _waveManager = ServiceLocator.Get<WaveManager>();
        GameManager.Instance?.StartGame();
        if (_overlayImage != null) _overlayImage.color = _dayColor;
        StartCoroutine(CycleRoutine());
    }

    private IEnumerator CycleRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(RunPhase(DayPhase.Dawn, _dawnDuration, _dawnColor));
            yield return StartCoroutine(RunPhase(DayPhase.Day, _dayDuration, _dayColor));
            yield return StartCoroutine(RunPhase(DayPhase.Dusk, _duskDuration, _duskColor));
            yield return StartCoroutine(RunPhase(DayPhase.Night, _nightDuration, _nightColor));
            _currentDay++;
            GameEvents.RaiseDayChanged(_currentDay);
        }
    }

    private IEnumerator RunPhase(DayPhase phase, float duration, Color targetColor)
    {
        GameEvents.RaiseDayPhaseChanged(phase);

        if (phase == DayPhase.Night)
            _waveManager?.StartWave(_currentDay);

        float elapsed = 0f;
        Color startColor = _overlayImage != null ? _overlayImage.color : Color.clear;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (_overlayImage != null)
                _overlayImage.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            yield return null;
        }
    }
}
