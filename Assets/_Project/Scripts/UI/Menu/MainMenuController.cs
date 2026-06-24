using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;

    public void OnPlayPressed()
    {
        AudioManager.Instance?.PlaySFX("sfx_button_click");
        SceneLoader.Instance?.LoadGameScene();
    }

    public void OnContinuePressed()
    {
        if (!SaveSystem.HasSave()) return;
        AudioManager.Instance?.PlaySFX("sfx_button_click");
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
        AudioManager.Instance?.PlayBGM("bgm_main_menu");
    }

    private void EnsureSettingsMenu()
    {
        if (_settingsPanel == null) return;

        var settingsMenu = _settingsPanel.GetComponent<SettingsMenu>();
        if (settingsMenu == null) settingsMenu = _settingsPanel.AddComponent<SettingsMenu>();
        settingsMenu.Initialize();
    }
}
