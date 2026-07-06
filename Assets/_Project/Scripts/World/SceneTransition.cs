using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string _targetSceneName = "ShopInterior";
    [SerializeField] private string _targetSpawnId = "ShopEntrance";
    [SerializeField] private float _transitionCooldown = 0.5f;

    private static float _lastTransitionTime = -999f;
    private bool _isTransitioning;

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
        if (string.IsNullOrWhiteSpace(_targetSceneName)) return;

        _isTransitioning = true;
        _lastTransitionTime = Time.unscaledTime;
        SceneTransitionState.TargetSpawnId = _targetSpawnId;

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(_targetSceneName);
        else
            SceneManager.LoadScene(_targetSceneName);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
