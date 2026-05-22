using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private UpgradeCard _cardPrefab;
    [SerializeField] private Transform _cardContainer;

    private UpgradeManager _upgradeManager;
    private readonly List<UpgradeCard> _cards = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
        if (_panel != null) _panel.SetActive(false);
    }

    private void Start()
    {
        _upgradeManager = ServiceLocator.Get<UpgradeManager>();
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<UpgradeScreen>();
    }

    public void Show(List<UpgradeData> upgrades)
    {
        foreach (var card in _cards) Destroy(card.gameObject);
        _cards.Clear();

        foreach (var upgrade in upgrades)
        {
            var card = Instantiate(_cardPrefab, _cardContainer);
            card.Init(upgrade, OnCardSelected);
            _cards.Add(card);
        }

        if (_panel != null) _panel.SetActive(true);
        GameManager.Instance?.OpenLevelUp();
    }

    private void OnCardSelected(UpgradeData upgrade)
    {
        _upgradeManager?.ApplyUpgrade(upgrade);
        if (_panel != null) _panel.SetActive(false);
    }
}
