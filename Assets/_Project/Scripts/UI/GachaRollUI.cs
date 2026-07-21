using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaRollUI : MonoBehaviour
{
    private const int SlotCount = 45;
    private const int WinningSlotIndex = 36;
    private const float SlotStep = 112f;
    private const float RollDuration = 3.2f;

    private RectTransform _strip;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _resultText;
    private bool _rolling;
    private Action _onComplete;

    private static Sprite _whiteSprite;
    private static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
            return _whiteSprite;
        }
    }

    public static void Show(GachaReward[] rewards, GachaReward winningReward, string chestName, Action onComplete)
    {
        GachaRollUI ui = FindObjectOfType<GachaRollUI>();
        if (ui == null)
            ui = new GameObject("GachaRollUI").AddComponent<GachaRollUI>();

        ui.Begin(rewards, winningReward, chestName, onComplete);
    }

    private void Begin(GachaReward[] rewards, GachaReward winningReward, string chestName, Action onComplete)
    {
        if (_rolling) return;

        BuildUI();
        gameObject.SetActive(true);
        _onComplete = onComplete;
        _titleText.text = $"{chestName} Chest";
        _resultText.text = "";

        ClearStrip();

        for (int i = 0; i < SlotCount; i++)
        {
            GachaReward reward = i == WinningSlotIndex ? winningReward : PickVisualReward(rewards);
            CreateSlot(i, reward);
        }

        StartCoroutine(Roll(winningReward));
    }

    private IEnumerator Roll(GachaReward winningReward)
    {
        _rolling = true;

        Vector2 start = new Vector2(340f, 0f);
        Vector2 end = new Vector2(-(WinningSlotIndex * SlotStep), 0f);
        _strip.anchoredPosition = start;

        float t = 0f;
        while (t < RollDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / RollDuration);
            float eased = 1f - Mathf.Pow(1f - p, 4f);
            _strip.anchoredPosition = Vector2.LerpUnclamped(start, end, eased);
            yield return null;
        }

        _strip.anchoredPosition = end;
        _resultText.text = winningReward.GetResultText();
        _onComplete?.Invoke();

        yield return new WaitForSecondsRealtime(1.4f);
        _rolling = false;
        Destroy(gameObject);
    }

    private GachaReward PickVisualReward(GachaReward[] rewards)
    {
        if (rewards == null || rewards.Length == 0) return null;
        return rewards[UnityEngine.Random.Range(0, rewards.Length)];
    }

    private void BuildUI()
    {
        if (_strip != null) return;

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        gameObject.AddComponent<GraphicRaycaster>();

        Image dim = NewImage("Dim", transform, new Color(0f, 0f, 0f, 0.72f));
        Stretch(dim.rectTransform);

        Image panel = NewImage("Panel", transform, new Color(0.09f, 0.1f, 0.12f, 0.96f));
        RectTransform panelRt = panel.rectTransform;
        panelRt.anchorMin = panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(820f, 300f);
        panelRt.anchoredPosition = Vector2.zero;

        _titleText = NewText("Title", panelRt, 28f, TextAlignmentOptions.Center, "");
        _titleText.rectTransform.anchorMin = _titleText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        _titleText.rectTransform.sizeDelta = new Vector2(760f, 42f);
        _titleText.rectTransform.anchoredPosition = new Vector2(0f, -24f);

        Image viewport = NewImage("Viewport", panelRt, new Color(0.02f, 0.025f, 0.035f, 1f));
        RectTransform viewportRt = viewport.rectTransform;
        viewportRt.anchorMin = viewportRt.anchorMax = new Vector2(0.5f, 0.5f);
        viewportRt.sizeDelta = new Vector2(720f, 116f);
        viewportRt.anchoredPosition = new Vector2(0f, 18f);
        viewport.gameObject.AddComponent<RectMask2D>();

        _strip = new GameObject("Strip").AddComponent<RectTransform>();
        _strip.SetParent(viewportRt, false);
        _strip.anchorMin = _strip.anchorMax = new Vector2(0.5f, 0.5f);
        _strip.pivot = new Vector2(0.5f, 0.5f);
        _strip.sizeDelta = new Vector2(SlotCount * SlotStep, 100f);

        Image marker = NewImage("CenterMarker", panelRt, new Color(1f, 0.24f, 0.12f, 0.95f));
        RectTransform markerRt = marker.rectTransform;
        markerRt.anchorMin = markerRt.anchorMax = new Vector2(0.5f, 0.5f);
        markerRt.sizeDelta = new Vector2(6f, 138f);
        markerRt.anchoredPosition = new Vector2(0f, 18f);

        _resultText = NewText("Result", panelRt, 24f, TextAlignmentOptions.Center, "");
        _resultText.rectTransform.anchorMin = _resultText.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        _resultText.rectTransform.sizeDelta = new Vector2(760f, 40f);
        _resultText.rectTransform.anchoredPosition = new Vector2(0f, 28f);
    }

    private void ClearStrip()
    {
        for (int i = _strip.childCount - 1; i >= 0; i--)
            Destroy(_strip.GetChild(i).gameObject);
    }

    private void CreateSlot(int index, GachaReward reward)
    {
        Image bg = NewImage("Slot_" + index, _strip, GetRarityColor(reward));
        RectTransform rt = bg.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(92f, 92f);
        rt.anchoredPosition = new Vector2(index * SlotStep, 0f);

        if (reward != null && reward.icon != null)
        {
            Image icon = NewImage("Icon", rt, Color.white);
            icon.sprite = reward.icon;
            icon.preserveAspect = true;
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            icon.rectTransform.sizeDelta = new Vector2(58f, 58f);
            icon.rectTransform.anchoredPosition = new Vector2(0f, 10f);
        }

        TextMeshProUGUI label = NewText("Label", rt, 12f, TextAlignmentOptions.Center, reward != null ? reward.displayName : "?");
        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        label.rectTransform.sizeDelta = new Vector2(86f, 24f);
        label.rectTransform.anchoredPosition = new Vector2(0f, 8f);
    }

    private Color GetRarityColor(GachaReward reward)
    {
        if (reward == null) return new Color(0.25f, 0.25f, 0.28f, 1f);
        if (reward.weight <= 2) return new Color(1f, 0.78f, 0.18f, 1f);
        if (reward.weight <= 5) return new Color(0.75f, 0.34f, 1f, 1f);
        if (reward.weight <= 10) return new Color(0.22f, 0.55f, 1f, 1f);
        return new Color(0.38f, 0.42f, 0.46f, 1f);
    }

    private Image NewImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.sprite = WhiteSprite;
        img.color = color;
        return img;
    }

    private TextMeshProUGUI NewText(string name, Transform parent, float fontSize, TextAlignmentOptions align, string text)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        if (tmp.font == null) tmp.font = TMP_Settings.defaultFontAsset;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.enableWordWrapping = false;
        return tmp;
    }

    private void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
