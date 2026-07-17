using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string _targetSceneName = "ShopInterior";
    [SerializeField] private string _targetSpawnId = "ShopEntrance";
    [SerializeField] private bool _saveReturnPoint = true;
    [SerializeField] private bool _returnToSavedScene;
    [SerializeField] private float _transitionCooldown = 0.5f;

    private static float _lastTransitionTime = -999f;
    private bool _isTransitioning;

    public void Configure(string targetSceneName, string targetSpawnId = "", bool saveReturnPoint = false)
    {
        _targetSceneName = targetSceneName;
        _targetSpawnId = targetSpawnId;
        _saveReturnPoint = saveReturnPoint;
        _returnToSavedScene = false;
    }

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTransitioning) return;
        if (Time.unscaledTime - _lastTransitionTime < _transitionCooldown) return;
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        string sceneToLoad = _targetSceneName;
        if (_returnToSavedScene)
        {
            if (!string.IsNullOrWhiteSpace(SceneTransitionState.ReturnSceneName))
            {
                sceneToLoad = SceneTransitionState.ReturnSceneName;
                SceneTransitionState.RestoreSavedReturnPointOnNextLoad();
            }
            else
            {
                SceneTransitionState.TargetSpawnId = _targetSpawnId;
            }
        }
        else
        {
            if (_saveReturnPoint)
                SceneTransitionState.SaveReturnPoint(SceneManager.GetActiveScene().name, other.transform.position);

            SceneTransitionState.TargetSpawnId = _targetSpawnId;
        }

        if (string.IsNullOrWhiteSpace(sceneToLoad)) return;

        _isTransitioning = true;
        _lastTransitionTime = Time.unscaledTime;

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(sceneToLoad);
        else
            SceneManager.LoadScene(sceneToLoad);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
