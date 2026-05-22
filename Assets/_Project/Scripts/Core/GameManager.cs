using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Menu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<GameManager>();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateEntered(newState);
    }

    private void OnStateEntered(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.LevelUp:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    public void StartGame() => SetState(GameState.Playing);
    public void PauseGame() => SetState(GameState.Paused);
    public void ResumeGame() => SetState(GameState.Playing);
    public void OpenLevelUp() => SetState(GameState.LevelUp);
    public void TriggerGameOver() => SetState(GameState.GameOver);
}
