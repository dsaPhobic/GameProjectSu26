using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float _dawnDuration = 10f;
    [SerializeField] private float _dayDuration = 150f;
    [SerializeField] private float _duskDuration = 10f;
    [SerializeField] private float _nightDuration = 60f;

    [SerializeField] private Light2D _globalLight;
    [SerializeField] private Color _dawnColor = new Color(1f, 0.8f, 0.6f);
    [SerializeField] private Color _dayColor = Color.white;
    [SerializeField] private Color _duskColor = new Color(1f, 0.5f, 0.2f);
    [SerializeField] private Color _nightColor = new Color(0.1f, 0.1f, 0.3f);

    [SerializeField] private float _dayIntensity = 1f;
    [SerializeField] private float _nightIntensity = 0.3f;

    private int _currentDay = 1;
    private WaveManager _waveManager;
    private Coroutine _cycleCoroutine;
    private DayPhase _currentPhase = DayPhase.Dawn;
    private float _phaseRemaining;
    public int CurrentDay => _currentDay;
    public DayPhase CurrentPhase => _currentPhase;
    public float PhaseRemaining => _phaseRemaining;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene2")
        {
            _dayColor = new Color(0.72f, 0.86f, 1f);
            _dayDuration *= 0.75f;
            _nightDuration *= 1.25f;
        }

        _waveManager = ServiceLocator.Get<WaveManager>();
        GameManager.Instance?.StartGame();
        if (_cycleCoroutine != null) return;

        if (_globalLight != null) _globalLight.color = _dayColor;
        _cycleCoroutine = StartCoroutine(CycleRoutineFrom(DayPhase.Dawn, _dawnDuration, startNightWave: true));
    }

    private IEnumerator CycleRoutineFrom(DayPhase startPhase, float startRemaining, bool startNightWave)
    {
        DayPhase phase = startPhase;
        float remaining = startRemaining > 0f ? startRemaining : GetPhaseDuration(phase);
        bool isFirstPhase = true;

        while (true)
        {
            bool shouldStartNightWave = !isFirstPhase || startNightWave;
            yield return StartCoroutine(RunPhase(phase, remaining, GetPhaseColor(phase), shouldStartNightWave));

            if (phase == DayPhase.Night)
            {
                _currentDay++;
                GameEvents.RaiseDayChanged(_currentDay);
            }

            phase = GetNextPhase(phase);
            remaining = GetPhaseDuration(phase);
            isFirstPhase = false;
        }
    }

    private IEnumerator RunPhase(DayPhase phase, float duration, Color targetColor, bool startNightWave)
    {
        _currentPhase = phase;
        _phaseRemaining = duration;
        GameEvents.RaiseDayPhaseChanged(phase);
        PlayPhaseMusic(phase);

        if (phase == DayPhase.Night && startNightWave)
            _waveManager?.StartWave(_currentDay);

        float elapsed = 0f;
        Color startColor = _globalLight != null ? _globalLight.color : Color.white;
        float startIntensity = _globalLight != null ? _globalLight.intensity : 1f;
        float targetIntensity = (phase == DayPhase.Night || phase == DayPhase.Dusk) ? _nightIntensity : _dayIntensity;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _phaseRemaining = Mathf.Max(0f, duration - elapsed);
            float t = duration > 0f ? elapsed / duration : 1f;
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

    public void LoadDay(int day)
    {
        _currentDay = Mathf.Max(1, day);
        GameEvents.RaiseDayChanged(_currentDay);
    }

    public void LoadState(int day, DayPhase phase, float phaseRemaining, bool startNightWave)
    {
        _waveManager = ServiceLocator.Get<WaveManager>();
        _currentDay = Mathf.Max(1, day);
        GameEvents.RaiseDayChanged(_currentDay);

        if (_cycleCoroutine != null)
            StopCoroutine(_cycleCoroutine);

        ApplyPhaseVisual(phase, phaseRemaining);
        _cycleCoroutine = StartCoroutine(CycleRoutineFrom(phase, phaseRemaining, startNightWave));
    }

    private void ApplyPhaseVisual(DayPhase phase, float phaseRemaining)
    {
        if (_globalLight == null) return;

        float duration = Mathf.Max(0.01f, GetPhaseDuration(phase));
        float remaining = phaseRemaining > 0f ? Mathf.Min(phaseRemaining, duration) : duration;
        float elapsed = Mathf.Clamp(duration - remaining, 0f, duration);
        float progress = elapsed / duration;

        DayPhase previousPhase = GetPreviousPhase(phase);
        _globalLight.color = Color.Lerp(GetPhaseColor(previousPhase), GetPhaseColor(phase), progress);
        _globalLight.intensity = Mathf.Lerp(GetPhaseIntensity(previousPhase), GetPhaseIntensity(phase), progress);
    }

    private float GetPhaseDuration(DayPhase phase)
    {
        return phase switch
        {
            DayPhase.Dawn => _dawnDuration,
            DayPhase.Day => _dayDuration,
            DayPhase.Dusk => _duskDuration,
            DayPhase.Night => _nightDuration,
            _ => _dayDuration
        };
    }

    private Color GetPhaseColor(DayPhase phase)
    {
        return phase switch
        {
            DayPhase.Dawn => _dawnColor,
            DayPhase.Day => _dayColor,
            DayPhase.Dusk => _duskColor,
            DayPhase.Night => _nightColor,
            _ => _dayColor
        };
    }

    private DayPhase GetNextPhase(DayPhase phase)
    {
        return phase switch
        {
            DayPhase.Dawn => DayPhase.Day,
            DayPhase.Day => DayPhase.Dusk,
            DayPhase.Dusk => DayPhase.Night,
            DayPhase.Night => DayPhase.Dawn,
            _ => DayPhase.Day
        };
    }

    private DayPhase GetPreviousPhase(DayPhase phase)
    {
        return phase switch
        {
            DayPhase.Dawn => DayPhase.Night,
            DayPhase.Day => DayPhase.Dawn,
            DayPhase.Dusk => DayPhase.Day,
            DayPhase.Night => DayPhase.Dusk,
            _ => DayPhase.Day
        };
    }

    private float GetPhaseIntensity(DayPhase phase)
    {
        return (phase == DayPhase.Night || phase == DayPhase.Dusk) ? _nightIntensity : _dayIntensity;
    }
}
