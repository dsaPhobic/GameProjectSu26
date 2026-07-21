using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpTimerUI : MonoBehaviour
{
    private const float RootYOffset = 1.35f;
    private const float RootScale = 0.01f;
    private const float SlotSpacing = 46f;

    private static readonly Dictionary<PlayerStats, List<PowerUpTimerUI>> ActiveByPlayer = new();
    private static Sprite _whiteSprite;

    private Image _icon;
    private Image _fillBar;
    private TextMeshProUGUI _timeText;
    private PowerUpType _type;
    private PlayerStats _owner;
    private float _duration;
    private float _remaining;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        ActiveByPlayer.Clear();
        _whiteSprite = null;
    }

    public static void Show(PlayerStats owner, PowerUpData data)
    {
        Show(owner, data, data != null ? data.duration : 0f);
    }

    public static void Show(PlayerStats owner, PowerUpData data, float remaining)
    {
        if (owner == null || data == null || data.duration <= 0f) return;

        var timers = GetTimers(owner);
        var existing = timers.Find(timer => timer != null && timer._type == data.type);
        if (existing != null)
        {
            existing.ResetTimer(data.duration, data.icon, remaining);
            return;
        }

        var root = FindOrCreateRoot(owner.transform);
        var timerObject = new GameObject("Timer_" + data.type);
        timerObject.transform.SetParent(root.transform, false);

        var timer = timerObject.AddComponent<PowerUpTimerUI>();
        timer.Init(owner, data, remaining);
        timers.Add(timer);
        Reposition(timers);
    }

    public void ShowTimer(float duration)
    {
        ResetTimer(duration, _icon != null ? _icon.sprite : null);
    }

    private static List<PowerUpTimerUI> GetTimers(PlayerStats owner)
    {
        if (!ActiveByPlayer.TryGetValue(owner, out var timers))
        {
            timers = new List<PowerUpTimerUI>();
            ActiveByPlayer[owner] = timers;
        }

        timers.RemoveAll(timer => timer == null);
        return timers;
    }

    private static GameObject FindOrCreateRoot(Transform owner)
    {
        Transform existing = owner.Find("PowerUpTimers");
        if (existing != null) return existing.gameObject;

        var root = new GameObject("PowerUpTimers");
        root.transform.SetParent(owner, false);
        root.transform.localPosition = new Vector3(0f, RootYOffset, 0f);
        root.transform.localScale = Vector3.one * RootScale;

        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 250;

        var rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(260f, 52f);

        return root;
    }

    private static void Reposition(List<PowerUpTimerUI> timers)
    {
        timers.RemoveAll(timer => timer == null);

        float startX = -((timers.Count - 1) * SlotSpacing) * 0.5f;
        for (int i = 0; i < timers.Count; i++)
        {
            var rect = timers[i].GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + i * SlotSpacing, 0f);
        }
    }

    private void Init(PlayerStats owner, PowerUpData data, float remaining)
    {
        _owner = owner;
        _type = data.type;
        BuildUI(data.icon);
        ResetTimer(data.duration, data.icon, remaining);
    }

    private void BuildUI(Sprite iconSprite)
    {
        var rect = gameObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(42f, 50f);

        var bg = NewImage("Background", transform, new Color(0f, 0f, 0f, 0.58f));
        bg.rectTransform.sizeDelta = new Vector2(42f, 42f);
        bg.rectTransform.anchoredPosition = new Vector2(0f, 4f);

        _icon = NewImage("Icon", transform, Color.white);
        _icon.sprite = iconSprite;
        _icon.preserveAspect = true;
        _icon.rectTransform.sizeDelta = new Vector2(34f, 34f);
        _icon.rectTransform.anchoredPosition = new Vector2(0f, 4f);

        _fillBar = NewImage("Fill", transform, new Color(0.1f, 0.8f, 1f, 0.32f));
        _fillBar.type = Image.Type.Filled;
        _fillBar.fillMethod = Image.FillMethod.Radial360;
        _fillBar.fillOrigin = 2;
        _fillBar.fillClockwise = false;
        _fillBar.rectTransform.sizeDelta = new Vector2(42f, 42f);
        _fillBar.rectTransform.anchoredPosition = new Vector2(0f, 4f);

        var textObject = new GameObject("TimeText");
        textObject.transform.SetParent(transform, false);
        var textRect = textObject.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(58f, 16f);
        textRect.anchoredPosition = new Vector2(0f, -24f);

        _timeText = textObject.AddComponent<TextMeshProUGUI>();
        if (_timeText.font == null) _timeText.font = TMP_Settings.defaultFontAsset;
        _timeText.alignment = TextAlignmentOptions.Center;
        _timeText.fontSize = 13f;
        _timeText.fontStyle = FontStyles.Bold;
        _timeText.color = Color.white;
        _timeText.raycastTarget = false;
    }

    private Image NewImage(string objectName, Transform parent, Color color)
    {
        var imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);

        var rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

        var image = imageObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private void ResetTimer(float duration, Sprite iconSprite)
    {
        ResetTimer(duration, iconSprite, duration);
    }

    private void ResetTimer(float duration, Sprite iconSprite, float remaining)
    {
        _duration = Mathf.Max(0.1f, duration);
        _remaining = Mathf.Clamp(remaining, 0f, _duration);

        if (_icon != null && iconSprite != null)
            _icon.sprite = iconSprite;

        UpdateVisual();
    }

    private void Update()
    {
        _remaining -= Time.deltaTime;
        UpdateVisual();

        if (_remaining <= 0f)
            Close();
    }

    private void UpdateVisual()
    {
        float normalized = Mathf.Clamp01(_remaining / _duration);

        if (_fillBar != null)
            _fillBar.fillAmount = normalized;

        if (_timeText != null)
            _timeText.text = Mathf.CeilToInt(Mathf.Max(0f, _remaining)).ToString();
    }

    private void Close()
    {
        if (_owner != null && ActiveByPlayer.TryGetValue(_owner, out var timers))
        {
            timers.Remove(this);
            Reposition(timers);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_owner != null && ActiveByPlayer.TryGetValue(_owner, out var timers))
        {
            timers.Remove(this);
            Reposition(timers);
        }
    }

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
