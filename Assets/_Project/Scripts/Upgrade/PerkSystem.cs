using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PerkSystem : MonoBehaviour
{
    [SerializeField] private List<UpgradeData> _allUpgrades;

    private UpgradeManager _upgradeManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureExists(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureExists(scene);
    }

    private static void EnsureExists(Scene scene)
    {
        if (scene.name != "GameScene" && scene.name != "ShopInterior" && scene.name != "HubTown") return;
        if (FindObjectOfType<PerkSystem>() != null) return;

        var root = new GameObject("UpgradeFlow");
        root.AddComponent<UpgradeManager>();
        root.AddComponent<UpgradeScreen>();
        root.AddComponent<PerkSystem>();
    }

    private void Awake()
    {
        if (_allUpgrades == null || _allUpgrades.Count == 0)
            _allUpgrades = new List<UpgradeData>(Resources.LoadAll<UpgradeData>("Upgrades"));
    }

    private void Start()
    {
        _upgradeManager = ServiceLocator.Get<UpgradeManager>();
        GameEvents.OnLevelUpScreenOpen += OnLevelUp;
    }

    private void OnDestroy()
    {
        GameEvents.OnLevelUpScreenOpen -= OnLevelUp;
    }

    private void OnLevelUp()
    {
        var three = GetRandomThree();
        var upgradeScreen = ServiceLocator.Get<UpgradeScreen>();
        if (upgradeScreen != null)
            upgradeScreen.Show(three);
        else
            GameManager.Instance?.ResumeGame();
    }

    public List<UpgradeData> GetRandomThree()
    {
        var applied = _upgradeManager?.GetAppliedIds() ?? new List<string>();
        var available = _allUpgrades.FindAll(u => u != null && !applied.Contains(u.id));
        available.Sort((_, __) => Random.Range(-1, 2));

        var result = new List<UpgradeData>();
        for (int i = 0; i < Mathf.Min(3, available.Count); i++)
            result.Add(available[i]);
        return result;
    }
}
