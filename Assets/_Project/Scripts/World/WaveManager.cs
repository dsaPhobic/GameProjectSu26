using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<EnemyWaveData> _waveDataList;
    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private List<GameObject> _enemyPrefabs;

    [SerializeField] private float _hpScalePerDay = 1.1f;
    [SerializeField] private float _countScalePerDay = 1.2f;
    [SerializeField] private float _leftSpawnMinX = -16.2f;
    [SerializeField] private float _topSpawnMaxY = 16.2f;

    private int _currentDay = 1;
    private int _activeEnemyCount;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<WaveManager>();
        GameEvents.OnEnemyDied -= OnEnemyDied;
    }

    private void Start()
    {
        GameEvents.OnEnemyDied += OnEnemyDied;
    }

    public void StartWave(int day)
    {
        _currentDay = day;
        StartCoroutine(SpawnWave(day));
        GameEvents.RaiseWaveStarted(day);
    }

    private IEnumerator SpawnWave(int day)
    {
        EnemyWaveData waveData = GetWaveData(day);
        if (waveData == null) yield break;

        float hpMult = Mathf.Pow(_hpScalePerDay, day - 1);

        foreach (var entry in waveData.spawnEntries)
        {
            int count = Mathf.RoundToInt(entry.count * Mathf.Pow(_countScalePerDay, day - 1));
            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(entry.enemyType);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(EnemyType type)
    {
        if (_spawnPoints.Count == 0) return;
        var point = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        var prefab = GetPrefabForType(type);
        if (prefab == null) return;
        Instantiate(prefab, GetSafeSpawnPosition(point), Quaternion.identity);
        _activeEnemyCount++;
    }

    private Vector3 GetSafeSpawnPosition(SpawnPoint point)
    {
        Vector3 position = point.transform.position;

        if (position.x < _leftSpawnMinX)
            position.x = _leftSpawnMinX;

        if (position.y > _topSpawnMaxY)
            position.y = _topSpawnMaxY;

        return position;
    }

    private EnemyWaveData GetWaveData(int day)
    {
        foreach (var wd in _waveDataList)
            if (wd.dayNumber == day) return wd;
        return _waveDataList.Count > 0 ? _waveDataList[^1] : null;
    }

    private GameObject GetPrefabForType(EnemyType type)
    {
        foreach (var prefab in _enemyPrefabs)
        {
            if (prefab.TryGetComponent<Enemy>(out var e) && e.Data?.enemyType == type)
                return prefab;
        }
        return _enemyPrefabs.Count > 0 ? _enemyPrefabs[0] : null;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        _activeEnemyCount = Mathf.Max(0, _activeEnemyCount - 1);
        if (_activeEnemyCount == 0)
            GameEvents.RaiseWaveCompleted();
    }
}
