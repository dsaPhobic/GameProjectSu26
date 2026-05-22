using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance?.CurrentState == GameState.Playing) Pause();
            else if (GameManager.Instance?.CurrentState == GameState.Paused) Resume();
        }
    }

    public void Pause()
    {
        GameManager.Instance?.PauseGame();
        if (_pausePanel != null) _pausePanel.SetActive(true);
    }

    public void Resume()
    {
        GameManager.Instance?.ResumeGame();
        if (_pausePanel != null) _pausePanel.SetActive(false);
    }

    public void OnSettingsPressed()
    {
        if (_settingsPanel != null) _settingsPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadMainMenu();
    }
}
