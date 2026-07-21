using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;
    private Button _continueButton;

    public void OnPlayPressed()
    {
        PlayerStats.ResetProgress();
        PlayerController.ResetProgress();
        PlayerAnimator.ResetProgress();
        PlayerToolHandler.ResetProgress();
        PlayerGunInventory.ResetProgress();
        PlayerPetInventory.ResetProgress();
        LevelPortalBootstrap.ResetProgress();
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        GameSaveController.StartNewGame();
        SceneLoader.Instance?.LoadGameScene();
    }

    public void OnContinuePressed()
    {
        if (!SaveSystem.HasSave()) return;
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        GameSaveController.ContinueGame();
        SceneLoader.Instance?.LoadGameScene();
    }

    public void OnSettingsPressed()
    {
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        if (_settingsPanel != null) _settingsPanel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _settingsPanel != null && _settingsPanel.activeSelf)
            _settingsPanel.SetActive(false);
    }

    public void OnQuitPressed()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void Start()
    {
        EnsureSettingsMenu();
        ResolveContinueButton();
        RefreshContinueButton();
        AudioManager.Instance?.PlayBGM("bgm_main_menu");
    }

    private void EnsureSettingsMenu()
    {
        if (_settingsPanel == null) return;

        var settingsMenu = _settingsPanel.GetComponent<SettingsMenu>();
        if (settingsMenu == null) settingsMenu = _settingsPanel.AddComponent<SettingsMenu>();
        settingsMenu.Initialize();
    }

    private void ResolveContinueButton()
    {
        if (_continueButton != null) return;

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (button.name == "Btn_Continue")
            {
                _continueButton = button;
                return;
            }
        }
    }

    private void RefreshContinueButton()
    {
        if (_continueButton != null)
            _continueButton.interactable = SaveSystem.HasSave();
    }
}
