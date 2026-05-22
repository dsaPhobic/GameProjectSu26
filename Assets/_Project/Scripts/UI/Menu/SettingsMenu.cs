using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _fullscreenToggle;

    private void Start()
    {
        if (_fullscreenToggle != null)
            _fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void OnBGMChanged(float value)
    {
        _mixer?.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void OnSFXChanged(float value)
    {
        _mixer?.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void OnFullscreenToggled(bool value)
    {
        Screen.fullScreen = value;
    }

    public void OnClosePressed()
    {
        gameObject.SetActive(false);
    }
}
