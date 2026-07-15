using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsOverlay : MonoBehaviour
{
    private const KeyCode ToggleKey = KeyCode.Tab;

    private GameObject _panel;
    private TextMeshProUGUI _bodyText;
    private PlayerStats _stats;

    private static Sprite _whiteSprite;
    private static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var tex = Texture2D.whiteTexture;
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }

            return _whiteSprite;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetBootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureForScene(scene);
    }

    private static void EnsureForScene(Scene scene)
    {
        if (!ShouldShow(scene.name)) return;
        if (FindObjectOfType<StatsOverlay>(true) != null) return;

        Canvas canvas = FindHUDCanvas();
        if (canvas == null) return;

        var root = new GameObject("StatsOverlay", typeof(RectTransform));
        root.layer = LayerMask.NameToLayer("UI");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var overlay = root.AddComponent<StatsOverlay>();
        overlay.BuildPanel(root.transform);
    }

    private static bool ShouldShow(string sceneName)
    {
        return sceneName == "GameScene" || sceneName == "ShopInterior" || sceneName == "HubTown";
    }

    private static Canvas FindHUDCanvas()
    {
        GameObject hud = GameObject.Find("Canvas_HUD");
        if (hud != null && hud.TryGetComponent(out Canvas hudCanvas))
            return hudCanvas;

        return FindObjectOfType<Canvas>();
    }

    private void Update()
    {
        if (_panel == null) return;

        bool shouldShow = Input.GetKey(ToggleKey);
        if (_panel.activeSelf != shouldShow)
            _panel.SetActive(shouldShow);

        if (!shouldShow) return;

        if (_stats == null)
            ResolvePlayerStats();

        RefreshText();
    }

    private void ResolvePlayerStats()
    {
        var player = ServiceLocator.Get<PlayerController>();
        if (player != null)
            _stats = player.GetComponent<PlayerStats>();

        if (_stats == null)
            _stats = FindObjectOfType<PlayerStats>();
    }

    private void BuildPanel(Transform parent)
    {
        _panel = new GameObject("Panel_Stats", typeof(RectTransform), typeof(Image));
        _panel.layer = LayerMask.NameToLayer("UI");
        _panel.transform.SetParent(parent, false);

        var rect = _panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0.5f);
        rect.anchorMax = new Vector2(1f, 0.5f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = new Vector2(-28f, 0f);
        rect.sizeDelta = new Vector2(360f, 300f);

        var image = _panel.GetComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = new Color(0.035f, 0.04f, 0.055f, 0.9f);
        image.raycastTarget = false;

        var title = CreateText("Text_StatsTitle", _panel.transform, new Vector2(0f, -18f), new Vector2(-32f, 42f), 26f);
        title.text = "STATS";
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(1f, 0.86f, 0.42f, 1f);

        _bodyText = CreateText("Text_StatsBody", _panel.transform, new Vector2(18f, -68f), new Vector2(-36f, -88f), 21f);
        _bodyText.alignment = TextAlignmentOptions.TopLeft;
        _bodyText.enableWordWrapping = false;
        _bodyText.lineSpacing = 12f;

        _panel.SetActive(false);
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta, float fontSize)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.transform.SetParent(parent, false);

        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private void RefreshText()
    {
        if (_bodyText == null) return;

        if (_stats == null)
        {
            _bodyText.text = "No player stats found";
            return;
        }

        _bodyText.text =
            $"Level: {_stats.Level}\n" +
            $"HP: {_stats.CurrentHP}/{_stats.MaxHP}\n" +
            $"XP: {_stats.XP}/{_stats.XPToNextLevel}\n" +
            $"Gold: {_stats.Gold}\n" +
            $"Damage: {_stats.Damage}\n" +
            $"Attack Speed: {_stats.AttackSpeed:0.##}\n" +
            $"Move Speed: {_stats.MoveSpeed:0.##}";
    }
}
