using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopkeeperNPC : MonoBehaviour, IInteractable
{
    public static bool IsPlayerShopping { get; private set; }

    private enum ShopCategory
    {
        Eggs,
        Guns
    }

    [Header("Shop")]
    [SerializeField] private PetEggData[] _eggsForSale;
    [SerializeField] private int _selectedEggIndex;
    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private KeyCode _buyKey = KeyCode.E;
    [SerializeField] private KeyCode _cycleEggKey = KeyCode.Q;
    [SerializeField] private KeyCode _categoryKey = KeyCode.Tab;

    [Header("UI Theme")]
    [SerializeField] private Color _panelColor = new Color(0.055f, 0.07f, 0.11f, 0.97f);
    [SerializeField] private Color _accentColor = new Color(0.96f, 0.7f, 0.22f, 1f);
    [SerializeField] private Color _successColor = new Color(0.3f, 0.92f, 0.55f, 1f);
    [SerializeField] private Color _errorColor = new Color(1f, 0.32f, 0.3f, 1f);

    private PlayerToolHandler _nearbyPlayer;
    private PlayerStats _playerStats;
    private PlayerGunInventory _gunInventory;
    private GunData[] _gunsForSale;
    private ShopCategory _category;
    private int _selectedGunIndex;

    private GameObject _shopUIRoot;
    private RectTransform _panelRect;
    private CanvasGroup _panelGroup;
    private Image _eggIcon;
    private TextMeshProUGUI _eggNameText;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _priceText;
    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _statusText;
    private TextMeshProUGUI _pageText;

    private bool _panelRequested;
    private float _panelVisibility;
    private float _messageUntil;
    private string _temporaryMessage;
    private Color _temporaryMessageColor;
    private Vector2 _panelShakeOffset;

    private static Sprite _whiteSprite;
    private static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                Texture2D texture = Texture2D.whiteTexture;
                _whiteSprite = Sprite.Create(texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100f);
            }

            return _whiteSprite;
        }
    }

    private void Awake()
    {
        _gunsForSale = Resources.LoadAll<GunData>("Guns");
        System.Array.Sort(_gunsForSale, (left, right) => left.price.CompareTo(right.price));
        BuildShopUI();
    }

    private void Start()
    {
        ResolvePlayerStats();
        RefreshShopUI();
    }

    private void Update()
    {
        _nearbyPlayer = FindNearbyPlayer();
        _panelRequested = _nearbyPlayer != null;
        IsPlayerShopping = _panelRequested;

        if (_playerStats == null || _gunInventory == null)
            ResolvePlayerStats();

        AnimatePanel();
        if (!_panelRequested) return;

        RefreshShopUI();

        if (Input.GetKeyDown(_categoryKey))
            SwitchCategory();

        if (Input.GetKeyDown(_cycleEggKey))
            CycleSelectedItem();

        if (Input.GetKeyDown(_buyKey))
            BuySelectedItem(_nearbyPlayer);
    }

    private void OnDestroy()
    {
        IsPlayerShopping = false;
        if (_shopUIRoot != null)
            Destroy(_shopUIRoot);
    }

    public void Interact(PlayerToolHandler player)
    {
        BuySelectedItem(player);
    }

    public bool CanInteract(ToolType tool)
    {
        return _category == ShopCategory.Eggs ? SelectedEgg != null : SelectedGun != null;
    }

    private void BuySelectedItem(PlayerToolHandler player)
    {
        if (_category == ShopCategory.Guns)
            BuySelectedGun(player);
        else
            BuySelectedEgg(player);
    }

    private void BuySelectedEgg(PlayerToolHandler player)
    {
        PetEggData egg = SelectedEgg;
        if (egg == null)
        {
            ShowMessage("Cửa hàng chưa có trứng để bán", _errorColor, 1.5f);
            return;
        }

        PlayerStats stats = player != null ? player.GetComponent<PlayerStats>() : null;
        if (stats == null) return;

        if (!stats.SpendGold(egg.price))
        {
            int missingGold = Mathf.Max(1, egg.price - stats.Gold);
            ShowMessage($"Thiếu {missingGold} vàng để mua trứng này", _errorColor, 1.4f);
            StartCoroutine(ShakePanel());
            AudioManager.Instance?.PlaySFX("sfx_button_click", 0.55f);
            return;
        }

        PlayerPetInventory.AddEgg(egg);
        ShowMessage($"Đã mua {GetVietnameseEggName(egg)}!", _successColor, 1.6f);
        StartCoroutine(PulseEggIcon());
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        RefreshShopUI();
        Debug.Log($"Bought {egg.eggName} for {egg.price} gold.");
    }

    private void CycleEgg()
    {
        if (_eggsForSale == null || _eggsForSale.Length == 0) return;

        _selectedEggIndex = (_selectedEggIndex + 1) % _eggsForSale.Length;
        _temporaryMessage = null;
        _messageUntil = 0f;
        RefreshShopUI();
        StartCoroutine(PulseEggIcon());
        AudioManager.Instance?.PlaySFX("sfx_button_click", 0.65f);

        PetEggData egg = SelectedEgg;
        if (egg != null)
            Debug.Log($"Selected egg: {egg.eggName} ({egg.price} gold)");
    }

    private void BuySelectedGun(PlayerToolHandler player)
    {
        GunData gun = SelectedGun;
        if (gun == null)
        {
            ShowMessage("Kho vũ khí đang trống", _errorColor, 1.5f);
            return;
        }

        PlayerStats stats = player != null ? player.GetComponent<PlayerStats>() : null;
        PlayerGunInventory inventory = player != null ? player.GetComponent<PlayerGunInventory>() : null;
        if (stats == null || inventory == null) return;

        if (inventory.IsUnlocked(gun))
        {
            inventory.Equip(gun);
            ShowMessage($"Đã trang bị {gun.displayName}!", _successColor, 1.4f);
            StartCoroutine(PulseEggIcon());
            return;
        }

        if (!stats.SpendGold(gun.price))
        {
            int missingGold = Mathf.Max(1, gun.price - stats.Gold);
            ShowMessage($"Thiếu {missingGold} vàng để mua vũ khí", _errorColor, 1.4f);
            StartCoroutine(ShakePanel());
            AudioManager.Instance?.PlaySFX("sfx_button_click", 0.55f);
            return;
        }

        inventory.UnlockAndEquip(gun);
        ShowMessage($"Mở khóa {gun.displayName} • Đã trang bị!", _successColor, 1.8f);
        StartCoroutine(PulseEggIcon());
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        GameSaveController.SaveCurrentGame();
    }

    private void SwitchCategory()
    {
        _category = _category == ShopCategory.Eggs ? ShopCategory.Guns : ShopCategory.Eggs;
        _temporaryMessage = null;
        _messageUntil = 0f;
        RefreshShopUI();
        StartCoroutine(PulseEggIcon());
        AudioManager.Instance?.PlaySFX("sfx_button_click", 0.7f);
    }

    private void CycleSelectedItem()
    {
        if (_category == ShopCategory.Eggs)
        {
            CycleEgg();
            return;
        }

        if (_gunsForSale == null || _gunsForSale.Length == 0) return;
        _selectedGunIndex = (_selectedGunIndex + 1) % _gunsForSale.Length;
        _temporaryMessage = null;
        _messageUntil = 0f;
        RefreshShopUI();
        StartCoroutine(PulseEggIcon());
        AudioManager.Instance?.PlaySFX("sfx_button_click", 0.65f);
    }

    private void ResolvePlayerStats()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        _playerStats = player != null ? player.GetComponent<PlayerStats>() : null;
        _gunInventory = player != null ? player.GetComponent<PlayerGunInventory>() : null;
    }

    private PlayerToolHandler FindNearbyPlayer()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (player == null) return null;

        float sqrRange = _interactRange * _interactRange;
        if (((Vector2)player.transform.position - (Vector2)transform.position).sqrMagnitude > sqrRange)
            return null;

        return player.GetComponent<PlayerToolHandler>();
    }

    private PetEggData SelectedEgg =>
        _eggsForSale != null && _eggsForSale.Length > 0
            ? _eggsForSale[Mathf.Clamp(_selectedEggIndex, 0, _eggsForSale.Length - 1)]
            : null;

    private GunData SelectedGun =>
        _gunsForSale != null && _gunsForSale.Length > 0
            ? _gunsForSale[Mathf.Clamp(_selectedGunIndex, 0, _gunsForSale.Length - 1)]
            : null;

    private void RefreshShopUI()
    {
        if (_eggNameText == null) return;

        if (_category == ShopCategory.Guns)
        {
            RefreshGunShopUI();
            return;
        }

        _titleText.text = $"TIỆM TRỨNG PHÉP THUẬT     [{_categoryKey}] VŨ KHÍ";

        PetEggData egg = SelectedEgg;
        if (egg == null)
        {
            _eggNameText.text = "HẾT HÀNG";
            _priceText.text = "--";
            _goldText.text = _playerStats != null ? $"Bạn có: {_playerStats.Gold} vàng" : "";
            _statusText.text = "Hãy quay lại sau";
            _statusText.color = _errorColor;
            _pageText.text = "0 / 0";
            _eggIcon.enabled = false;
            return;
        }

        _eggNameText.text = GetVietnameseEggName(egg).ToUpperInvariant();
        _priceText.text = $"GIÁ  {egg.price} VÀNG";
        _goldText.text = _playerStats != null ? $"Bạn có: {_playerStats.Gold} vàng" : "Đang tải số vàng...";
        _pageText.text = $"{_selectedEggIndex + 1} / {_eggsForSale.Length}";

        _eggIcon.sprite = egg.eggSprite;
        _eggIcon.enabled = egg.eggSprite != null;

        if (!string.IsNullOrEmpty(_temporaryMessage) && Time.unscaledTime < _messageUntil)
        {
            _statusText.text = _temporaryMessage;
            _statusText.color = _temporaryMessageColor;
            return;
        }

        _temporaryMessage = null;
        bool canAfford = _playerStats != null && _playerStats.CanAfford(egg.price);
        _statusText.text = canAfford
            ? "Sẵn sàng mua • Trứng sẽ vào túi đồ"
            : $"Chưa đủ tiền • Cần thêm {Mathf.Max(0, egg.price - (_playerStats?.Gold ?? 0))} vàng";
        _statusText.color = canAfford ? _successColor : _errorColor;
        _priceText.color = canAfford ? _accentColor : _errorColor;
    }

    private void RefreshGunShopUI()
    {
        _titleText.text = $"KHO VŨ KHÍ MA PHÁP     [{_categoryKey}] TRỨNG";
        GunData gun = SelectedGun;
        if (gun == null)
        {
            _eggNameText.text = "HẾT HÀNG";
            _priceText.text = "--";
            _statusText.text = "Hãy quay lại sau";
            _eggIcon.enabled = false;
            _pageText.text = "0 / 0";
            return;
        }

        bool unlocked = _gunInventory != null && _gunInventory.IsUnlocked(gun);
        bool equipped = unlocked && _gunInventory.CurrentGun == gun;
        bool canAfford = _playerStats != null && _playerStats.CanAfford(gun.price);

        _eggNameText.text = gun.displayName.ToUpperInvariant();
        _priceText.text = unlocked ? (equipped ? "ĐANG TRANG BỊ" : "ĐÃ SỞ HỮU") : $"GIÁ  {gun.price} VÀNG";
        _priceText.color = unlocked || canAfford ? _accentColor : _errorColor;
        _goldText.text = _playerStats != null ? $"Bạn có: {_playerStats.Gold} vàng" : "Đang tải số vàng...";
        _pageText.text = $"{_selectedGunIndex + 1} / {_gunsForSale.Length}";
        _eggIcon.sprite = gun.icon;
        _eggIcon.enabled = gun.icon != null;

        if (!string.IsNullOrEmpty(_temporaryMessage) && Time.unscaledTime < _messageUntil)
        {
            _statusText.text = _temporaryMessage;
            _statusText.color = _temporaryMessageColor;
            return;
        }

        _temporaryMessage = null;
        string fireMode = gun.fireMode == GunFireMode.Automatic ? "TỰ ĐỘNG" : "BÁN TỰ ĐỘNG";
        _statusText.text = unlocked
            ? (equipped ? $"{fireMode} • Nhấn F để đổi súng" : $"{fireMode} • Nhấn E để trang bị")
            : gun.fireMode == GunFireMode.Automatic
                ? $"{fireMode} • {gun.shotsPerSecond:0.#} viên/giây • Giữ chuột trái"
                : $"{fireMode} • {gun.pellets} viên/loạt • Bấm chuột trái";
        _statusText.color = unlocked || canAfford ? _successColor : _errorColor;
    }

    private void ShowMessage(string message, Color color, float duration)
    {
        _temporaryMessage = message;
        _temporaryMessageColor = color;
        _messageUntil = Time.unscaledTime + Mathf.Max(0.1f, duration);
        RefreshShopUI();
    }

    private string GetVietnameseEggName(PetEggData egg)
    {
        if (egg == null) return "Trứng phép thuật";

        string normalized = egg.eggName != null ? egg.eggName.ToLowerInvariant() : "";
        if (normalized.Contains("blue")) return "Trứng Lam";
        if (normalized.Contains("brown")) return "Trứng Đất";
        if (normalized.Contains("green")) return "Trứng Lục";
        if (normalized.Contains("red")) return "Trứng Hỏa";
        return string.IsNullOrWhiteSpace(egg.eggName) ? "Trứng phép thuật" : egg.eggName;
    }

    private void AnimatePanel()
    {
        if (_panelGroup == null || _panelRect == null) return;

        float target = _panelRequested ? 1f : 0f;
        _panelVisibility = Mathf.MoveTowards(_panelVisibility, target, Time.unscaledDeltaTime * 6.5f);
        float eased = 1f - Mathf.Pow(1f - _panelVisibility, 3f);

        _panelGroup.alpha = eased;
        _panelGroup.interactable = false;
        _panelGroup.blocksRaycasts = false;
        _panelRect.anchoredPosition = new Vector2(0f, Mathf.Lerp(12f, 48f, eased)) + _panelShakeOffset;
        _panelRect.localScale = Vector3.one * Mathf.Lerp(0.96f, 1f, eased);
    }

    private IEnumerator PulseEggIcon()
    {
        if (_eggIcon == null) yield break;

        RectTransform rect = _eggIcon.rectTransform;
        float elapsed = 0f;
        const float duration = 0.28f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float scale = Mathf.Lerp(0.78f, 1f, progress) +
                          Mathf.Sin(progress * Mathf.PI) * 0.15f;
            rect.localScale = Vector3.one * Mathf.Min(scale, 1.15f);
            yield return null;
        }

        rect.localScale = Vector3.one;
    }

    private IEnumerator ShakePanel()
    {
        float elapsed = 0f;
        const float duration = 0.24f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float strength = 1f - Mathf.Clamp01(elapsed / duration);
            _panelShakeOffset = new Vector2(Mathf.Sin(elapsed * 85f) * 12f * strength, 0f);
            yield return null;
        }

        _panelShakeOffset = Vector2.zero;
    }

    private void BuildShopUI()
    {
        _shopUIRoot = new GameObject("ShopkeeperPurchaseUI");
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0) _shopUIRoot.layer = uiLayer;

        Canvas canvas = _shopUIRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 850;

        CanvasScaler scaler = _shopUIRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panelObject = new GameObject("ShopPanel");
        panelObject.layer = _shopUIRoot.layer;
        _panelRect = panelObject.AddComponent<RectTransform>();
        _panelRect.SetParent(_shopUIRoot.transform, false);
        _panelRect.anchorMin = _panelRect.anchorMax = new Vector2(0.5f, 0f);
        _panelRect.pivot = new Vector2(0.5f, 0f);
        _panelRect.sizeDelta = new Vector2(650f, 286f);
        _panelRect.anchoredPosition = new Vector2(0f, 12f);
        _panelGroup = panelObject.AddComponent<CanvasGroup>();
        _panelGroup.alpha = 0f;

        Image shadow = CreateImage("Shadow", _panelRect,
            new Color(0f, 0f, 0f, 0.48f), new Vector2(0f, -8f), new Vector2(664f, 294f));
        shadow.transform.SetAsFirstSibling();

        CreateImage("PanelBackground", _panelRect, _panelColor, Vector2.zero, _panelRect.sizeDelta);
        CreateImage("TopAccent", _panelRect, _accentColor, new Vector2(0f, 140f), new Vector2(650f, 6f));
        CreateImage("HeaderShade", _panelRect,
            new Color(0.11f, 0.14f, 0.21f, 0.96f), new Vector2(0f, 108f), new Vector2(646f, 58f));

        _titleText = CreateText("Title", _panelRect, "TIỆM TRỨNG PHÉP THUẬT",
            27f, TextAlignmentOptions.Left, new Vector2(-28f, 109f), new Vector2(510f, 42f), Color.white);
        _titleText.fontStyle = FontStyles.Bold;

        _pageText = CreateText("Page", _panelRect, "1 / 4", 18f,
            TextAlignmentOptions.Center, new Vector2(260f, 109f), new Vector2(90f, 36f), _accentColor);

        Image eggFrame = CreateImage("EggFrame", _panelRect,
            new Color(0.12f, 0.16f, 0.24f, 1f), new Vector2(-220f, -3f), new Vector2(174f, 174f));
        CreateImage("EggFrameInner", eggFrame.rectTransform,
            new Color(0.04f, 0.055f, 0.09f, 1f), Vector2.zero, new Vector2(156f, 156f));
        CreateImage("EggGlow", eggFrame.rectTransform,
            new Color(_accentColor.r, _accentColor.g, _accentColor.b, 0.13f), Vector2.zero, new Vector2(136f, 136f));

        _eggIcon = CreateImage("EggIcon", eggFrame.rectTransform, Color.white, Vector2.zero, new Vector2(116f, 116f));
        _eggIcon.preserveAspect = true;

        _eggNameText = CreateText("EggName", _panelRect, "TRỨNG PHÉP THUẬT", 31f,
            TextAlignmentOptions.Left, new Vector2(83f, 46f), new Vector2(390f, 44f), Color.white);
        _eggNameText.fontStyle = FontStyles.Bold;

        _priceText = CreateText("Price", _panelRect, "GIÁ  0 VÀNG", 25f,
            TextAlignmentOptions.Left, new Vector2(83f, 7f), new Vector2(390f, 36f), _accentColor);
        _priceText.fontStyle = FontStyles.Bold;

        _goldText = CreateText("PlayerGold", _panelRect, "Bạn có: 0 vàng", 19f,
            TextAlignmentOptions.Left, new Vector2(83f, -27f), new Vector2(390f, 30f), new Color(0.82f, 0.84f, 0.9f, 1f));

        _statusText = CreateText("Status", _panelRect, "", 17f,
            TextAlignmentOptions.Left, new Vector2(83f, -61f), new Vector2(390f, 30f), _successColor);

        CreateKeyHint("CycleHint", _panelRect, $"[{_cycleEggKey}]  ĐỔI MÓN",
            new Vector2(-115f, -116f), new Vector2(250f, 42f), new Color(0.13f, 0.17f, 0.25f, 1f));
        CreateKeyHint("BuyHint", _panelRect, $"[{_buyKey}]  MUA NGAY",
            new Vector2(164f, -116f), new Vector2(278f, 42f), new Color(0.34f, 0.22f, 0.07f, 1f));
    }

    private void CreateKeyHint(string objectName, Transform parent, string label,
        Vector2 position, Vector2 size, Color backgroundColor)
    {
        Image background = CreateImage(objectName, parent, backgroundColor, position, size);
        TextMeshProUGUI text = CreateText("Label", background.rectTransform, label, 18f,
            TextAlignmentOptions.Center, Vector2.zero, size, Color.white);
        text.fontStyle = FontStyles.Bold;
    }

    private Image CreateImage(string objectName, Transform parent, Color color,
        Vector2 position, Vector2 size)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.layer = _shopUIRoot.layer;
        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = imageObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private TextMeshProUGUI CreateText(string objectName, Transform parent, string value,
        float fontSize, TextAlignmentOptions alignment, Vector2 position, Vector2 size, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.layer = _shopUIRoot.layer;
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        if (text.font == null) text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        return text;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _nearbyPlayer != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
