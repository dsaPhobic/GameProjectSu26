using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private float _fadeTime = 1f;

    [System.Serializable]
    public struct AudioEntry
    {
        public string key;
        public AudioClip clip;
    }

    [SerializeField] private List<AudioEntry> _bgmList;
    [SerializeField] private List<AudioEntry> _sfxList;

    private Dictionary<string, AudioClip> _bgm;
    private Dictionary<string, AudioClip> _sfx;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _bgm = new Dictionary<string, AudioClip>();
        _sfx = new Dictionary<string, AudioClip>();
        foreach (var e in _bgmList) _bgm[e.key] = e.clip;
        foreach (var e in _sfxList) _sfx[e.key] = e.clip;
    }

    public void PlayBGM(string key, bool fade = true)
    {
        if (!_bgm.TryGetValue(key, out var clip)) return;
        if (_bgmSource.clip == clip) return;
        if (fade) StartCoroutine(FadeBGM(clip));
        else { _bgmSource.clip = clip; _bgmSource.Play(); }
    }

    public void PlaySFX(string key, float volume = 1f)
    {
        if (_sfx.TryGetValue(key, out var clip))
            _sfxSource.PlayOneShot(clip, volume);
    }

    private IEnumerator FadeBGM(AudioClip newClip)
    {
        float startVol = _bgmSource.volume;
        for (float t = 0; t < _fadeTime; t += Time.unscaledDeltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(startVol, 0, t / _fadeTime);
            yield return null;
        }
        _bgmSource.clip = newClip;
        _bgmSource.Play();
        for (float t = 0; t < _fadeTime; t += Time.unscaledDeltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(0, startVol, t / _fadeTime);
            yield return null;
        }
        _bgmSource.volume = startVol;
    }
}
