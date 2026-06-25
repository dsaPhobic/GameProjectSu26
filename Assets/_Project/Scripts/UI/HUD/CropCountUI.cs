using TMPro;
using UnityEngine;

public class CropCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private FarmManager _farmManager;

    private void Start()
    {
        _farmManager = ServiceLocator.Get<FarmManager>();
        Refresh();
    }

    private void OnEnable()
    {
        GameEvents.OnDayPhaseChanged += _ => Refresh();
        GameEvents.OnWaveCompleted   += Refresh;
    }

    private void OnDisable()
    {
        GameEvents.OnDayPhaseChanged -= _ => Refresh();
        GameEvents.OnWaveCompleted   -= Refresh;
    }

    private void Refresh()
    {
        if (_text == null || _farmManager == null) return;
        _text.text = $"Crops: {_farmManager.CountLivingCrops()}";
    }
}
