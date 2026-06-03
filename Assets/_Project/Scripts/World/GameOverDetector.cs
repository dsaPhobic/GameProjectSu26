using UnityEngine;

public class GameOverDetector : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnPlayerDied += TriggerGameOver;
        GameEvents.OnAllCropsDestroyed += TriggerGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= TriggerGameOver;
        GameEvents.OnAllCropsDestroyed -= TriggerGameOver;
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
        GameManager.Instance?.TriggerGameOver();
        SceneLoader.Instance?.LoadGameOver();
    }
}
