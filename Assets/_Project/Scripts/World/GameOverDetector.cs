using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverDetector : MonoBehaviour
{
    private bool _endingTriggered;
    private int _currentDay = 1;
    private int _goldEarned;
    private int _lastGold;
    private int _enemiesKilled;
    private bool _goldInitialized;

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += TriggerGameOver;
        GameEvents.OnAllCropsDestroyed += TriggerGameOver;
        GameEvents.OnDayChanged += OnDayChanged;
        GameEvents.OnGoldChanged += OnGoldChanged;
        GameEvents.OnEnemyDied += OnEnemyDied;
    }

    private void Start()
    {
        var playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            _lastGold = playerStats.Gold;
            _goldInitialized = true;
        }
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= TriggerGameOver;
        GameEvents.OnAllCropsDestroyed -= TriggerGameOver;
        GameEvents.OnDayChanged -= OnDayChanged;
        GameEvents.OnGoldChanged -= OnGoldChanged;
        GameEvents.OnEnemyDied -= OnEnemyDied;
    }

    private void TriggerGameOver()
    {
        if (_endingTriggered) return;
        _endingTriggered = true;
        GameOverScreen.SetNextResult(false, _currentDay, _goldEarned, _enemiesKilled);
        GameManager.Instance?.TriggerGameOver();
        AudioManager.Instance?.PlayBGM("bgm_game_over");
        SceneLoader.Instance?.LoadGameOver();
    }

    private void TriggerGameWin()
    {
        if (_endingTriggered) return;
        _endingTriggered = true;
        GameOverScreen.SetNextResult(true, _currentDay, _goldEarned, _enemiesKilled);
        GameManager.Instance?.TriggerGameOver();
        AudioManager.Instance?.PlayBGM("bgm_game_over");
        SceneLoader.Instance?.LoadGameOver();
    }

    private void OnDayChanged(int day)
    {
        _currentDay = Mathf.Max(1, day);
    }

    private void OnGoldChanged(int gold)
    {
        if (!_goldInitialized)
        {
            _lastGold = gold;
            _goldInitialized = true;
            return;
        }

        if (gold > _lastGold)
            _goldEarned += gold - _lastGold;

        _lastGold = gold;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        _enemiesKilled++;

        if (SceneManager.GetActiveScene().name == "GameScene2" &&
            enemy != null && enemy.Data != null && enemy.Data.enemyType == EnemyType.DemonBoss)
            TriggerGameWin();
    }
}
