using UnityEngine;
using UnityEngine.UI;

public class PlayerChargeBar : MonoBehaviour
{
    private const float WorldYOffset = -0.7f;
    private const float BarWorldWidth = 1.1f;
    private const float BarWorldHeight = 0.12f;

    private GameObject _barRoot;
    private Image _fill;

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

    private void Awake()
    {
        BuildBar();
        Hide();
    }

    public void Show()
    {
        if (_barRoot == null || _fill == null)
            BuildBar();

        if (_barRoot != null) _barRoot.SetActive(true);
        SetProgress(0f);
    }

    public void Hide()
    {
        if (_barRoot != null) _barRoot.SetActive(false);
    }

    public void SetProgress(float progress)
    {
        if (_fill == null) return;

        float t = Mathf.Clamp01(progress);
        _fill.fillAmount = t;
        _fill.color = t >= 1f
            ? new Color(1f, 0.9f, 0.25f, 1f)
            : Color.Lerp(new Color(0.35f, 0.7f, 1f, 1f), new Color(1f, 0.45f, 0.2f, 1f), t);
    }

    private void BuildBar()
    {
        float s = Mathf.Max(transform.lossyScale.x, 0.01f);

        _barRoot = new GameObject("ChargeBar");
        _barRoot.transform.SetParent(transform, false);
        _barRoot.transform.localPosition = new Vector3(0f, WorldYOffset / s, 0f);
        _barRoot.transform.localScale = Vector3.one * (0.01f / s);

        var canvas = _barRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 8;

        var canvasRT = _barRoot.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(BarWorldWidth * 100f, BarWorldHeight * 100f);

        CreateBar("BG", new Color(0.08f, 0.08f, 0.08f, 0.85f), false);
        _fill = CreateBar("Fill", new Color(0.35f, 0.7f, 1f, 1f), true);
    }

    private Image CreateBar(string childName, Color color, bool filled)
    {
        var go = new GameObject(childName);
        go.transform.SetParent(_barRoot.transform, false);
        var img = go.AddComponent<Image>();
        img.sprite = WhiteSprite;
        img.color = color;
        img.raycastTarget = false;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        if (filled)
        {
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillOrigin = (int)Image.OriginHorizontal.Left;
            img.fillAmount = 0f;
        }

        return img;
    }
}
