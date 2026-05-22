using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;

    private UpgradeData _data;
    private System.Action<UpgradeData> _onSelected;

    public void Init(UpgradeData data, System.Action<UpgradeData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;
        if (_nameText != null) _nameText.text = data.displayName;
        if (_descText != null) _descText.text = data.description;
        if (_icon != null && data.icon != null) _icon.sprite = data.icon;
        _button?.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        _onSelected?.Invoke(_data);
    }
}
