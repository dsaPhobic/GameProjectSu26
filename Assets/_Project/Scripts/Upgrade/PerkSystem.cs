using System.Collections.Generic;
using UnityEngine;

public class PerkSystem : MonoBehaviour
{
    [SerializeField] private List<UpgradeData> _allUpgrades;

    private UpgradeManager _upgradeManager;

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
        upgradeScreen?.Show(three);
    }

    public List<UpgradeData> GetRandomThree()
    {
        var applied = _upgradeManager?.GetAppliedIds() ?? new List<string>();
        var available = _allUpgrades.FindAll(u => !applied.Contains(u.id));
        available.Sort((_, __) => Random.Range(-1, 2));

        var result = new List<UpgradeData>();
        for (int i = 0; i < Mathf.Min(3, available.Count); i++)
            result.Add(available[i]);
        return result;
    }
}
