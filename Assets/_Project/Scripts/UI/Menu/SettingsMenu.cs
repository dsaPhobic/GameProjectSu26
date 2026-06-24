using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private const string BGMVolumePref = "settings_bgm_volume";
    private const string SFXVolumePref = "settings_sfx_volume";
    private const string FullscreenPref = "settings_fullscreen";

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _fullscreenToggle;

    private bool _initialized;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        SyncControlsFromPrefs();
    }

    public void Initialize()
    {
        if (_initialized) return;

        FindControlsIfNeeded();
        WireControlEvents();
        SyncControlsFromPrefs();
        _initialized = true;
    }

    public void OnBGMChanged(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BGMVolumePref, value);
        ApplyBGMVolume(value);
    }

    public void OnSFXChanged(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFXVolumePref, value);
        ApplySFXVolume(value);
    }

    public void OnFullscreenToggled(bool value)
    {
        PlayerPrefs.SetInt(FullscreenPref, value ? 1 : 0);
        Screen.fullScreen = value;
    }

    public void OnClosePressed()
    {
        gameObject.SetActive(false);
    }

    private void FindControlsIfNeeded()
    {
        _bgmSlider ??= FindInRow<Slider>("Row_BGM");
        _sfxSlider ??= FindInRow<Slider>("Row_SFX");
        _fullscreenToggle ??= FindInRow<Toggle>("Row_Fullscreen");
    }

    private void WireControlEvents()
    {
        if (_bgmSlider != null)
        {
            _bgmSlider.onValueChanged.RemoveListener(OnBGMChanged);
            _bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
            _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }

        if (_fullscreenToggle != null)
        {
            _fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggled);
            _fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }
    }

    private void SyncControlsFromPrefs()
    {
        float bgm = PlayerPrefs.GetFloat(BGMVolumePref, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SFXVolumePref, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenPref, Screen.fullScreen ? 1 : 0) == 1;

        if (_bgmSlider != null) _bgmSlider.SetValueWithoutNotify(bgm);
        if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(sfx);
        if (_fullscreenToggle != null) _fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        ApplyBGMVolume(bgm);
        ApplySFXVolume(sfx);
        Screen.fullScreen = fullscreen;
    }

    private void ApplyBGMVolume(float value)
    {
        _mixer?.SetFloat("BGMVolume", ToDecibels(value));
        AudioManager.Instance?.SetBGMVolume(value);
    }

    private void ApplySFXVolume(float value)
    {
        _mixer?.SetFloat("SFXVolume", ToDecibels(value));
        AudioManager.Instance?.SetSFXVolume(value);
    }

    private static float ToDecibels(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }

    private T FindInRow<T>(string rowName) where T : Component
    {
        Transform row = FindChildRecursive(transform, rowName);
        return row != null ? row.GetComponentInChildren<T>(true) : null;
    }

    private static Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;

            Transform nested = FindChildRecursive(child, childName);
            if (nested != null) return nested;
        }

        return null;
    }
}
