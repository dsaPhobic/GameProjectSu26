using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _daysSurvivedText;
    [SerializeField] private TextMeshProUGUI _goldEarnedText;
    [SerializeField] private TextMeshProUGUI _enemiesKilledText;

    private int _daysSurvived;
    private int _goldEarned;
    private int _enemiesKilled;

    private void OnEnable()
    {
        GameEvents.OnDayChanged += d => _daysSurvived = d;
        GameEvents.OnGoldChanged += g => _goldEarned = g;
        GameEvents.OnEnemyDied += _ => _enemiesKilled++;
    }

    private void OnDisable()
    {
        GameEvents.OnDayChanged -= d => _daysSurvived = d;
        GameEvents.OnGoldChanged -= g => _goldEarned = g;
        GameEvents.OnEnemyDied -= _ => _enemiesKilled++;
    }

    public void ShowResults()
    {
        if (_daysSurvivedText != null) _daysSurvivedText.text = $"Days Survived: {_daysSurvived}";
        if (_goldEarnedText != null) _goldEarnedText.text = $"Gold Earned: {_goldEarned}";
        if (_enemiesKilledText != null) _enemiesKilledText.text = $"Enemies Killed: {_enemiesKilled}";
        AudioManager.Instance?.PlayBGM("bgm_game_over");
    }

    public void OnRetryPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadGameScene();
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadMainMenu();
    }
}
