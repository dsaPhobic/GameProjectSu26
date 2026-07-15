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

    public static UpgradeCard BuildRuntime(GameObject cardObject)
    {
        cardObject.layer = LayerMask.NameToLayer("UI");

        var rect = cardObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(280f, 360f);

        var image = cardObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = new Color(0.16f, 0.16f, 0.22f, 1f);

        var button = cardObject.AddComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.highlightedColor = new Color(0.28f, 0.28f, 0.4f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        button.colors = colors;

        var card = cardObject.AddComponent<UpgradeCard>();
        card._button = button;
        card._nameText = NewText("Name", cardObject.transform, 30f, TextAlignmentOptions.Center, "");
        card._nameText.fontStyle = FontStyles.Bold;
        var nameRect = card._nameText.rectTransform;
        nameRect.anchorMin = new Vector2(0f, 1f);
        nameRect.anchorMax = new Vector2(1f, 1f);
        nameRect.pivot = new Vector2(0.5f, 1f);
        nameRect.anchoredPosition = new Vector2(0f, -20f);
        nameRect.sizeDelta = new Vector2(-20f, 64f);

        card._icon = NewImage("Icon", cardObject.transform, Color.white);
        card._icon.preserveAspect = true;
        var iconRect = card._icon.rectTransform;
        iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0.5f, 1f);
        iconRect.anchoredPosition = new Vector2(0f, -96f);
        iconRect.sizeDelta = new Vector2(96f, 96f);

        card._descText = NewText("Desc", cardObject.transform, 22f, TextAlignmentOptions.Center, "");
        card._descText.enableWordWrapping = true;
        var descRect = card._descText.rectTransform;
        descRect.anchorMin = new Vector2(0f, 0f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.offsetMin = new Vector2(14f, 64f);
        descRect.offsetMax = new Vector2(-14f, -196f);

        var selectLabel = NewText("SelectLabel", cardObject.transform, 24f, TextAlignmentOptions.Center, "SELECT");
        selectLabel.fontStyle = FontStyles.Bold;
        selectLabel.color = new Color(1f, 0.88f, 0.3f);
        var selectRect = selectLabel.rectTransform;
        selectRect.anchorMin = new Vector2(0f, 0f);
        selectRect.anchorMax = new Vector2(1f, 0f);
        selectRect.pivot = new Vector2(0.5f, 0f);
        selectRect.anchoredPosition = new Vector2(0f, 16f);
        selectRect.sizeDelta = new Vector2(-20f, 40f);

        return card;
    }

    public void Init(UpgradeData data, System.Action<UpgradeData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;
        if (_nameText != null) _nameText.text = data.displayName;
        if (_descText != null) _descText.text = data.description;
        if (_icon != null && data.icon != null) _icon.sprite = data.icon;
        if (_button != null) _button.onClick.RemoveAllListeners();
        _button?.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        _onSelected?.Invoke(_data);
    }

    private static Image NewImage(string objectName, Transform parent, Color color)
    {
        var imageObject = new GameObject(objectName);
        imageObject.layer = LayerMask.NameToLayer("UI");
        imageObject.transform.SetParent(parent, false);
        imageObject.AddComponent<RectTransform>();

        var image = imageObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI NewText(string objectName, Transform parent, float fontSize, TextAlignmentOptions alignment, string text)
    {
        var textObject = new GameObject(objectName);
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.transform.SetParent(parent, false);
        textObject.AddComponent<RectTransform>();

        var tmp = textObject.AddComponent<TextMeshProUGUI>();
        if (tmp.font == null) tmp.font = TMP_Settings.defaultFontAsset;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }

    private static Sprite _whiteSprite;
    private static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var texture = Texture2D.whiteTexture;
                _whiteSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            }

            return _whiteSprite;
        }
    }
}
