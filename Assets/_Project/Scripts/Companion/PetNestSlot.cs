using System.Collections;
using UnityEngine;

public class PetNestSlot : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float _interactRange = 1.4f;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    [Header("Visuals")]
    [SerializeField] private GameObject _emptyNestVisual;
    [SerializeField] private GameObject _eggVisualRoot;
    [SerializeField] private SpriteRenderer _eggRenderer;
    [SerializeField] private Animator _eggAnimator;
    [SerializeField] private string _hatchTriggerName = "Hatch";
    [SerializeField] private string _hatchedBoolName = "IsHatched";

    [Header("Pet")]
    [SerializeField] private Transform _petSpawnPoint;
    [SerializeField] private bool _parentPetToPlayer;

    private static readonly int HatchHash = Animator.StringToHash("Hatch");
    private static readonly int IsHatchedHash = Animator.StringToHash("IsHatched");

    private PetEggData _currentEgg;
    private float _hatchTimer;
    private bool _isHatching;
    private bool _isReadyToHatch;
    private PlayerToolHandler _nearbyPlayer;

    private void Awake()
    {
        if (_eggAnimator == null)
            _eggAnimator = GetComponentInChildren<Animator>(true);

        if (_eggVisualRoot == null && _eggAnimator != null)
            _eggVisualRoot = _eggAnimator.gameObject;

        if (_eggRenderer == null && _eggVisualRoot != null)
            _eggRenderer = _eggVisualRoot.GetComponentInChildren<SpriteRenderer>(true);

        SetEggVisible(false);
    }

    private void Update()
    {
        TickHatchTimer();

        _nearbyPlayer = FindNearbyPlayer();
        if (_nearbyPlayer == null || _isHatching) return;

        if (Input.GetKeyDown(_interactKey))
            Interact(_nearbyPlayer);
    }

    private void TickHatchTimer()
    {
        if (_currentEgg == null || _isReadyToHatch || _isHatching) return;

        _hatchTimer -= Time.deltaTime;
        if (_hatchTimer <= 0f)
        {
            _isReadyToHatch = true;
            StartCoroutine(HatchRoutine(FindAnyPlayer()));
        }
    }

    private void Interact(PlayerToolHandler player)
    {
        if (_currentEgg == null)
        {
            PlaceEgg(player);
            return;
        }

        if (_isReadyToHatch)
            StartCoroutine(HatchRoutine(player));
        else
            Debug.Log($"{_currentEgg.eggName} is hatching: {Mathf.CeilToInt(_hatchTimer)}s left.");
    }

    private void PlaceEgg(PlayerToolHandler player)
    {
        if (!PlayerPetInventory.TryConsumeAnyEgg(out PetEggData egg))
        {
            Debug.Log("You do not have any pet eggs.");
            return;
        }

        _currentEgg = egg;
        _hatchTimer = Mathf.Max(0f, egg.hatchSeconds);
        _isReadyToHatch = _hatchTimer <= 0f;
        _isHatching = false;

        if (_eggRenderer != null)
            _eggRenderer.sprite = egg.eggSprite;

        if (_eggAnimator != null && egg.hatchAnimatorController != null)
            _eggAnimator.runtimeAnimatorController = egg.hatchAnimatorController;

        SetEggVisible(true);
        Debug.Log($"Placed {egg.eggName}. Hatch time: {Mathf.CeilToInt(_hatchTimer)}s.");
    }

    private IEnumerator HatchRoutine(PlayerToolHandler player)
    {
        if (_currentEgg == null) yield break;

        _isHatching = true;
        PlayHatchAnimation();

        yield return new WaitForSeconds(1f);

        SpawnPet(player);
        ResetNest();
    }

    private void PlayHatchAnimation()
    {
        if (_eggAnimator == null) return;

        if (string.IsNullOrWhiteSpace(_hatchTriggerName) || _hatchTriggerName == "Hatch")
            _eggAnimator.SetTrigger(HatchHash);
        else
            _eggAnimator.SetTrigger(_hatchTriggerName);

        if (string.IsNullOrWhiteSpace(_hatchedBoolName) || _hatchedBoolName == "IsHatched")
            _eggAnimator.SetBool(IsHatchedHash, true);
        else
            _eggAnimator.SetBool(_hatchedBoolName, true);
    }

    private void SpawnPet(PlayerToolHandler player)
    {
        if (_currentEgg.petPrefab == null) return;

        Transform playerTransform = player != null ? player.transform : null;
        Vector3 spawnPosition = _petSpawnPoint != null ? _petSpawnPoint.position : transform.position;
        Transform parent = _parentPetToPlayer ? playerTransform : null;

        GameObject petObject = Instantiate(_currentEgg.petPrefab, spawnPosition, Quaternion.identity, parent);
        // Persistent objects must be roots. Drone still follows the current Player through SetFollowTarget.
        petObject.transform.SetParent(null, true);
        DontDestroyOnLoad(petObject);
        PlayerPetInventory.RegisterPersistentPet(petObject);
        if (petObject.TryGetComponent<Drone>(out var drone))
            drone.SetFollowTarget(playerTransform);
    }

    private void ResetNest()
    {
        _currentEgg = null;
        _hatchTimer = 0f;
        _isReadyToHatch = false;
        _isHatching = false;

        if (_eggAnimator != null)
            _eggAnimator.SetBool(IsHatchedHash, false);

        SetEggVisible(false);
    }

    private void SetEggVisible(bool visible)
    {
        if (_eggVisualRoot != null)
            _eggVisualRoot.SetActive(visible);

        if (_emptyNestVisual != null)
            _emptyNestVisual.SetActive(!visible);
    }

    private PlayerToolHandler FindNearbyPlayer()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (player == null) return null;

        float sqrRange = _interactRange * _interactRange;
        if (((Vector2)player.transform.position - (Vector2)transform.position).sqrMagnitude > sqrRange)
            return null;

        return player.GetComponent<PlayerToolHandler>();
    }

    private PlayerToolHandler FindAnyPlayer()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        return player != null ? player.GetComponent<PlayerToolHandler>() : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _nearbyPlayer != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
