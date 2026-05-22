using System;

public static class GameEvents
{
    // Player events
    public static event Action<int> OnPlayerHPChanged;
    public static event Action<int> OnPlayerXPChanged;
    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnPlayerLevelUp;
    public static event Action OnPlayerDied;

    // Game phase events
    public static event Action<DayPhase> OnDayPhaseChanged;
    public static event Action<int> OnDayChanged;
    public static event Action<int> OnWaveStarted;
    public static event Action OnWaveCompleted;

    // Combat events
    public static event Action<Enemy> OnEnemyDied;
    public static event Action OnAllCropsDestroyed;

    // Upgrade events
    public static event Action OnLevelUpScreenOpen;

    public static void RaisePlayerHPChanged(int hp) => OnPlayerHPChanged?.Invoke(hp);
    public static void RaisePlayerXPChanged(int xp) => OnPlayerXPChanged?.Invoke(xp);
    public static void RaiseGoldChanged(int gold) => OnGoldChanged?.Invoke(gold);
    public static void RaisePlayerLevelUp(int level) => OnPlayerLevelUp?.Invoke(level);
    public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
    public static void RaiseDayPhaseChanged(DayPhase phase) => OnDayPhaseChanged?.Invoke(phase);
    public static void RaiseDayChanged(int day) => OnDayChanged?.Invoke(day);
    public static void RaiseWaveStarted(int wave) => OnWaveStarted?.Invoke(wave);
    public static void RaiseWaveCompleted() => OnWaveCompleted?.Invoke();
    public static void RaiseEnemyDied(Enemy enemy) => OnEnemyDied?.Invoke(enemy);
    public static void RaiseAllCropsDestroyed() => OnAllCropsDestroyed?.Invoke();
    public static void RaiseLevelUpScreenOpen() => OnLevelUpScreenOpen?.Invoke();
}
