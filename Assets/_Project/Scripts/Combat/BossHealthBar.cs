using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    private const float BarWidth = 820f;
    private const float BarHeight = 28f;

    private Entity _boss;
    private Enemy _enemy;
    private Image _fill;
    private Image _damageFill;
    private TextMeshProUGUI _nameText;
    private GameObject _root;
    private float _displayedFill = 1f;

    private static Sprite _whiteSprite;
    private static BossHealthBar _activeBar;

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

    private void Start()
    {
        _boss = GetComponent<Entity>();
        _enemy = GetComponent<Enemy>();
        if (_boss == null) return;

        if (_activeBar != null && _activeBar != this)
            _activeBar.Hide();

        _activeBar = this;
        BuildBar();
        UpdateBar(true);
    }

    private void OnDestroy()
    {
        if (_activeBar == this)
            _activeBar = null;

        Hide();
    }

    private void LateUpdate()
    {
        if (_boss == null || _root == null) return;

        if (_boss.IsDead)
        {
            Hide();
            return;
        }

        UpdateBar(false);
    }

    private void BuildBar()
    {
        var canvasGo = new GameObject("BossHealthCanvas");
        _root = canvasGo;

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        var frame = CreateRect("BossHealth_Frame", canvasGo.transform, new Vector2(0f, -52f), new Vector2(BarWidth + 18f, BarHeight + 16f));
        frame.anchorMin = new Vector2(0.5f, 1f);
        frame.anchorMax = new Vector2(0.5f, 1f);
        frame.pivot = new Vector2(0.5f, 1f);
        var frameImage = frame.gameObject.AddComponent<Image>();
        frameImage.sprite = WhiteSprite;
        frameImage.color = new Color(0.03f, 0.025f, 0.02f, 0.92f);
        frameImage.raycastTarget = false;

        var bg = CreateImage("BossHealth_Backplate", frame, Vector2.zero, new Vector2(BarWidth, BarHeight), new Color(0.11f, 0.085f, 0.07f, 1f), false);
        _damageFill = CreateImage("BossHealth_DamageFill", frame, Vector2.zero, new Vector2(BarWidth, BarHeight), new Color(0.75f, 0.56f, 0.28f, 1f), true);
        _fill = CreateImage("BossHealth_Fill", frame, Vector2.zero, new Vector2(BarWidth, BarHeight), new Color(0.58f, 0.03f, 0.035f, 1f), true);

        var topLine = CreateImage("BossHealth_TopLine", frame, new Vector2(0f, 17f), new Vector2(BarWidth + 8f, 2f), new Color(0.72f, 0.65f, 0.48f, 0.9f), false);
        var bottomLine = CreateImage("BossHealth_BottomLine", frame, new Vector2(0f, -17f), new Vector2(BarWidth + 8f, 2f), new Color(0.72f, 0.65f, 0.48f, 0.75f), false);
        bg.transform.SetAsFirstSibling();
        topLine.transform.SetAsLastSibling();
        bottomLine.transform.SetAsLastSibling();

        _nameText = CreateText("BossHealth_Name", canvasGo.transform, new Vector2(-BarWidth * 0.5f, -28f), new Vector2(BarWidth, 24f), 22f);
        _nameText.text = GetBossName();
    }

    private void UpdateBar(bool instant)
    {
        float targetFill = _boss.MaxHP > 0 ? Mathf.Clamp01((float)_boss.CurrentHP / _boss.MaxHP) : 0f;

        if (_fill != null)
            _fill.fillAmount = targetFill;

        _displayedFill = instant ? targetFill : Mathf.MoveTowards(_displayedFill, targetFill, Time.deltaTime * 0.35f);
        if (_damageFill != null)
            _damageFill.fillAmount = _displayedFill;
    }

    private string GetBossName()
    {
        if (_enemy != null && _enemy.Data != null && !string.IsNullOrWhiteSpace(_enemy.Data.enemyName))
            return _enemy.Data.enemyName;

        return "Demon Boss";
    }

    private void Hide()
    {
        if (_root != null)
            Destroy(_root);
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    private static Image CreateImage(string name, Transform parent, Vector2 anchoredPos, Vector2 size, Color color, bool filled)
    {
        var rt = CreateRect(name, parent, anchoredPos, size);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = color;
        image.raycastTarget = false;

        if (filled)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 1f;
        }

        return image;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, Vector2 anchoredPos, Vector2 size, float fontSize)
    {
        var rt = CreateRect(name, parent, anchoredPos, size);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0f, 1f);

        var text = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (text.font == null) text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Left;
        text.color = new Color(0.86f, 0.8f, 0.66f, 1f);
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        return text;
    }
}
