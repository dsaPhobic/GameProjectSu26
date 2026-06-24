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
    [SerializeField] private Color _nightColor = new Color(0.1f, 0.1f, 0.3f);

    [SerializeField] private float _dayIntensity = 1f;
    [SerializeField] private float _nightIntensity = 0.3f;

    private int _currentDay = 1;
    private WaveManager _waveManager;

    private void Start()
    {
        _waveManager = ServiceLocator.Get<WaveManager>();
        GameManager.Instance?.StartGame();
        if (_globalLight != null) _globalLight.color = _dayColor;
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
        PlayPhaseMusic(phase);

        if (phase == DayPhase.Night)
            _waveManager?.StartWave(_currentDay);

        float elapsed = 0f;
        Color startColor = _globalLight != null ? _globalLight.color : Color.white;
        float startIntensity = _globalLight != null ? _globalLight.intensity : 1f;
        float targetIntensity = (phase == DayPhase.Night || phase == DayPhase.Dusk) ? _nightIntensity : _dayIntensity;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (_globalLight != null)
            {
                _globalLight.color = Color.Lerp(startColor, targetColor, t);
                _globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            }
            yield return null;
        }
    }

    private void PlayPhaseMusic(DayPhase phase)
    {
        switch (phase)
        {
            case DayPhase.Night:
                AudioManager.Instance?.PlayBGM("bgm_night");
                break;
            case DayPhase.Dawn:
            case DayPhase.Day:
            case DayPhase.Dusk:
                AudioManager.Instance?.PlayBGM("bgm_day");
                break;
        }
    }
}
