using UnityEngine;

public class GameOverDetector : MonoBehaviour
{
    private bool _endingTriggered;

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += TriggerGameOver;
        GameEvents.OnAllCropsDestroyed += TriggerGameOver;
        GameEvents.OnEnemyDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= TriggerGameOver;
        GameEvents.OnAllCropsDestroyed -= TriggerGameOver;
        GameEvents.OnEnemyDied -= OnEnemyDied;
    }

    private bool _cropsEverPlanted = false;

    private void Update()
    {
        var farmManager = ServiceLocator.Get<FarmManager>();
        if (farmManager == null) return;

        if (!_cropsEverPlanted && farmManager.CountLivingCrops() > 0)
            _cropsEverPlanted = true;

        if (_cropsEverPlanted && farmManager.CountLivingCrops() == 0)
            GameEvents.RaiseAllCropsDestroyed();
    }

    private void TriggerGameOver()
    {
        if (_endingTriggered) return;
        _endingTriggered = true;
        GameOverScreen.SetNextResult(false);
        GameManager.Instance?.TriggerGameOver();
        AudioManager.Instance?.PlayBGM("bgm_game_over");
        SceneLoader.Instance?.LoadGameOver();
    }

    private void TriggerGameWin()
    {
        if (_endingTriggered) return;
        _endingTriggered = true;
        GameOverScreen.SetNextResult(true);
        GameManager.Instance?.TriggerGameOver();
        AudioManager.Instance?.PlayBGM("bgm_game_over");
        SceneLoader.Instance?.LoadGameOver();
    }

    private void OnEnemyDied(Enemy enemy)
    {
        if (enemy != null && enemy.Data != null && enemy.Data.enemyType == EnemyType.DemonBoss)
            TriggerGameWin();
    }
}
