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

    private Vector3 GetRandomWorldPos()
    {
        return new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0);
    }
}
