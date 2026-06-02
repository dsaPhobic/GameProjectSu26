using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menu: MagicFarm > Create Scenes
/// Chạy từng menu item để tạo scene tự động.
/// </summary>
public static class MagicFarmSceneCreator
{
    private const string SCENE_DIR = "Assets/_Project/Scenes/";
    private const string UI_PNG = "Assets/_Project/Sprites/UI/kenney_ui-pack-rpg-expansion/PNG/";

    // ─────────────────────────────────────────────────────────────
    // MENU ITEMS
    // ─────────────────────────────────────────────────────────────

    [MenuItem("MagicFarm/Create Scenes/1 - MainMenu Scene")]
    public static void CreateMainMenu()
    {
        if (!ConfirmCreate("MainMenu")) return;

        EnsureSpriteUIType(
            UI_PNG + "panel_brown.png",
            UI_PNG + "panel_beige.png",
            UI_PNG + "panelInset_brown.png",
            UI_PNG + "buttonLong_brown.png",
            UI_PNG + "buttonLong_brown_pressed.png"
        );

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── Camera ──────────────────────────────────────────────
        var cam = BuildCamera(new Color(0.08f, 0.06f, 0.04f));

        // ── EventSystem ─────────────────────────────────────────
        BuildEventSystem();

        // ── AudioManager ────────────────────────────────────────
        BuildAudioManager();

        // ── SceneLoader (DontDestroyOnLoad — phải có ở scene đầu tiên) ──
        var sceneLoaderGO = new GameObject("SceneLoader");
        sceneLoaderGO.AddComponent<SceneLoader>();

        // ── Canvas ──────────────────────────────────────────────
        var canvasGO = BuildCanvas("Canvas");
        var controller = canvasGO.AddComponent<MainMenuController>();

        // Background (full screen, màu tối)
        var bgGO = BuildStretchImage(canvasGO.transform, "Background",
            LoadSprite(UI_PNG + "panel_brown.png"), new Color(0.12f, 0.08f, 0.05f));

        // Title
        BuildText(canvasGO.transform, "Title",
            "Magic Farm Defender",
            72, FontStyles.Bold, new Color(1f, 0.9f, 0.4f),
            new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.96f), Vector2.zero, new Vector2(1000, 130));

        // Panel Buttons (VerticalLayoutGroup)
        var btnPanel = BuildVerticalPanel(canvasGO.transform, "Panel_Buttons",
            new Vector2(0.35f, 0.28f), new Vector2(0.65f, 0.78f));

        var btnNormal = LoadSprite(UI_PNG + "buttonLong_brown.png");
        var btnPressed = LoadSprite(UI_PNG + "buttonLong_brown_pressed.png");

        var playBtn     = BuildMenuButton(btnPanel.transform, "Btn_Play",     "PLAY",     btnNormal, btnPressed);
        var continueBtn = BuildMenuButton(btnPanel.transform, "Btn_Continue", "CONTINUE", btnNormal, btnPressed);
        var settingsBtn = BuildMenuButton(btnPanel.transform, "Btn_Settings", "SETTINGS", btnNormal, btnPressed);
        var quitBtn     = BuildMenuButton(btnPanel.transform, "Btn_Quit",     "QUIT",     btnNormal, btnPressed);

        // Panel Settings (hidden)
        var settingsPanel = BuildCenteredPanel(canvasGO.transform, "Panel_Settings",
            new Vector2(500, 420), LoadSprite(UI_PNG + "panel_beige.png"), new Color(0.93f, 0.87f, 0.76f));
        settingsPanel.SetActive(false);

        BuildText(settingsPanel.transform, "Title_Settings",
            "Settings", 40, FontStyles.Bold, Color.black,
            new Vector2(0f, 0.82f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);

        var bgmSlider        = BuildSliderRow(settingsPanel.transform, "Row_BGM",         "Music",      0.60f, 0.80f, 0.8f);
        var sfxSlider        = BuildSliderRow(settingsPanel.transform, "Row_SFX",         "SFX",        0.38f, 0.58f, 1.0f);
        var fullscreenToggle = BuildToggleRow(settingsPanel.transform, "Row_Fullscreen",  "Fullscreen", 0.16f, 0.36f);

        var closeBtn = BuildMenuButton(settingsPanel.transform, "Btn_Close", "CLOSE", btnNormal, btnPressed);
        SetAnchors(closeBtn.GetComponent<RectTransform>(),
            new Vector2(0.2f, 0.02f), new Vector2(0.8f, 0.15f));

        // ── Wire SettingsMenu ────────────────────────────────────
        var settingsMenu = settingsPanel.AddComponent<SettingsMenu>();
        SetSerializedField(settingsMenu, "_bgmSlider",        bgmSlider);
        SetSerializedField(settingsMenu, "_sfxSlider",        sfxSlider);
        SetSerializedField(settingsMenu, "_fullscreenToggle", fullscreenToggle);

        // ── Wire MainMenuController ──────────────────────────────
        SetSerializedField(controller, "_settingsPanel", settingsPanel);

        // ── Wire Button OnClick (persistent) ────────────────────
        AddClick(playBtn,     controller.OnPlayPressed);
        AddClick(continueBtn, controller.OnContinuePressed);
        AddClick(settingsBtn, controller.OnSettingsPressed);
        AddClick(quitBtn,     controller.OnQuitPressed);
        AddClick(closeBtn,    settingsMenu.OnClosePressed);

        // ── Save ─────────────────────────────────────────────────
        EditorSceneManager.SaveScene(scene, SCENE_DIR + "MainMenu.unity");
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("✅ MainMenu Scene Created!",
            "Scene lưu tại:\nAssets/_Project/Scenes/MainMenu.unity\n\n" +
            "⚠️  2 bước còn lại phải làm tay:\n\n" +
            "1. Tạo AudioMixer:\n" +
            "   Assets > Create > Audio Mixer\n" +
            "   Đặt tên 'MainMixer', expose 2 param:\n" +
            "   'BGMVolume' và 'SFXVolume'\n\n" +
            "2. Kéo MainMixer vào:\n" +
            "   Panel_Settings > SettingsMenu > _mixer\n\n" +
            "3. Thêm scene vào Build Settings (index 0)",
            "OK");
    }

    [MenuItem("MagicFarm/Create Scenes/2 - GameOver Scene")]
    public static void CreateGameOver()
    {
        if (!ConfirmCreate("GameOver")) return;

        EnsureSpriteUIType(
            UI_PNG + "panel_brown.png",
            UI_PNG + "panelInset_brown.png",
            UI_PNG + "buttonLong_brown.png",
            UI_PNG + "buttonLong_brown_pressed.png"
        );

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        BuildCamera(new Color(0.05f, 0.04f, 0.03f));
        BuildEventSystem();

        var sceneLoaderGO = new GameObject("SceneLoader");
        sceneLoaderGO.AddComponent<SceneLoader>();

        var canvasGO = BuildCanvas("Canvas");

        // Full-screen dark overlay
        var overlayGO = BuildStretchImage(canvasGO.transform, "Panel_GameOver",
            null, new Color(0f, 0f, 0f, 0.85f));

        // Game Over title
        BuildText(overlayGO.transform, "Text_Title",
            "GAME OVER",
            90, FontStyles.Bold, new Color(0.9f, 0.2f, 0.15f),
            new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.88f), Vector2.zero, new Vector2(800, 150));

        // Stats panel
        var statsPanel = BuildVerticalPanel(overlayGO.transform, "Panel_Stats",
            new Vector2(0.25f, 0.38f), new Vector2(0.75f, 0.68f));
        statsPanel.GetComponent<VerticalLayoutGroup>().spacing = 12f;

        var dayText    = BuildStatText(statsPanel.transform, "Text_DaysSurvived",  "Days Survived: 0");
        var goldText   = BuildStatText(statsPanel.transform, "Text_GoldEarned",    "Gold Earned: 0");
        var enemyText  = BuildStatText(statsPanel.transform, "Text_EnemiesKilled", "Enemies Killed: 0");

        // Buttons row
        var btnPanel = BuildHorizontalPanel(overlayGO.transform, "Panel_Buttons",
            new Vector2(0.25f, 0.14f), new Vector2(0.75f, 0.30f));

        var btnNormal  = LoadSprite(UI_PNG + "buttonLong_brown.png");
        var btnPressed = LoadSprite(UI_PNG + "buttonLong_brown_pressed.png");
        var retryBtn   = BuildMenuButton(btnPanel.transform, "Btn_Retry",    "RETRY",     btnNormal, btnPressed);
        var menuBtn    = BuildMenuButton(btnPanel.transform, "Btn_MainMenu", "MAIN MENU", btnNormal, btnPressed);

        // Wire GameOverScreen
        var gameOverScreen = overlayGO.AddComponent<GameOverScreen>();
        SetSerializedField(gameOverScreen, "_daysSurvivedText",  dayText);
        SetSerializedField(gameOverScreen, "_goldEarnedText",    goldText);
        SetSerializedField(gameOverScreen, "_enemiesKilledText", enemyText);

        AddClick(retryBtn, gameOverScreen.OnRetryPressed);
        AddClick(menuBtn,  gameOverScreen.OnMainMenuPressed);

        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene, SCENE_DIR + "GameOver.unity");
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("✅ GameOver Scene Created!",
            "Scene lưu tại:\nAssets/_Project/Scenes/GameOver.unity\n\n" +
            "⚠️  Thêm scene vào Build Settings (index 2)\n\n" +
            "Lưu ý: ShowResults() được gọi tự động\n" +
            "khi GameOverDetector load scene này.",
            "OK");
    }

    // ─────────────────────────────────────────────────────────────
    // BUILDER HELPERS
    // ─────────────────────────────────────────────────────────────

    static GameObject BuildCamera(Color bgColor)
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgColor;
        cam.orthographic = true;
        go.AddComponent<AudioListener>();
        return go;
    }

    static void BuildEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    static void BuildAudioManager()
    {
        var go = new GameObject("AudioManager");
        var mgr = go.AddComponent<AudioManager>();

        var bgmSrc = new GameObject("BGM_Source").AddComponent<AudioSource>();
        bgmSrc.loop = true;
        bgmSrc.playOnAwake = false;
        bgmSrc.transform.SetParent(go.transform);

        var sfxSrc = new GameObject("SFX_Source").AddComponent<AudioSource>();
        sfxSrc.loop = false;
        sfxSrc.playOnAwake = false;
        sfxSrc.transform.SetParent(go.transform);

        SetSerializedField(mgr, "_bgmSource", bgmSrc);
        SetSerializedField(mgr, "_sfxSource", sfxSrc);
    }

    static GameObject BuildCanvas(string name)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static GameObject BuildStretchImage(Transform parent, string name, Sprite sprite, Color fallbackColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        if (sprite != null) { img.sprite = sprite; img.type = Image.Type.Tiled; }
        else img.color = fallbackColor;
        return go;
    }

    static void BuildText(Transform parent, string name, string text, float size,
        FontStyles style, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        if (sizeDelta != Vector2.zero)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = sizeDelta;
        }
        else
        {
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    static TextMeshProUGUI BuildStatText(Transform parent, string name, string text)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500, 50);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 50;
        return tmp;
    }

    static GameObject BuildVerticalPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 20f;
        vlg.childControlHeight = false;
        vlg.childControlWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        return go;
    }

    static GameObject BuildHorizontalPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var hlg = go.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 30f;
        hlg.childControlHeight = false;
        hlg.childControlWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childForceExpandWidth = false;
        return go;
    }

    static GameObject BuildCenteredPanel(Transform parent, string name,
        Vector2 size, Sprite sprite, Color fallback)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        var img = go.AddComponent<Image>();
        if (sprite != null) { img.sprite = sprite; img.type = Image.Type.Sliced; }
        else img.color = fallback;
        return go;
    }

    static GameObject BuildMenuButton(Transform parent, string name, string label,
        Sprite normal, Sprite pressed)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 70);

        var img = go.AddComponent<Image>();
        if (normal != null) img.sprite = normal;
        img.type = Image.Type.Sliced;

        var btn = go.AddComponent<Button>();
        if (normal != null && pressed != null)
        {
            btn.transition = Selectable.Transition.SpriteSwap;
            var ss = btn.spriteState;
            ss.pressedSprite = pressed;
            ss.highlightedSprite = normal;
            btn.spriteState = ss;
        }

        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 360;
        le.preferredHeight = 70;

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = textRT.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(0.95f, 0.88f, 0.7f);
        tmp.alignment = TextAlignmentOptions.Center;

        return go;
    }

    static Slider BuildSliderRow(Transform parent, string rowName, string labelText,
        float anchorMinY, float anchorMaxY, float defaultValue)
    {
        var row = new GameObject(rowName);
        row.transform.SetParent(parent, false);
        var rt = row.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.05f, anchorMinY);
        rt.anchorMax = new Vector2(0.95f, anchorMaxY);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.spacing = 10f;
        hlg.childControlHeight = false;
        hlg.childControlWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childForceExpandWidth = false;

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(row.transform, false);
        labelGO.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 40);
        var labelLE = labelGO.AddComponent<LayoutElement>();
        labelLE.preferredWidth = 120;
        labelLE.preferredHeight = 40;
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text = labelText;
        labelTMP.fontSize = 24;
        labelTMP.color = Color.black;
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(row.transform, false);
        var sliderRT = sliderGO.AddComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(220, 30);
        var sliderLE = sliderGO.AddComponent<LayoutElement>();
        sliderLE.preferredWidth = 220;
        sliderLE.preferredHeight = 30;
        var slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = defaultValue;

        var bgImg = new GameObject("Background");
        bgImg.transform.SetParent(sliderGO.transform, false);
        var bgRT = bgImg.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.25f);
        bgRT.anchorMax = new Vector2(1, 0.75f);
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;
        var bgI = bgImg.AddComponent<Image>();
        bgI.color = new Color(0.6f, 0.55f, 0.45f);
        slider.targetGraphic = bgI;

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        var faRT = fillArea.AddComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.25f);
        faRT.anchorMax = new Vector2(1, 0.75f);
        faRT.offsetMin = faRT.offsetMax = Vector2.zero;

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(defaultValue, 1);
        fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;
        var fillI = fill.AddComponent<Image>();
        fillI.color = new Color(0.8f, 0.6f, 0.2f);
        slider.fillRect = fillRT;

        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderGO.transform, false);
        var haRT = handleArea.AddComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero;
        haRT.anchorMax = Vector2.one;
        haRT.offsetMin = haRT.offsetMax = Vector2.zero;

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleRT = handle.AddComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(20, 20);
        var handleI = handle.AddComponent<Image>();
        handleI.color = new Color(0.95f, 0.85f, 0.5f);
        slider.handleRect = handleRT;

        return slider;
    }

    static Toggle BuildToggleRow(Transform parent, string rowName, string labelText,
        float anchorMinY, float anchorMaxY)
    {
        var row = new GameObject(rowName);
        row.transform.SetParent(parent, false);
        var rt = row.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.05f, anchorMinY);
        rt.anchorMax = new Vector2(0.95f, anchorMaxY);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.spacing = 10f;
        hlg.childControlHeight = false;
        hlg.childControlWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childForceExpandWidth = false;

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(row.transform, false);
        var labelLE = labelGO.AddComponent<LayoutElement>();
        labelLE.preferredWidth = 120;
        labelLE.preferredHeight = 40;
        labelGO.AddComponent<RectTransform>().sizeDelta = new Vector2(120, 40);
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text = labelText;
        labelTMP.fontSize = 24;
        labelTMP.color = Color.black;
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var toggleGO = new GameObject("Toggle");
        toggleGO.transform.SetParent(row.transform, false);
        var toggleRT = toggleGO.AddComponent<RectTransform>();
        toggleRT.sizeDelta = new Vector2(40, 40);
        var toggleLE = toggleGO.AddComponent<LayoutElement>();
        toggleLE.preferredWidth = 40;
        toggleLE.preferredHeight = 40;
        var toggle = toggleGO.AddComponent<Toggle>();

        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(toggleGO.transform, false);
        var bgRT = bgGO.AddComponent<RectTransform>();
        bgRT.anchorMin = bgRT.anchorMax = new Vector2(0.5f, 0.5f);
        bgRT.sizeDelta = new Vector2(30, 30);
        var bgI = bgGO.AddComponent<Image>();
        bgI.color = new Color(0.7f, 0.65f, 0.55f);
        toggle.targetGraphic = bgI;

        var checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bgGO.transform, false);
        var checkRT = checkGO.AddComponent<RectTransform>();
        checkRT.anchorMin = Vector2.zero;
        checkRT.anchorMax = Vector2.one;
        checkRT.offsetMin = checkRT.offsetMax = Vector2.zero;
        var checkI = checkGO.AddComponent<Image>();
        checkI.color = new Color(0.9f, 0.7f, 0.2f);
        toggle.graphic = checkI;
        toggle.isOn = Screen.fullScreen;

        return toggle;
    }

    // ─────────────────────────────────────────────────────────────
    // UTILITY HELPERS
    // ─────────────────────────────────────────────────────────────

    static void SetAnchors(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static void AddClick(GameObject btn, UnityEngine.Events.UnityAction action)
    {
        var button = btn.GetComponent<Button>();
        if (button == null) return;
        UnityEventTools.AddPersistentListener(button.onClick, action);
    }

    static void SetSerializedField(Object target, string fieldName, Object value)
    {
        var so = new SerializedObject(target);
        var prop = so.FindProperty(fieldName);
        if (prop != null)
        {
            prop.objectReferenceValue = value;
            so.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning($"[MagicFarmSceneCreator] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }

    static Sprite LoadSprite(string path)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            Debug.LogWarning($"[MagicFarmSceneCreator] Sprite not found: {path}");
        return sprite;
    }

    static void EnsureSpriteUIType(params string[] paths)
    {
        bool reimported = false;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            if (importer.textureType == TextureImporterType.Sprite) continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            reimported = true;
        }
        if (reimported) AssetDatabase.Refresh();
    }

    static bool ConfirmCreate(string sceneName)
    {
        string path = SCENE_DIR + sceneName + ".unity";
        if (System.IO.File.Exists(path))
        {
            return EditorUtility.DisplayDialog(
                "Scene đã tồn tại",
                $"{sceneName}.unity đã có. Ghi đè?",
                "Ghi đè", "Hủy");
        }
        return true;
    }
}
