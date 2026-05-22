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

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    private void Start()
    {
        AudioManager.Instance?.PlayBGM("bgm_main_menu");
    }
}
