using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Unlocks the level-two portal after the level-one boss is defeated.
/// The portal is generated at runtime so it does not depend on scene references.
/// </summary>
public class LevelPortalBootstrap : MonoBehaviour
{
    private static Sprite _portalSprite;
    private static bool _levelOneBossDefeated;
    private string _label;
    private bool _listenForBoss;
    private float _hideLabelAt = -1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        _levelOneBossDefeated = false;
        _portalSprite = null;
    }

    public static void ResetProgress() => _levelOneBossDefeated = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoadedCallback()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SetupScene(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupScene(scene);
    }

    private static void SetupScene(Scene scene)
    {
        string sceneName = scene.name;
        if (sceneName != "GameScene" && sceneName != "GameScene2") return;

        if (sceneName == "GameScene2")
        {
            GameObject announcementObject = new GameObject("Level2Announcement");
            LevelPortalBootstrap announcement = announcementObject.AddComponent<LevelPortalBootstrap>();
            announcement._label = "MÀN 2 - Khó hơn";
            announcement._hideLabelAt = Time.unscaledTime + 3f;
            return;
        }

        GameObject watcherObject = new GameObject("LevelPortalUnlocker");
        LevelPortalBootstrap watcher = watcherObject.AddComponent<LevelPortalBootstrap>();
        watcher._listenForBoss = true;

        if (_levelOneBossDefeated)
            CreatePortal();
    }

    private static void CreatePortal()
    {
        if (GameObject.Find("LevelPortal") != null) return;

        PlayerController player = FindObjectOfType<PlayerController>();
        Vector3 origin = player != null ? player.transform.position : Vector3.zero;

        GameObject portal = new GameObject("LevelPortal");
        portal.transform.position = origin + Vector3.right * 4f;

        BoxCollider2D trigger = portal.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(1.2f, 1.8f);

        SpriteRenderer renderer = portal.AddComponent<SpriteRenderer>();
        renderer.sprite = GetPortalSprite();
        renderer.color = new Color(0.2f, 0.75f, 1f, 0.8f);
        renderer.sortingOrder = 20;
        portal.transform.localScale = new Vector3(1.2f, 1.8f, 1f);

        SceneTransition transition = portal.AddComponent<SceneTransition>();
        transition.Configure("GameScene2");

        LevelPortalBootstrap display = portal.AddComponent<LevelPortalBootstrap>();
        display._label = "Cổng sang Màn 2";
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(Enemy enemy)
    {
        if (!_listenForBoss || SceneManager.GetActiveScene().name != "GameScene") return;
        if (enemy == null || enemy.Data == null || enemy.Data.enemyType != EnemyType.DemonBoss) return;

        _levelOneBossDefeated = true;
        CreatePortal();
    }

    private static Sprite GetPortalSprite()
    {
        if (_portalSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            _portalSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 1f);
        }

        return _portalSprite;
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(_label)) return;
        if (_hideLabelAt >= 0f && Time.unscaledTime >= _hideLabelAt)
        {
            enabled = false;
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = Color.white;

        GUI.Box(new Rect(Screen.width * 0.5f - 120f, 12f, 240f, 34f), _label, style);
    }
}
