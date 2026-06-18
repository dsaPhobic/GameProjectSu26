using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHP = 100;

    protected int _currentHP;

    public bool IsDead => _currentHP <= 0;
    public int CurrentHP => _currentHP;
    public int MaxHP => maxHP;

    protected virtual void Awake()
    {
        _currentHP = maxHP;
    }

    public virtual void TakeDamage(int damage)
    {
        if (IsDead) return;
        _currentHP -= damage;
        OnDamaged(damage);
        if (IsDead) Die();
    }

    protected virtual void OnDamaged(int damage) { }

    protected virtual void Die() { }
}
