using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _maxHP = 100;
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _startingGold = 25;

    private int _currentHP;
    private int _level = 1;
    private int _xp;
    private int _gold;
    private bool _isDead;
    private static bool _sharedGoldInitialized;
    private static int _sharedGold;
    private static bool _sharedProgressInitialized;
    private static int _sharedMaxHP;
    private static int _sharedCurrentHP;
    private static int _sharedDamage;
    private static float _sharedAttackSpeed;
    private static float _sharedMoveSpeed;
    private static int _sharedLevel;
    private static int _sharedXP;

    public int MaxHP => _maxHP;
    public int CurrentHP => _currentHP;
    public int Damage => _damage;
    public float AttackSpeed => _attackSpeed;
    public float MoveSpeed => _moveSpeed;
    public int Level => _level;
    public int XP => _xp;
    public int Gold => _gold;

    [SerializeField] private int _xpPerLevel = 100;

    public int XPToNextLevel => _xpPerLevel * _level;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSharedRuntimeState()
    {
        ResetSharedStateForNewGame();
    }

    public static void ResetSharedStateForNewGame()
    {
        _sharedGoldInitialized = false;
        _sharedGold = 0;
        _sharedProgressInitialized = false;
    }

    public static void ResetProgress()
    {
        _sharedGoldInitialized = false;
        _sharedGold = 0;
        _sharedProgressInitialized = false;
    }

    private void Awake()
    {
        if (!_sharedProgressInitialized)
        {
            _sharedMaxHP = _maxHP;
            _sharedCurrentHP = _maxHP;
            _sharedDamage = _damage;
            _sharedAttackSpeed = _attackSpeed;
            _sharedMoveSpeed = _moveSpeed;
            _sharedLevel = 1;
            _sharedXP = 0;
            _sharedProgressInitialized = true;
        }

        _maxHP = _sharedMaxHP;
        _currentHP = _sharedCurrentHP;
        _damage = _sharedDamage;
        _attackSpeed = _sharedAttackSpeed;
        _moveSpeed = _sharedMoveSpeed;
        _level = _sharedLevel;
        _xp = _sharedXP;
        _isDead = _currentHP <= 0;

        if (!_sharedGoldInitialized)
        {
            _sharedGold = _startingGold;
            _sharedGoldInitialized = true;
        }

        _gold = _sharedGold;
    }

    private void SyncSharedProgress()
    {
        _sharedMaxHP = _maxHP;
        _sharedCurrentHP = _currentHP;
        _sharedDamage = _damage;
        _sharedAttackSpeed = _attackSpeed;
        _sharedMoveSpeed = _moveSpeed;
        _sharedLevel = _level;
        _sharedXP = _xp;
    }

    private void OnEnable() => GameEvents.OnEnemyDied += HandleEnemyKilled;
    private void OnDisable() => GameEvents.OnEnemyDied -= HandleEnemyKilled;

    private void HandleEnemyKilled(Enemy enemy)
    {
        if (enemy == null || enemy.Data == null) return;
        AddXP(enemy.Data.xpReward);
        AddGold(enemy.Data.goldReward);
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHP = Mathf.Max(0, _currentHP - amount);
        SyncSharedProgress();
        GameEvents.RaisePlayerHPChanged(_currentHP);
        if (_currentHP == 0)
        {
            _isDead = true;
            AudioManager.Instance?.PlaySFX("sfx_die");
            GameEvents.RaisePlayerDied();
        }
    }

    public void Heal(int amount)
    {
        _currentHP = Mathf.Min(_maxHP, _currentHP + amount);
        _isDead = _currentHP <= 0;
        SyncSharedProgress();
        GameEvents.RaisePlayerHPChanged(_currentHP);
    }

    public void AddXP(int amount)
    {
        _xp += amount;
        while (_xp >= _xpPerLevel * _level)
        {
            _xp -= _xpPerLevel * _level;
            LevelUp();
        }
        SyncSharedProgress();
        GameEvents.RaisePlayerXPChanged(_xp);
    }

    public void AddGold(int amount)
    {
        _sharedGold += amount;
        _gold = _sharedGold;
        GameEvents.RaiseGoldChanged(_gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (_sharedGold < amount) return false;

        _sharedGold -= amount;
        _gold = _sharedGold;
        GameEvents.RaiseGoldChanged(_gold);
        return true;
    }

    public void ModifyMaxHP(int delta) { _maxHP += delta; Heal(0); }
    public void ModifyDamage(int delta) { _damage += delta; SyncSharedProgress(); }
    public void ModifyAttackSpeed(float delta) { _attackSpeed += delta; SyncSharedProgress(); }
    public void ModifyMoveSpeed(float delta) { _moveSpeed += delta; SyncSharedProgress(); }

    public void LoadState(int maxHP, int currentHP, int level, int xp, int gold, int damage, float attackSpeed, float moveSpeed)
    {
        _maxHP = Mathf.Max(1, maxHP);
        _currentHP = Mathf.Clamp(currentHP, 0, _maxHP);
        _level = Mathf.Max(1, level);
        _xp = Mathf.Max(0, xp);
        _damage = Mathf.Max(0, damage);
        _attackSpeed = Mathf.Max(0.1f, attackSpeed);
        _moveSpeed = Mathf.Max(0f, moveSpeed);
        _isDead = _currentHP <= 0;
        SyncSharedProgress();

        _sharedGold = Mathf.Max(0, gold);
        _sharedGoldInitialized = true;
        _gold = _sharedGold;

        GameEvents.RaisePlayerHPChanged(_currentHP);
        GameEvents.RaisePlayerXPChanged(_xp);
        GameEvents.RaisePlayerLevelUp(_level);
        GameEvents.RaiseGoldChanged(_gold);
    }

    private void LevelUp()
    {
        _level++;
        AudioManager.Instance?.PlaySFX("sfx_levelup");
        GameEvents.RaisePlayerLevelUp(_level);
        GameEvents.RaiseLevelUpScreenOpen();
    }
}
