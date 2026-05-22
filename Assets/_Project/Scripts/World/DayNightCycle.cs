using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float _dawnDuration = 15f;
    [SerializeField] private float _dayDuration = 90f;
    [SerializeField] private float _duskDuration = 15f;
    [SerializeField] private float _nightDuration = 120f;

    [SerializeField] private Light2D _globalLight;
    [SerializeField] private Color _dawnColor = new Color(1f, 0.8f, 0.6f);
    [SerializeField] private Color _dayColor = Color.white;
    [SerializeField] private Color _duskColor = new Color(1f, 0.5f, 0.2f);
    [SerializeField] private Color _nightColor = new Color(0.2f, 0.2f, 0.5f);

    private int _currentDay = 1;
    private WaveManager _waveManager;

    private void Start()
    {
        _waveManager = ServiceLocator.Get<WaveManager>();
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
        Color startColor = _globalLight != null ? _globalLight.color : Color.white;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (_globalLight != null)
                _globalLight.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            yield return null;
        }
    }
}
