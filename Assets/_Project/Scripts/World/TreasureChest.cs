using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float _rewardRevealDelay = 0.15f;
    [SerializeField] private bool _opened;
    [SerializeField] private bool _allowOpenAgain;

    [Header("Interaction UI")]
    [SerializeField] private GameObject _promptRoot;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Vector3 _promptOffset = new Vector3(0f, 1.15f, 0f);
    [SerializeField] private Color _affordableColor = new Color(1f, 0.88f, 0.3f, 1f);
    [SerializeField] private Color _notAffordableColor = new Color(1f, 0.3f, 0.25f, 1f);

    private static readonly int OpenHash = Animator.StringToHash("Open");
    private PlayerToolHandler _nearbyPlayer;
    private PlayerStats _playerStats;
    private bool _rolling;
    private float _messageUntil;
    private string _temporaryMessage;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        BuildRuntimePromptIfMissing();
        SetPromptVisible(false);
    }

    private void Start()
    {
        ResolvePlayerStats();
    }

    private void Update()
    {
        PlayerToolHandler player = FindNearbyPlayer();
        _nearbyPlayer = player;

        if (_playerStats == null)
            ResolvePlayerStats();

        bool canOpen = !_opened || _allowOpenAgain;
        SetPromptVisible(canOpen && !_rolling);
        RefreshPrompt(player != null);

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
        if (player == null || _rolling) return;
        if (_opened && !_allowOpenAgain) return;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        GachaReward reward = PickReward();
        if (reward == null)
        {
            Debug.LogWarning($"{name} has no gacha rewards.");
            ShowTemporaryMessage("Rương chưa có phần thưởng!", 1.5f);
            return;
        }

        if (!stats.SpendGold(_openCost))
        {
            Debug.Log($"Need {_openCost} gold to open {_chestType} chest.");
            ShowTemporaryMessage($"Không đủ vàng! Cần {_openCost}", 1.25f);
            return;
        }

        StartCoroutine(OpenRoutine(player, reward));
    }

    private IEnumerator OpenRoutine(PlayerToolHandler player, GachaReward reward)
    {
        _opened = true;
        _rolling = true;
        SetPromptVisible(false);

        float animationDuration = PlayOpenAnimation();
        StartCoroutine(PlayOpenPulse());
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.15f, 0.06f);

        if (animationDuration > 0f)
        {
            // Stop just before the end so imported looping clips remain on
            // their open frame instead of immediately returning to frame 1.
            yield return new WaitForSecondsRealtime(animationDuration * 0.95f);
            if (_animator != null)
                _animator.speed = 0f;
        }

        if (_rewardRevealDelay > 0f)
            yield return new WaitForSecondsRealtime(_rewardRevealDelay);

        GachaRollUI.Show(_rewards, reward, _chestType.ToString(), () =>
        {
            reward.Give(player);
            Debug.Log($"Opened {_chestType} chest for {_openCost} gold: {reward.GetResultText()}");
            _rolling = false;
        });
    }

    private float PlayOpenAnimation()
    {
        if (_animator == null || _animator.runtimeAnimatorController == null) return 0f;

        _animator.enabled = true;
        _animator.speed = 1f;
        _animator.Update(0f);

        string triggerName = string.IsNullOrWhiteSpace(_openTriggerName) ? "Open" : _openTriggerName;
        if (HasTrigger(triggerName))
        {
            if (triggerName == "Open")
                _animator.SetTrigger(OpenHash);
            else
                _animator.SetTrigger(triggerName);

            return GetLongestAnimationDuration();
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
        _animator.Update(0f);
        return GetLongestAnimationDuration();
    }

    private float GetLongestAnimationDuration()
    {
        RuntimeAnimatorController controller = _animator != null
            ? _animator.runtimeAnimatorController
            : null;

        if (controller == null || controller.animationClips == null) return 0f;

        float duration = 0f;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip != null)
                duration = Mathf.Max(duration, clip.length);
        }

        return duration;
    }

    private IEnumerator PlayOpenPulse()
    {
        Vector3 baseScale = transform.localScale;
        const float duration = 0.22f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float scale = 1f + Mathf.Sin(progress * Mathf.PI) * 0.12f;
            transform.localScale = baseScale * scale;
            yield return null;
        }

        transform.localScale = baseScale;
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

    private void ResolvePlayerStats()
    {
        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        _playerStats = player != null ? player.GetComponent<PlayerStats>() : null;
    }

    private void RefreshPrompt(bool playerIsNearby)
    {
        if (_priceText == null) return;

        if (!string.IsNullOrEmpty(_temporaryMessage) && Time.unscaledTime < _messageUntil)
        {
            _priceText.text = _temporaryMessage;
            _priceText.color = _notAffordableColor;
            return;
        }

        _temporaryMessage = null;
        bool canAfford = _playerStats != null && _playerStats.Gold >= _openCost;
        _priceText.text = playerIsNearby
            ? $"[{_openKey}] Mở - {_openCost} vàng"
            : $"{GetChestDisplayName()} - {_openCost} vàng";
        _priceText.color = canAfford ? _affordableColor : _notAffordableColor;
    }

    private void ShowTemporaryMessage(string message, float duration)
    {
        _temporaryMessage = message;
        _messageUntil = Time.unscaledTime + Mathf.Max(0.1f, duration);
        SetPromptVisible(true);
        RefreshPrompt(true);
    }

    private string GetChestDisplayName()
    {
        return _chestType switch
        {
            ChestType.Wood => "Rương gỗ",
            ChestType.Iron => "Rương sắt",
            ChestType.Gold => "Rương vàng",
            ChestType.Platinum => "Rương bạch kim",
            _ => "Rương"
        };
    }

    private void SetPromptVisible(bool visible)
    {
        if (_promptRoot != null && _promptRoot.activeSelf != visible)
            _promptRoot.SetActive(visible);
    }

    private void BuildRuntimePromptIfMissing()
    {
        if (_promptRoot != null && _priceText != null) return;

        GameObject root = new GameObject("ChestPricePrompt");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = _promptOffset;
        root.transform.localScale = Vector3.one * 0.01f;

        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(240f, 58f);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;

        Image background = root.AddComponent<Image>();
        background.color = new Color(0.04f, 0.05f, 0.07f, 0.88f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("PriceText");
        textObject.transform.SetParent(root.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 4f);
        textRect.offsetMax = new Vector2(-8f, -4f);

        _priceText = textObject.AddComponent<TextMeshProUGUI>();
        if (_priceText.font == null)
            _priceText.font = TMP_Settings.defaultFontAsset;
        _priceText.fontSize = 21f;
        _priceText.fontStyle = FontStyles.Bold;
        _priceText.alignment = TextAlignmentOptions.Center;
        _priceText.enableWordWrapping = false;
        _priceText.raycastTarget = false;

        _promptRoot = root;
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
