using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _daysSurvivedText;
    [SerializeField] private TextMeshProUGUI _goldEarnedText;
    [SerializeField] private TextMeshProUGUI _enemiesKilledText;

    private static bool _nextResultIsWin;
    private static int _nextDaysSurvived;
    private static int _nextGoldEarned;
    private static int _nextEnemiesKilled;
    private bool _isRetrying;

    public static void SetNextResult(bool isWin)
    {
        _nextResultIsWin = isWin;
    }

    public static void SetNextResult(bool isWin, int daysSurvived, int goldEarned, int enemiesKilled)
    {
        _nextResultIsWin = isWin;
        _nextDaysSurvived = Mathf.Max(0, daysSurvived);
        _nextGoldEarned = Mathf.Max(0, goldEarned);
        _nextEnemiesKilled = Mathf.Max(0, enemiesKilled);
    }

    private void Start()
    {
        ResolveTitleText();
        ShowResults();
    }

    public void ShowResults()
    {
        if (_titleText != null) _titleText.text = _nextResultIsWin ? "GAME WIN" : "GAME OVER";
        if (_daysSurvivedText != null) _daysSurvivedText.text = $"Days Survived: {_nextDaysSurvived}";
        if (_goldEarnedText != null) _goldEarnedText.text = $"Gold Earned: {_nextGoldEarned}";
        if (_enemiesKilledText != null) _enemiesKilledText.text = $"Enemies Killed: {_nextEnemiesKilled}";
        AudioManager.Instance?.PlayBGM("bgm_game_over");
    }

    private void ResolveTitleText()
    {
        if (_titleText != null) return;

        var title = transform.Find("Text_Title");
        if (title != null)
            _titleText = title.GetComponent<TextMeshProUGUI>();
    }

    public void OnRetryPressed()
    {
        if (_isRetrying) return;
        _isRetrying = true;

        Time.timeScale = 1f;
        PlayerStats.ResetProgress();
        PlayerController.ResetProgress();
        PlayerAnimator.ResetProgress();
        PlayerToolHandler.ResetProgress();
        PlayerPetInventory.ResetProgress();
        LevelPortalBootstrap.ResetProgress();
        GameManager.Instance?.StartGame();
        SceneLoader.Instance?.LoadGameScene();
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadMainMenu();
    }
}
