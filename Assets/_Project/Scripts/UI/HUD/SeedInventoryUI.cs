using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedInventoryUI : MonoBehaviour
{
    [SerializeField] private float _slotSize = 52f;
    [SerializeField] private float _spacing = 6f;
    [SerializeField] private Sprite _slotBackground;


    private PlayerToolHandler _toolHandler;
    private Image[] _highlights;
    private TextMeshProUGUI[] _countLabels;
    private int _slotCount;
    private bool _built;

    private void Start()
    {
        TryResolveAndBuild();
        GameEvents.OnSeedChanged += OnSeedChanged;
        GameEvents.OnSeedCountChanged += OnSeedCountChanged;
    }

    private void Update()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        PlayerToolHandler current = player != null ? player.GetComponent<PlayerToolHandler>() : null;

        if (!_built || _toolHandler == null || (current != null && _toolHandler != current))
            TryResolveAndBuild();
    }

    private void OnDestroy()
    {
        GameEvents.OnSeedChanged -= OnSeedChanged;
        GameEvents.OnSeedCountChanged -= OnSeedCountChanged;
    }

    private void BuildSlots()
    {
        var seeds = _toolHandler?.AvailableSeeds;
        if (seeds == null || seeds.Length == 0) return;

        ClearSlots();

        _slotCount = seeds.Length;
        _highlights = new Image[_slotCount];
        _countLabels = new TextMeshProUGUI[_slotCount];

        var layout = GetComponent<VerticalLayoutGroup>();
        if (layout == null)
            layout = gameObject.AddComponent<VerticalLayoutGroup>();

        layout.spacing = _spacing;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        for (int i = 0; i < _slotCount; i++)
        {
            var slot = CreateSlot(seeds[i], i);
            slot.SetParent(transform, false);
        }

        Refresh(_toolHandler.SelectedSeedIndex);
        _built = true;
    }

    private void TryResolveAndBuild()
    {
        var player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();

        PlayerToolHandler toolHandler = player?.GetComponent<PlayerToolHandler>();
        if (toolHandler == null) return;

        _toolHandler = toolHandler;
        BuildSlots();
    }

    private void ClearSlots()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    private RectTransform CreateSlot(CropData crop, int index)
    {
        // Root slot
        var slotGO = new GameObject($"SeedSlot_{index}");
        var slotRT = slotGO.AddComponent<RectTransform>();
        slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);

        var slotBg = slotGO.AddComponent<Image>();
        if (_slotBackground != null)
        {
            slotBg.sprite = _slotBackground;
            slotBg.type = Image.Type.Sliced;
            slotBg.color = Color.white;
        }
        else
        {
            slotBg.color = new Color(0f, 0f, 0f, 0.55f);
        }
        slotBg.raycastTarget = false;

        // Highlight border (behind icon)
        var hlGO = new GameObject("Highlight");
        var hlRT = hlGO.AddComponent<RectTransform>();
        hlRT.SetParent(slotRT, false);
        hlRT.anchorMin = Vector2.zero;
        hlRT.anchorMax = Vector2.one;
        hlRT.offsetMin = new Vector2(-3f, -3f);
        hlRT.offsetMax = new Vector2(3f, 3f);
        var hl = hlGO.AddComponent<Image>();
        hl.color = new Color(1f, 0.85f, 0.1f, 1f);
        hl.raycastTarget = false;
        _highlights[index] = hl;

        // Crop icon
        var iconGO = new GameObject("Icon");
        var iconRT = iconGO.AddComponent<RectTransform>();
        iconRT.SetParent(slotRT, false);
        iconRT.anchorMin = Vector2.zero;
        iconRT.anchorMax = Vector2.one;
        iconRT.offsetMin = new Vector2(5f, 5f);
        iconRT.offsetMax = new Vector2(-5f, -5f);
        var icon = iconGO.AddComponent<Image>();
        icon.raycastTarget = false;
        icon.preserveAspect = true;

        var sprite = crop?.stageSprites?.Length > 0 ? crop.stageSprites[0] : null;
        if (sprite != null) icon.sprite = sprite;

        // Count badge (góc dưới phải)
        var badgeGO = new GameObject("Count");
        var badgeRT = badgeGO.AddComponent<RectTransform>();
        badgeRT.SetParent(slotRT, false);
        badgeRT.anchorMin = new Vector2(0.55f, 0f);
        badgeRT.anchorMax = new Vector2(1f, 0.45f);
        badgeRT.offsetMin = Vector2.zero;
        badgeRT.offsetMax = Vector2.zero;
        var badge = badgeGO.AddComponent<TextMeshProUGUI>();
        badge.text = _toolHandler != null ? _toolHandler.GetSeedCount(index).ToString() : "0";
        badge.fontSize = 14;
        badge.fontStyle = FontStyles.Bold;
        badge.alignment = TextAlignmentOptions.BottomRight;
        badge.color = Color.white;
        badge.raycastTarget = false;
        _countLabels[index] = badge;

        return slotRT;
    }

    private void OnSeedChanged(CropData seed)
    {
        Refresh(_toolHandler?.SelectedSeedIndex ?? 0);
    }

    private void OnSeedCountChanged(int index, int count)
    {
        if (_countLabels == null || index < 0 || index >= _countLabels.Length) return;
        if (_countLabels[index] != null)
            _countLabels[index].text = count.ToString();
    }

    private void Refresh(int selectedIndex)
    {
        for (int i = 0; i < _slotCount; i++)
        {
            if (_highlights[i] == null) continue;
            _highlights[i].enabled = (i == selectedIndex);
        }
    }
}
