using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private UpgradeCard _cardPrefab;
    [SerializeField] private RectTransform _cardContainer;

    private UpgradeManager _upgradeManager;
    private readonly List<UpgradeCard> _cards = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
        BuildRuntimeUIIfMissing();
        if (_panel != null) _panel.SetActive(false);
    }

    private void Start()
    {
        _upgradeManager = ServiceLocator.Get<UpgradeManager>();
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<UpgradeScreen>();
    }

    public void Show(List<UpgradeData> upgrades)
    {
        foreach (var card in _cards) Destroy(card.gameObject);
        _cards.Clear();

        if (upgrades == null || upgrades.Count == 0)
        {
            GameManager.Instance?.ResumeGame();
            return;
        }

        foreach (var upgrade in upgrades)
        {
            var card = Instantiate(_cardPrefab, _cardContainer);
            card.gameObject.SetActive(true);
            card.Init(upgrade, OnCardSelected);
            _cards.Add(card);
        }

        if (_panel != null) _panel.SetActive(true);
        GameManager.Instance?.OpenLevelUp();
    }

    private void OnCardSelected(UpgradeData upgrade)
    {
        _upgradeManager?.ApplyUpgrade(upgrade);
        if (_panel != null) _panel.SetActive(false);
    }

    private void BuildRuntimeUIIfMissing()
    {
        if (_panel != null && _cardPrefab != null && _cardContainer != null) return;

        EnsureEventSystem();

        var canvasObject = new GameObject("UpgradeCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvasObject.layer = LayerMask.NameToLayer("UI");

        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 220;

        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObject.AddComponent<GraphicRaycaster>();

        var panelImage = NewImage("Panel", canvasObject.transform, new Color(0f, 0f, 0f, 0.58f));
        _panel = panelImage.gameObject;
        Stretch(panelImage.rectTransform);

        var title = NewText("Title", _panel.transform, 54, TextAlignmentOptions.Center, "CHOOSE UPGRADE");
        title.fontStyle = FontStyles.Bold;
        var titleRect = title.rectTransform;
        titleRect.anchorMin = titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -110f);
        titleRect.sizeDelta = new Vector2(900f, 80f);

        var container = new GameObject("Container_Cards");
        container.transform.SetParent(_panel.transform, false);
        _cardContainer = container.AddComponent<RectTransform>();
        _cardContainer.anchorMin = _cardContainer.anchorMax = _cardContainer.pivot = new Vector2(0.5f, 0.5f);
        _cardContainer.anchoredPosition = new Vector2(0f, -20f);
        _cardContainer.sizeDelta = new Vector2(920f, 380f);

        var layout = container.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        layout.spacing = 40f;

        _cardPrefab = CreateRuntimeCardPrefab();
    }

    private UpgradeCard CreateRuntimeCardPrefab()
    {
        var cardObject = new GameObject("UpgradeCard_RuntimePrefab");
        cardObject.SetActive(false);
        cardObject.transform.SetParent(transform, false);
        return UpgradeCard.BuildRuntime(cardObject);
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static Image NewImage(string objectName, Transform parent, Color color)
    {
        var imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);
        imageObject.layer = LayerMask.NameToLayer("UI");

        var rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

        var image = imageObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = color;
        return image;
    }

    private static TextMeshProUGUI NewText(string objectName, Transform parent, float fontSize, TextAlignmentOptions alignment, string text)
    {
        var textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);
        textObject.layer = LayerMask.NameToLayer("UI");
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
