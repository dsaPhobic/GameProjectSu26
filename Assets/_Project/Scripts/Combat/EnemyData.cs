using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy", menuName = "MagicFarm/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public EnemyType enemyType;
    public int maxHP = 50;
    public int damage = 10;
    public float moveSpeed = 2f;
    public int xpReward = 10;
    public int goldReward = 5;
    public float attackCooldown = 1.5f;
    public float attackRange = 1f;
    public float detectionRange = 8f;
}
