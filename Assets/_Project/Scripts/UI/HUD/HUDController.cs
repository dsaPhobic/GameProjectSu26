using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDController : MonoBehaviour
{
    private static HUDController _instance;

    // Seed text đã có sẵn trong scene (tính năng seed cycling) — vẫn dùng reference cũ nếu có.
    [SerializeField] private TextMeshProUGUI _seedText;

    private PlayerStats _stats;

    private Image _hpFill, _xpFill;
    private TextMeshProUGUI _hpText, _xpText, _goldText, _dayText, _levelText;

    private static Sprite _whiteSprite;
    private static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var tex = Texture2D.whiteTexture;
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f), 100f);
            }
            return _whiteSprite;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureHudExists(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureHudExists(scene);
        if (_instance != null)
            _instance.HandleSceneLoaded();
    }

    private static void EnsureHudExists(Scene scene)
    {
        if (!ShouldShowHud(scene.name))
        {
            if (_instance != null)
                Destroy(_instance.gameObject);
            return;
        }

        if (FindObjectOfType<HUDController>() != null) return;

        GameObject canvasObject = new GameObject("Canvas_HUD");
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0)
            canvasObject.layer = uiLayer;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        canvasObject.AddComponent<HUDController>();
    }

    private static bool ShouldShowHud(string sceneName)
    {
        return sceneName == "GameScene" || sceneName == "GameScene2" ||
               sceneName == "ShopInterior" || sceneName == "HubTown";
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        BuildHUD();

    }

    private void Start()
    {
        GameEvents.OnPlayerHPChanged += UpdateHP;
        GameEvents.OnPlayerXPChanged += UpdateXP;
        GameEvents.OnGoldChanged += UpdateGold;
        GameEvents.OnDayChanged += UpdateDay;
        GameEvents.OnPlayerLevelUp += UpdateLevel;
        GameEvents.OnSeedChanged += UpdateSeed;

        TryResolvePlayer();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        GameEvents.OnPlayerHPChanged -= UpdateHP;
        GameEvents.OnPlayerXPChanged -= UpdateXP;
        GameEvents.OnGoldChanged -= UpdateGold;
        GameEvents.OnDayChanged -= UpdateDay;
        GameEvents.OnPlayerLevelUp -= UpdateLevel;
        GameEvents.OnSeedChanged -= UpdateSeed;
    }

    private void Update()
    {
        if (_stats == null) TryResolvePlayer();
    }

    private void HandleSceneLoaded()
    {
        _stats = null;
        TryResolvePlayer();
    }

    private void TryResolvePlayer()
    {
        var player = ServiceLocator.Get<PlayerController>();
        if (player != null) _stats = player.GetComponent<PlayerStats>();
        if (_stats == null) _stats = FindObjectOfType<PlayerStats>();
        if (_stats == null) return;

        var toolHandler = _stats.GetComponent<PlayerToolHandler>();
        if (toolHandler != null) UpdateSeed(toolHandler.SelectedSeed);

        RefreshAll();
    }

    private void RefreshAll()
    {
        UpdateHP(_stats.CurrentHP);
        UpdateXP(_stats.XP);
        UpdateGold(_stats.Gold);
        UpdateLevel(_stats.Level);
    }

    private void UpdateHP(int hp)
    {
        if (_stats == null) return;
        float t = _stats.MaxHP > 0 ? (float)hp / _stats.MaxHP : 0f;
        if (_hpFill != null) _hpFill.fillAmount = Mathf.Clamp01(t);
        if (_hpText != null) _hpText.text = $"{hp}/{_stats.MaxHP}";
    }

    private void UpdateXP(int xp)
    {
        if (_stats == null) return;
        int need = _stats.XPToNextLevel;
        if (_xpFill != null) _xpFill.fillAmount = need > 0 ? Mathf.Clamp01((float)xp / need) : 0f;
        if (_xpText != null) _xpText.text = $"{xp}/{need}";
    }

    private void UpdateGold(int gold) { if (_goldText != null) _goldText.text = $"Gold: {gold}"; }
    private void UpdateDay(int day) { if (_dayText != null) _dayText.text = $"Day {day}"; }
    private void UpdateLevel(int level) { if (_levelText != null) _levelText.text = $"Lv. {level}"; }

    private void UpdateSeed(CropData seed)
    {
        if (_seedText != null)
            _seedText.text = seed != null ? $"Seed: {seed.cropName} [Q]" : "Seed: None";
    }

    private void BuildHUD()
    {
        var dark = new Color(0.12f, 0.12f, 0.12f, 0.85f);

        _levelText = CreateText("HUD_Level", new Vector2(20, -16), new Vector2(220, 30), 26, TextAlignmentOptions.Left, "Lv. 1");

        CreateImage("HUD_HPBg", new Vector2(20, -52), new Vector2(300, 26), dark, false);
        _hpFill = CreateImage("HUD_HPFill", new Vector2(20, -52), new Vector2(300, 26), new Color(0.9f, 0.2f, 0.2f, 1f), true);
        _hpText = CreateText("HUD_HPText", new Vector2(20, -52), new Vector2(300, 26), 16, TextAlignmentOptions.Center, "HP");

        CreateImage("HUD_XPBg", new Vector2(20, -84), new Vector2(300, 20), dark, false);
        _xpFill = CreateImage("HUD_XPFill", new Vector2(20, -84), new Vector2(300, 20), new Color(0.25f, 0.55f, 1f, 1f), true);
        _xpText = CreateText("HUD_XPText", new Vector2(20, -84), new Vector2(300, 20), 13, TextAlignmentOptions.Center, "XP");

        _goldText = CreateText("HUD_Gold", new Vector2(20, -110), new Vector2(220, 28), 22, TextAlignmentOptions.Left, "Gold: 0");
        _dayText = CreateText("HUD_Day", new Vector2(20, -140), new Vector2(220, 28), 22, TextAlignmentOptions.Left, "Day 1");

        CreateSeedInventory();
        CreatePetEggInventory();

        if (_xpFill != null) _xpFill.fillAmount = 0f;
    }

    private void CreateSeedInventory()
    {
        if (FindObjectOfType<SeedInventoryUI>() != null) return;

        var rt = NewChild("HUD_SeedInventory", new Vector2(20, -190), new Vector2(64, 260));
        rt.gameObject.AddComponent<SeedInventoryUI>();
    }

    private void CreatePetEggInventory()
    {
        if (FindObjectOfType<PetEggInventoryUI>() != null) return;

        var go = new GameObject("HUD_PetEggInventory");
        go.layer = gameObject.layer;
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-20f, 20f);
        rt.sizeDelta = new Vector2(280f, 190f);
        go.AddComponent<PetEggInventoryUI>();
    }

    private RectTransform NewChild(string name, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.layer = gameObject.layer;
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    private Image CreateImage(string name, Vector2 pos, Vector2 size, Color color, bool filled)
    {
        var rt = NewChild(name, pos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = WhiteSprite;
        img.color = color;
        img.raycastTarget = false;
        if (filled)
        {
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillOrigin = (int)Image.OriginHorizontal.Left;
            img.fillAmount = 1f;
        }
        return img;
    }

    private TextMeshProUGUI CreateText(string name, Vector2 pos, Vector2 size, float fontSize,
        TextAlignmentOptions align, string text)
    {
        var rt = NewChild(name, pos, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (tmp.font == null) tmp.font = TMP_Settings.defaultFontAsset;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.enableWordWrapping = false;
        return tmp;
    }
}
