using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Entity _entity;
    private Image _fill;
    private GameObject _barRoot;

    private const float WorldYOffset = 0.9f;
    private const float BarWorldWidth = 1f;
    private const float BarWorldHeight = 0.12f;

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

    private void Start()
    {
        _entity = GetComponent<Entity>();
        if (_entity != null) BuildBar();
    }

    private void BuildBar()
    {
        float s = Mathf.Max(transform.lossyScale.x, 0.01f);

        _barRoot = new GameObject("HealthBar");
        _barRoot.transform.SetParent(transform, false);
        _barRoot.transform.localPosition = new Vector3(0, WorldYOffset / s, 0);
        _barRoot.transform.localScale = Vector3.one * (0.01f / s);

        var canvas = _barRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 5;

        var canvasRT = _barRoot.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(BarWorldWidth * 100f, BarWorldHeight * 100f);

        CreateBar("BG", new Color(0.12f, 0.12f, 0.12f, 0.85f), false);
        _fill = CreateBar("Fill", new Color(0.9f, 0.2f, 0.2f, 1f), true);
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
            img.fillAmount = 1f;
        }
        return img;
    }

    private void LateUpdate()
    {
        if (_entity == null || _fill == null || _barRoot == null) return;

        if (_entity.IsDead)
        {
            _barRoot.SetActive(false);
            return;
        }

        float t = Mathf.Clamp01((float)_entity.CurrentHP / _entity.MaxHP);
        _fill.fillAmount = t;
        _fill.color = Color.Lerp(new Color(0.9f, 0.2f, 0.2f), new Color(0.2f, 0.8f, 0.2f), t);
    }
}
