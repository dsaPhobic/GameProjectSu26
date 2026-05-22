using UnityEngine;
using UnityEngine.UI;

public class PowerUpTimerUI : MonoBehaviour
{
    [SerializeField] private Image _fillBar;
    [SerializeField] private GameObject _panel;

    private float _duration;
    private float _elapsed;
    private bool _active;

    public void ShowTimer(float duration)
    {
        _duration = duration;
        _elapsed = 0f;
        _active = true;
        if (_panel != null) _panel.SetActive(true);
    }

    private void Update()
    {
        if (!_active) return;
        _elapsed += Time.deltaTime;
        if (_fillBar != null) _fillBar.fillAmount = 1f - (_elapsed / _duration);
        if (_elapsed >= _duration)
        {
            _active = false;
            if (_panel != null) _panel.SetActive(false);
        }
    }
}
