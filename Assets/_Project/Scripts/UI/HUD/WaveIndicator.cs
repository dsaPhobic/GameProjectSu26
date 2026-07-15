using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _panel;

    private const string GameplaySceneName = "GameScene";
    private const float DisplaySeconds = 1f;
    private Coroutine _hideCoroutine;

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
        if (scene.name != GameplaySceneName) return;
        if (FindObjectOfType<WaveIndicator>(true) != null) return;

        Canvas canvas = FindHUDCanvas();
        if (canvas == null) return;

        var root = new GameObject("WaveIndicator", typeof(RectTransform));
        root.layer = LayerMask.NameToLayer("UI");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var panel = CreatePanel(root.transform);
        var text = CreateText(panel.transform);

        var indicator = root.AddComponent<WaveIndicator>();
        indicator._panel = panel;
        indicator._text = text;
        panel.SetActive(false);
    }

    private static Canvas FindHUDCanvas()
    {
        GameObject hud = GameObject.Find("Canvas_HUD");
        if (hud != null && hud.TryGetComponent(out Canvas hudCanvas))
            return hudCanvas;

        return FindObjectOfType<Canvas>();
    }

    private static GameObject CreatePanel(Transform parent)
    {
        var panel = new GameObject("Panel_Wave", typeof(RectTransform), typeof(Image));
        panel.layer = LayerMask.NameToLayer("UI");
        panel.transform.SetParent(parent, false);

        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(440f, 96f);

        var image = panel.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.68f);
        image.raycastTarget = false;

        return panel;
    }

    private static TextMeshProUGUI CreateText(Transform parent)
    {
        var textObject = new GameObject("Text_Wave", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.transform.SetParent(parent, false);

        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 44f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private void OnEnable()
    {
        GameEvents.OnWaveStarted += OnWaveStarted;
        GameEvents.OnWaveCompleted += OnWaveCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnWaveStarted -= OnWaveStarted;
        GameEvents.OnWaveCompleted -= OnWaveCompleted;
    }

    private void OnWaveStarted(int wave)
    {
        if (_panel != null) _panel.SetActive(true);
        if (_text != null) _text.text = $"Wave {wave}!";

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private void OnWaveCompleted()
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        if (_panel != null) _panel.SetActive(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(DisplaySeconds);
        if (_panel != null) _panel.SetActive(false);
        _hideCoroutine = null;
    }
}
