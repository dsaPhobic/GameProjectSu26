using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetEggInventoryUI : MonoBehaviour
{
    private VerticalLayoutGroup _layout;

    private void Awake()
    {
        Image background = gameObject.AddComponent<Image>();
        background.color = new Color(0.05f, 0.05f, 0.08f, 0.78f);
        background.raycastTarget = false;

        _layout = gameObject.AddComponent<VerticalLayoutGroup>();
        _layout.padding = new RectOffset(12, 12, 10, 10);
        _layout.spacing = 5f;
        _layout.childAlignment = TextAnchor.LowerLeft;
        _layout.childControlWidth = true;
        _layout.childControlHeight = false;
        _layout.childForceExpandWidth = true;
        _layout.childForceExpandHeight = false;
    }

    private void OnEnable()
    {
        PlayerPetInventory.InventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerPetInventory.InventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
            Destroy(child);
        }

        CreateText("Pet Eggs", 22f, FontStyles.Bold);

        bool hasEgg = false;
        for (int i = 0; i < PlayerPetInventory.EggTypeCount; i++)
        {
            PetEggData egg = PlayerPetInventory.GetEgg(i);
            int count = PlayerPetInventory.GetEggCount(i);
            if (egg == null || count <= 0) continue;

            hasEgg = true;
            CreateEggRow(egg, count);
        }

        if (!hasEgg)
            CreateText("No eggs", 17f, FontStyles.Italic);
    }

    private void CreateEggRow(PetEggData egg, int count)
    {
        GameObject row = new GameObject($"Egg_{egg.name}");
        RectTransform rt = row.AddComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.sizeDelta = new Vector2(0f, 34f);

        HorizontalLayoutGroup horizontal = row.AddComponent<HorizontalLayoutGroup>();
        horizontal.spacing = 8f;
        horizontal.childAlignment = TextAnchor.MiddleLeft;
        horizontal.childControlWidth = false;
        horizontal.childControlHeight = false;
        horizontal.childForceExpandWidth = false;
        horizontal.childForceExpandHeight = false;

        GameObject iconObject = new GameObject("Icon");
        RectTransform iconRt = iconObject.AddComponent<RectTransform>();
        iconRt.SetParent(rt, false);
        iconRt.sizeDelta = new Vector2(32f, 32f);
        Image icon = iconObject.AddComponent<Image>();
        icon.sprite = egg.eggSprite;
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        GameObject textObject = new GameObject("Count");
        RectTransform textRt = textObject.AddComponent<RectTransform>();
        textRt.SetParent(rt, false);
        textRt.sizeDelta = new Vector2(200f, 32f);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = $"{egg.eggName}  x{count}";
        text.fontSize = 17f;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    private void CreateText(string value, float size, FontStyles style)
    {
        GameObject textObject = new GameObject(value);
        RectTransform rt = textObject.AddComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.sizeDelta = new Vector2(0f, 28f);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.color = Color.white;
        text.raycastTarget = false;
    }
}
