using System.Collections;
using UnityEngine;

public class PetHatchery : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    [Header("Hatching")]
    [SerializeField] private Animator _eggAnimator;
    [SerializeField] private string _hatchTriggerName = "Hatch";
    [SerializeField] private string _hatchedBoolName = "IsHatched";
    [SerializeField] private float _hatchDuration = 2f;
    [SerializeField] private bool _startsHatching;
    [SerializeField] private bool _hideEggAfterHatch = true;
    [SerializeField] private GameObject _eggVisualRoot;

    [Header("Pet")]
    [SerializeField] private Drone _petToActivate;
    [SerializeField] private GameObject _petPrefab;
    [SerializeField] private Transform _petSpawnPoint;
    [SerializeField] private bool _parentPetToPlayer = false;

    private static readonly int HatchHash = Animator.StringToHash("Hatch");
    private static readonly int IsHatchedHash = Animator.StringToHash("IsHatched");

    private bool _isHatching;
    private bool _isHatched;
    private PlayerToolHandler _nearbyPlayer;

    private void Awake()
    {
        if (_eggAnimator == null)
            _eggAnimator = GetComponentInChildren<Animator>();

        if (_eggVisualRoot == null && _eggAnimator != null)
            _eggVisualRoot = _eggAnimator.gameObject;
    }

    private void Start()
    {
        if (_startsHatching)
            BeginHatching();
    }

    private void Update()
    {
        _nearbyPlayer = FindNearbyPlayer();
        if (_nearbyPlayer == null || _isHatching || _isHatched) return;

        if (Input.GetKeyDown(_interactKey))
            BeginHatching();
    }

    public bool CanInteract(ToolType tool)
    {
        return !_isHatching && !_isHatched;
    }

    public void Interact(PlayerToolHandler player)
    {
        if (CanInteract(player.CurrentTool))
            BeginHatching();
    }

    private void BeginHatching()
    {
        if (_isHatching || _isHatched) return;
        StartCoroutine(HatchRoutine());
    }

    private IEnumerator HatchRoutine()
    {
        _isHatching = true;
        PlayHatchAnimation();

        yield return new WaitForSeconds(Mathf.Max(0f, _hatchDuration));

        _isHatching = false;
        _isHatched = true;
        SetHatchedAnimation();
        UnlockPet();
        HideEggVisual();
    }

    private void PlayHatchAnimation()
    {
        if (_eggAnimator == null) return;

        if (string.IsNullOrWhiteSpace(_hatchTriggerName) || _hatchTriggerName == "Hatch")
            _eggAnimator.SetTrigger(HatchHash);
        else
            _eggAnimator.SetTrigger(_hatchTriggerName);
    }

    private void SetHatchedAnimation()
    {
        if (_eggAnimator == null) return;

        if (string.IsNullOrWhiteSpace(_hatchedBoolName) || _hatchedBoolName == "IsHatched")
            _eggAnimator.SetBool(IsHatchedHash, true);
        else
            _eggAnimator.SetBool(_hatchedBoolName, true);
    }

    private void UnlockPet()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        Transform playerTransform = player != null ? player.transform : null;

        Drone pet = _petToActivate;
        if (pet == null && _petPrefab != null)
        {
            Vector3 spawnPosition = _petSpawnPoint != null ? _petSpawnPoint.position : transform.position;
            Transform parent = _parentPetToPlayer ? playerTransform : null;
            GameObject petObject = Instantiate(_petPrefab, spawnPosition, Quaternion.identity, parent);
            pet = petObject.GetComponent<Drone>();
        }

        if (pet == null) return;

        pet.gameObject.SetActive(true);
        pet.SetFollowTarget(playerTransform);
    }

    private void HideEggVisual()
    {
        if (!_hideEggAfterHatch || _eggVisualRoot == null) return;

        _eggVisualRoot.SetActive(false);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _nearbyPlayer != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
