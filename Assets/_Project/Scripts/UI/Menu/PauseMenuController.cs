using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsPanel;

    private void Start()
    {
        EnsureSettingsMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_settingsPanel != null && _settingsPanel.activeSelf)
            {
                _settingsPanel.SetActive(false);
                return;
            }
            if (GameManager.Instance?.CurrentState == GameState.Playing) Pause();
            else if (GameManager.Instance?.CurrentState == GameState.Paused) Resume();
        }
    }

    public void Pause()
    {
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        GameManager.Instance?.PauseGame();
        if (_settingsPanel != null) _settingsPanel.SetActive(false); 
        if (_pausePanel != null) _pausePanel.SetActive(true);
    }

    public void Resume()
    {
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        GameManager.Instance?.ResumeGame();
        if (_settingsPanel != null) _settingsPanel.SetActive(false);
        if (_pausePanel != null) _pausePanel.SetActive(false);
    }

    public void OnSettingsPressed()
    {
        EnsureSettingsMenu();
        if (_settingsPanel != null) _settingsPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
        GameSaveController.SaveCurrentGame();
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadMainMenu();
    }

    private void EnsureSettingsMenu()
    {
        if (_settingsPanel == null) return;

        var settingsMenu = _settingsPanel.GetComponent<SettingsMenu>();
        if (settingsMenu == null) settingsMenu = _settingsPanel.AddComponent<SettingsMenu>();
        settingsMenu.Initialize();
    }
}
