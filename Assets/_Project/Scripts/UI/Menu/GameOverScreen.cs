using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _daysSurvivedText;
    [SerializeField] private TextMeshProUGUI _goldEarnedText;
    [SerializeField] private TextMeshProUGUI _enemiesKilledText;

    private static bool _nextResultIsWin;

    private int _daysSurvived;
    private int _goldEarned;
    private int _enemiesKilled;

    public static void SetNextResult(bool isWin)
    {
        _nextResultIsWin = isWin;
    }

    private void OnEnable()
    {
        GameEvents.OnDayChanged += OnDayChanged;
        GameEvents.OnGoldChanged += OnGoldChanged;
        GameEvents.OnEnemyDied += OnEnemyDied;
        _daysSurvived = 0;
        _goldEarned = 0;
        _enemiesKilled = 0;
    }

    private void OnDisable()
    {
        GameEvents.OnDayChanged -= OnDayChanged;
        GameEvents.OnGoldChanged -= OnGoldChanged;
        GameEvents.OnEnemyDied -= OnEnemyDied;
    }

    private void Start()
    {
        ResolveTitleText();
        ShowResults();
    }

    private void OnDayChanged(int d) => _daysSurvived = d;
    private void OnGoldChanged(int g) => _goldEarned = g;
    private void OnEnemyDied(Enemy _) => _enemiesKilled++;

    public void ShowResults()
    {
        if (_titleText != null) _titleText.text = _nextResultIsWin ? "GAME WIN" : "GAME OVER";
        if (_daysSurvivedText != null) _daysSurvivedText.text = $"Days Survived: {_daysSurvived}";
        if (_goldEarnedText != null) _goldEarnedText.text = $"Gold Earned: {_goldEarned}";
        if (_enemiesKilledText != null) _enemiesKilledText.text = $"Enemies Killed: {_enemiesKilled}";
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
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadGameScene();
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadMainMenu();
    }
}
