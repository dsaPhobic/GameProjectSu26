using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Wave", menuName = "MagicFarm/Enemy Wave Data")]
public class EnemyWaveData : ScriptableObject
{
    [Serializable]
    public struct SpawnEntry
    {
        public EnemyType enemyType;
        public int count;
        public float spawnInterval;
    }

    public int dayNumber;
    public List<SpawnEntry> spawnEntries = new();
}
