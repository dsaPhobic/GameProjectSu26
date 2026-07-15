using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpScreen : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindObjectOfType<LevelUpScreen>() != null) return;
        new GameObject("LevelUpScreen").AddComponent<LevelUpScreen>();
    }

    private class Upgrade
    {
        public string Name;
        public string Desc;
        public Sprite Icon;
        public Action<PlayerStats, PlayerController> Apply;
    }

    private readonly List<Upgrade> _pool = new();
    private bool _isOpen;
    private PlayerStats _stats;
    private PlayerController _player;

    private GameObject _panel;
    private readonly TextMeshProUGUI[] _cardName = new TextMeshProUGUI[3];
    private readonly TextMeshProUGUI[] _cardDesc = new TextMeshProUGUI[3];
    private readonly Image[] _cardIcon = new Image[3];
    private readonly Button[] _cardButton = new Button[3];

    private static Sprite _white;
    private static Sprite White
    {
        get
        {
            if (_white == null)
            {
                var t = Texture2D.whiteTexture;
                _white = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
            }
            return _white;
        }
    }

    private void Awake()
    {
        BuildPool();
        BuildUI();
        GameEvents.OnLevelUpScreenOpen += Open;
    }

    private void OnDestroy()
    {
        GameEvents.OnLevelUpScreenOpen -= Open;
    }

    private void BuildPool()
    {
        _pool.Add(new Upgrade { Name = "Sharp Blade", Desc = "+5 Damage", Icon = LoadIcon("icon_upgrade_damage"), Apply = (s, p) => s.ModifyDamage(5) });
        _pool.Add(new Upgrade { Name = "Berserker", Desc = "+10 Damage", Icon = LoadIcon("icon_upgrade_damage"), Apply = (s, p) => s.ModifyDamage(10) });
        _pool.Add(new Upgrade { Name = "Iron Body", Desc = "+25 Max HP", Icon = LoadIcon("icon_upgrade_max_hp"), Apply = (s, p) => s.ModifyMaxHP(25) });
        _pool.Add(new Upgrade { Name = "Wind Walker", Desc = "+0.6 Move Speed", Icon = LoadIcon("icon_upgrade_move_speed"), Apply = (s, p) => s.ModifyMoveSpeed(0.6f) });
        _pool.Add(new Upgrade { Name = "Rapid Strike", Desc = "+0.35 Attack Speed", Icon = LoadIcon("icon_upgrade_attack_speed"), Apply = (s, p) => s.ModifyAttackSpeed(0.35f) });
        _pool.Add(new Upgrade { Name = "Phantom Step", Desc = "-0.5s Dash Cooldown", Icon = LoadIcon("icon_upgrade_cooldown"), Apply = (s, p) => p?.ReduceDashCooldown(0.5f) });
        _pool.Add(new Upgrade
        {
            Name = "Hero Transformation",
            Desc = "Unlock armored form",
            Icon = LoadIcon("icon_upgrade_dragon_armor"),
            Apply = (stats, player) =>
            {
                player?.GetComponent<PlayerAnimator>()?.SetArmored(true);
            }
        });
    }

    private void Open()
    {
        ResolvePlayer();
        if (_stats == null) return;

        var picks = PickThree();
        for (int i = 0; i < 3; i++)
        {
            var upgrade = picks[i];
            _cardName[i].text = upgrade.Name;
            _cardDesc[i].text = upgrade.Desc;
            _cardIcon[i].sprite = upgrade.Icon;
            _cardIcon[i].enabled = upgrade.Icon != null;
            _cardButton[i].onClick.RemoveAllListeners();
            _cardButton[i].onClick.AddListener(() => Choose(upgrade));
        }

        _panel.SetActive(true);
        _isOpen = true;
    }

    private void Choose(Upgrade upgrade)
    {
        if (!_isOpen || upgrade == null) return;

        _isOpen = false;
        upgrade.Apply?.Invoke(_stats, _player);
        AudioManager.Instance?.PlaySFX("sfx_levelup");
        _panel.SetActive(false);
    }

    private List<Upgrade> PickThree()
    {
        var copy = new List<Upgrade>(_pool);
        for (int i = copy.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }

        return copy.GetRange(0, Mathf.Min(3, copy.Count));
    }

    private void ResolvePlayer()
    {
        _player = ServiceLocator.Get<PlayerController>();
        if (_player != null) _stats = _player.GetComponent<PlayerStats>();
        if (_stats == null) _stats = FindObjectOfType<PlayerStats>();
        if (_player == null && _stats != null) _player = _stats.GetComponent<PlayerController>();
    }

    private void BuildUI()
    {
        EnsureEventSystem();

        var canvasGo = new GameObject("LevelUpCanvas");
        canvasGo.layer = LayerMask.NameToLayer("UI");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGo.AddComponent<GraphicRaycaster>();

        var panelImg = NewImage("Panel", canvasGo.transform, new Color(0f, 0f, 0f, 0.55f), true);
        _panel = panelImg.gameObject;
        Stretch(panelImg.rectTransform);

        var title = NewText("Title", _panel.transform, 54, TextAlignmentOptions.Center, "CHOOSE UPGRADE");
        title.fontStyle = FontStyles.Bold;
        var trt = title.rectTransform;
        trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 1f);
        trt.pivot = new Vector2(0.5f, 1f);
        trt.anchoredPosition = new Vector2(0, -110);
        trt.sizeDelta = new Vector2(900, 80);

        float[] xs = { -320f, 0f, 320f };
        for (int i = 0; i < 3; i++)
        {
            var card = NewImage("Card" + i, _panel.transform, new Color(0.16f, 0.16f, 0.22f, 1f), true);
            var crt = card.rectTransform;
            crt.anchorMin = crt.anchorMax = crt.pivot = new Vector2(0.5f, 0.5f);
            crt.anchoredPosition = new Vector2(xs[i], -20f);
            crt.sizeDelta = new Vector2(280f, 360f);

            var btn = card.gameObject.AddComponent<Button>();
            btn.targetGraphic = card;
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.28f, 0.28f, 0.4f, 1f);
            colors.pressedColor = new Color(0.1f, 0.1f, 0.15f, 1f);
            btn.colors = colors;
            _cardButton[i] = btn;

            var nameT = NewText("Name", card.transform, 30, TextAlignmentOptions.Center, "");
            nameT.fontStyle = FontStyles.Bold;
            var nrt = nameT.rectTransform;
            nrt.anchorMin = new Vector2(0, 1);
            nrt.anchorMax = new Vector2(1, 1);
            nrt.pivot = new Vector2(0.5f, 1);
            nrt.anchoredPosition = new Vector2(0, -20);
            nrt.sizeDelta = new Vector2(-20, 64);
            _cardName[i] = nameT;

            var icon = NewImage("Icon", card.transform, Color.white, false);
            icon.preserveAspect = true;
            icon.enabled = false;
            var irt = icon.rectTransform;
            irt.anchorMin = irt.anchorMax = irt.pivot = new Vector2(0.5f, 1f);
            irt.anchoredPosition = new Vector2(0, -96f);
            irt.sizeDelta = new Vector2(96f, 96f);
            _cardIcon[i] = icon;

            var descT = NewText("Desc", card.transform, 22, TextAlignmentOptions.Center, "");
            descT.enableWordWrapping = true;
            var drt = descT.rectTransform;
            drt.anchorMin = new Vector2(0, 0);
            drt.anchorMax = new Vector2(1, 1);
            drt.pivot = new Vector2(0.5f, 0.5f);
            drt.offsetMin = new Vector2(14, 64);
            drt.offsetMax = new Vector2(-14, -196);
            _cardDesc[i] = descT;

            var selectLabel = NewText("SelectLabel", card.transform, 24, TextAlignmentOptions.Center, "SELECT");
            selectLabel.fontStyle = FontStyles.Bold;
            selectLabel.color = new Color(1f, 0.88f, 0.3f);
            var srt = selectLabel.rectTransform;
            srt.anchorMin = new Vector2(0, 0);
            srt.anchorMax = new Vector2(1, 0);
            srt.pivot = new Vector2(0.5f, 0);
            srt.anchoredPosition = new Vector2(0, 16);
            srt.sizeDelta = new Vector2(-20, 40);
        }

        _panel.SetActive(false);
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private Image NewImage(string name, Transform parent, Color color, bool raycast)
    {
        var go = new GameObject(name);
        go.layer = LayerMask.NameToLayer("UI");

        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.sprite = White;
        img.color = color;
        img.raycastTarget = raycast;
        return img;
    }

    private TextMeshProUGUI NewText(string name, Transform parent, float fontSize, TextAlignmentOptions align, string text)
    {
        var go = new GameObject(name);
        go.layer = LayerMask.NameToLayer("UI");

        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        if (tmp.font == null) tmp.font = TMP_Settings.defaultFontAsset;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }

    private static Sprite LoadIcon(string iconName)
    {
        return Resources.Load<Sprite>($"UpgradeIcons/{iconName}");
    }
}
