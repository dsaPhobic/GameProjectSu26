using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    public struct PoolEntry
    {
        public EnemyType type;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxSize;
    }

    [SerializeField] private List<PoolEntry> _entries;
    private Dictionary<EnemyType, ObjectPool<GameObject>> _pools;

    private void Awake()
    {
        ServiceLocator.Register(this);
        _pools = new Dictionary<EnemyType, ObjectPool<GameObject>>();
        foreach (var entry in _entries)
        {
            var capturedEntry = entry;
            _pools[entry.type] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(capturedEntry.prefab),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: entry.defaultCapacity,
                maxSize: entry.maxSize);
        }
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<EnemyPool>();
    }

    public GameObject Get(EnemyType type)
    {
        return _pools.TryGetValue(type, out var pool) ? pool.Get() : null;
    }

    public void Return(EnemyType type, GameObject obj)
    {
        if (_pools.TryGetValue(type, out var pool))
            pool.Release(obj);
    }
}
