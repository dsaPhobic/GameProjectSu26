using TMPro;
using UnityEngine;

public class WaveIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _panel;

    private void OnEnable()
    {
        GameEvents.OnWaveStarted += OnWaveStarted;
        GameEvents.OnWaveCompleted += OnWaveCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnWaveStarted -= OnWaveStarted;
        GameEvents.OnWaveCompleted -= OnWaveCompleted;
    }

    private void OnWaveStarted(int wave)
    {
        if (_panel != null) _panel.SetActive(true);
        if (_text != null) _text.text = $"Wave {wave}!";
    }

    private void OnWaveCompleted()
    {
        if (_panel != null) _panel.SetActive(false);
    }
}
