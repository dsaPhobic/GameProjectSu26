using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _powerUpPrefabs;
    [SerializeField] private float _dropChance = 0.1f;
    [SerializeField] private float _nightRandomInterval = 30f;

    private float _nightTimer;
    private bool _isNight;

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += OnEnemyDied;
        GameEvents.OnDayPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= OnEnemyDied;
        GameEvents.OnDayPhaseChanged -= OnPhaseChanged;
    }

    private void Update()
    {
        if (!_isNight) return;
        _nightTimer -= Time.deltaTime;
        if (_nightTimer <= 0)
        {
            _nightTimer = _nightRandomInterval;
            SpawnRandom(GetRandomWorldPos());
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        if (Random.value <= _dropChance)
            SpawnRandom(enemy.transform.position);
    }

    private void OnPhaseChanged(DayPhase phase)
    {
        _isNight = phase == DayPhase.Night;
        if (_isNight) _nightTimer = _nightRandomInterval;
    }

    private void SpawnRandom(Vector3 pos)
    {
        if (_powerUpPrefabs.Count == 0) return;
        var prefab = _powerUpPrefabs[Random.Range(0, _powerUpPrefabs.Count)];
        Instantiate(prefab, pos, Quaternion.identity);
    }

    public void SpawnSaved(PowerUpType type, Vector3 pos)
    {
        foreach (GameObject prefab in _powerUpPrefabs)
        {
            if (prefab == null) continue;
            var powerUp = prefab.GetComponent<PowerUp>();
            if (powerUp == null || powerUp.Type != type) continue;

            Instantiate(prefab, pos, Quaternion.identity);
            return;
        }
    }

    public List<PowerUpData> GetPowerUpDataSet()
    {
        var dataSet = new List<PowerUpData>();
        foreach (GameObject prefab in _powerUpPrefabs)
        {
            if (prefab == null) continue;
            var powerUp = prefab.GetComponent<PowerUp>();
            if (powerUp != null && powerUp.Data != null && !dataSet.Contains(powerUp.Data))
                dataSet.Add(powerUp.Data);
        }

        return dataSet;
    }

    private Vector3 GetRandomWorldPos()
    {
        return new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0);
    }
}
