using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<EnemyWaveData> _waveDataList;
    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private List<GameObject> _enemyPrefabs;

    [SerializeField] private float _baseHpMultiplier = 1f;
    [SerializeField] private float _baseDamageMultiplier = 1f;
    [SerializeField] private float _hpScalePerDay = 1.1f;
    [SerializeField] private float _damageScalePerDay = 1f;
    [SerializeField] private float _countScalePerDay = 1.2f;
    [SerializeField] private float _leftSpawnMinX = -16.2f;
    [SerializeField] private float _topSpawnMaxY = 16.2f;

    private int _currentDay = 1;
    private int _activeEnemyCount;
    private bool _waveCompletionRaised;
    public int CurrentDay => _currentDay;

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
        _waveCompletionRaised = false;
        StartCoroutine(SpawnWave(day));
        GameEvents.RaiseWaveStarted(day);
    }

    public void SetCurrentDay(int day)
    {
        _currentDay = Mathf.Max(1, day);
    }

    private IEnumerator SpawnWave(int day)
    {
        EnemyWaveData waveData = GetWaveData(day);
        if (waveData == null) yield break;

        float hpMult = _baseHpMultiplier * Mathf.Pow(_hpScalePerDay, day - 1);
        float damageMult = _baseDamageMultiplier * Mathf.Pow(_damageScalePerDay, day - 1);

        foreach (var entry in waveData.spawnEntries)
        {
            int count = GetSpawnCount(entry, day);
            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(entry.enemyType, hpMult, damageMult);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(EnemyType type, float hpMultiplier, float damageMultiplier)
    {
        var point = GetRandomSpawnPoint();
        if (point == null) return;

        var prefab = GetPrefabForType(type);
        if (prefab == null) return;

        var enemyObject = Instantiate(prefab, GetSafeSpawnPosition(point), Quaternion.identity);
        if (enemyObject.TryGetComponent<Enemy>(out var enemy))
            enemy.ApplySpawnScaling(hpMultiplier, damageMultiplier);

        _activeEnemyCount++;
    }

    public void SpawnSavedEnemy(EnemyType type, Vector3 position, int hp)
    {
        var prefab = GetPrefabForType(type);
        if (prefab == null) return;

        var enemyObject = Instantiate(prefab, position, Quaternion.identity);
        if (enemyObject.TryGetComponent<Enemy>(out var enemy))
        {
            float hpMult = _baseHpMultiplier * Mathf.Pow(_hpScalePerDay, _currentDay - 1);
            float damageMult = _baseDamageMultiplier * Mathf.Pow(_damageScalePerDay, _currentDay - 1);
            enemy.ApplySpawnScaling(hpMult, damageMult);
            enemy.LoadHealth(hp);
        }

        _activeEnemyCount++;
    }

    public bool WaveContainsEnemy(int day, EnemyType type)
    {
        EnemyWaveData waveData = GetWaveData(day);
        if (waveData == null) return false;

        foreach (var entry in waveData.spawnEntries)
            if (entry.enemyType == type && entry.count > 0)
                return true;

        return false;
    }

    public Vector3 GetRestoreSpawnPosition()
    {
        var point = GetRandomSpawnPoint();
        return GetSafeSpawnPosition(point);
    }

    public void SetActiveEnemyCount(int count)
    {
        _activeEnemyCount = Mathf.Max(0, count);
        _waveCompletionRaised = false;
    }

    /// <summary>
    /// Ends the current boss encounter immediately. Remaining enemies are removed
    /// without awarding kill rewards, and pending spawns are cancelled.
    /// </summary>
    public void FinishBossEncounter(Enemy defeatedBoss)
    {
        StopAllCoroutines();

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy == null || enemy == defeatedBoss) continue;

            // Hide immediately; Destroy is deferred until the end of the frame.
            enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject);
        }

        _activeEnemyCount = 0;
        RaiseWaveCompletedOnce();
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        if (_spawnPoints == null || _spawnPoints.Count == 0) return null;

        _spawnPoints.RemoveAll(point => point == null);
        if (_spawnPoints.Count == 0) return null;

        return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
    }

    private Vector3 GetSafeSpawnPosition(SpawnPoint point)
    {
        if (point == null) return transform.position;

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
        return null;
    }

    private int GetSpawnCount(EnemyWaveData.SpawnEntry entry, int day)
    {
        if (entry.enemyType == EnemyType.DemonBoss)
            return entry.count;

        return Mathf.RoundToInt(entry.count * Mathf.Pow(_countScalePerDay, day - 1));
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
            RaiseWaveCompletedOnce();
    }

    private void RaiseWaveCompletedOnce()
    {
        if (_waveCompletionRaised) return;

        _waveCompletionRaised = true;
        GameEvents.RaiseWaveCompleted();
    }
}
