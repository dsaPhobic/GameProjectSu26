using UnityEngine;

public class TreasureChest : MonoBehaviour, IInteractable
{
    public enum ChestType
    {
        Wood,
        Iron,
        Gold,
        Platinum
    }

    [Header("Chest")]
    [SerializeField] private ChestType _chestType = ChestType.Wood;
    [SerializeField] private int _openCost = 25;
    [SerializeField] private float _interactRange = 1.4f;
    [SerializeField] private KeyCode _openKey = KeyCode.E;

    [Header("Rewards")]
    [SerializeField] private GachaReward[] _rewards;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _openTriggerName = "Open";
    [SerializeField] private bool _opened;
    [SerializeField] private bool _allowOpenAgain;

    private static readonly int OpenHash = Animator.StringToHash("Open");
    private PlayerToolHandler _nearbyPlayer;
    private bool _rolling;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerToolHandler player = FindNearbyPlayer();
        _nearbyPlayer = player;

        if (player == null || _rolling) return;
        if (Input.GetKeyDown(_openKey))
            TryOpen(player);
    }

    public bool CanInteract(ToolType tool)
    {
        return !_opened || _allowOpenAgain;
    }

    public void Interact(PlayerToolHandler player)
    {
        TryOpen(player);
    }

    private void TryOpen(PlayerToolHandler player)
    {
        if (player == null) return;
        if (_opened && !_allowOpenAgain) return;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        if (!stats.SpendGold(_openCost))
        {
            Debug.Log($"Need {_openCost} gold to open {_chestType} chest.");
            return;
        }

        GachaReward reward = PickReward();
        if (reward == null)
        {
            Debug.LogWarning($"{name} has no gacha rewards.");
            stats.AddGold(_openCost);
            return;
        }

        _opened = true;
        _rolling = true;
        PlayOpenAnimation();

        GachaRollUI.Show(_rewards, reward, _chestType.ToString(), () =>
        {
            reward.Give(player);
            Debug.Log($"Opened {_chestType} chest for {_openCost} gold: {reward.GetResultText()}");
            _rolling = false;
        });
    }

    private void PlayOpenAnimation()
    {
        if (_animator == null || _animator.runtimeAnimatorController == null) return;

        string triggerName = string.IsNullOrWhiteSpace(_openTriggerName) ? "Open" : _openTriggerName;
        if (HasTrigger(triggerName))
        {
            if (triggerName == "Open")
                _animator.SetTrigger(OpenHash);
            else
                _animator.SetTrigger(triggerName);
            return;
        }

        // Imported Iron/Gold/Platinum controllers contain a single open state
        // instead of an Open trigger, so play that state directly.
        string stateName = _chestType switch
        {
            ChestType.Iron => "IronChest_Open",
            ChestType.Gold => "GoldChest_Open",
            ChestType.Platinum => "PlatinumChest_Open",
            _ => "Open"
        };
        _animator.Play($"Base Layer.{stateName}", 0, 0f);
    }

    private bool HasTrigger(string parameterName)
    {
        foreach (AnimatorControllerParameter parameter in _animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger &&
                parameter.name == parameterName)
                return true;
        }

        return false;
    }

    private GachaReward PickReward()
    {
        if (_rewards == null || _rewards.Length == 0) return null;

        int totalWeight = 0;
        for (int i = 0; i < _rewards.Length; i++)
        {
            if (_rewards[i] != null)
                totalWeight += Mathf.Max(1, _rewards[i].weight);
        }

        int roll = Random.Range(0, totalWeight);
        for (int i = 0; i < _rewards.Length; i++)
        {
            if (_rewards[i] == null) continue;

            roll -= Mathf.Max(1, _rewards[i].weight);
            if (roll < 0)
                return _rewards[i];
        }

        return _rewards[0];
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

    private void OnValidate()
    {
        if (_openCost <= 0)
            _openCost = GetDefaultCost(_chestType);
    }

    private int GetDefaultCost(ChestType type)
    {
        return type switch
        {
            ChestType.Wood => 25,
            ChestType.Iron => 75,
            ChestType.Gold => 150,
            ChestType.Platinum => 300,
            _ => 25
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _nearbyPlayer != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
