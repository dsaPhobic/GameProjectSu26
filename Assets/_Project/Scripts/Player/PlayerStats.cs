using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _maxHP = 100;
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _moveSpeed = 5f;

    private int _currentHP;
    private int _level = 1;
    private int _xp;
    private int _gold;

    public int MaxHP => _maxHP;
    public int CurrentHP => _currentHP;
    public int Damage => _damage;
    public float AttackSpeed => _attackSpeed;
    public float MoveSpeed => _moveSpeed;
    public int Level => _level;
    public int XP => _xp;
    public int Gold => _gold;

    [SerializeField] private int _xpPerLevel = 100;

    private void Awake()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(int amount)
    {
        _currentHP = Mathf.Max(0, _currentHP - amount);
        GameEvents.RaisePlayerHPChanged(_currentHP);
        if (_currentHP == 0) GameEvents.RaisePlayerDied();
    }

    public void Heal(int amount)
    {
        _currentHP = Mathf.Min(_maxHP, _currentHP + amount);
        GameEvents.RaisePlayerHPChanged(_currentHP);
    }

    public void AddXP(int amount)
    {
        _xp += amount;
        GameEvents.RaisePlayerXPChanged(_xp);
        if (_xp >= _xpPerLevel * _level) LevelUp();
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        GameEvents.RaiseGoldChanged(_gold);
    }

    public void ModifyMaxHP(int delta) { _maxHP += delta; Heal(0); }
    public void ModifyDamage(int delta) => _damage += delta;
    public void ModifyAttackSpeed(float delta) => _attackSpeed += delta;
    public void ModifyMoveSpeed(float delta) => _moveSpeed += delta;

    private void LevelUp()
    {
        _level++;
        GameEvents.RaisePlayerLevelUp(_level);
        GameEvents.RaiseLevelUpScreenOpen();
    }
}
