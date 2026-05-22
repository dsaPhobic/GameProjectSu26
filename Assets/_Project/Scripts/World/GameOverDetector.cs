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

    private void Update()
    {
        var farmManager = ServiceLocator.Get<FarmManager>();
        if (farmManager != null && farmManager.CountLivingCrops() == 0)
            GameEvents.RaiseAllCropsDestroyed();
    }

    private void TriggerGameOver()
    {
        GameManager.Instance?.TriggerGameOver();
        SceneLoader.Instance?.LoadGameOver();
    }
}
