using UnityEngine;

public class PlayerToolHandler : MonoBehaviour
{
    [SerializeField] private ToolType _startingTool = ToolType.Gun;

    public ToolType CurrentTool { get; private set; }

    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private LayerMask _farmTileLayer;
    [SerializeField] private CropData[] _availableSeeds;
    [SerializeField] private int _startingSeedCount = 5;

    [Header("Tool Visuals")]
    [SerializeField] private SpriteRenderer _toolRenderer;
    [SerializeField] private Sprite _hoeSprite;
    [SerializeField] private Sprite _wateringCanSprite;
    [SerializeField] private Sprite _swordSprite;
    [SerializeField] private Sprite _gunSprite;

    private int _selectedSeedIndex = 0;
    private int[] _seedCounts;
    private Vector3 _defaultToolLocalPosition;

    public CropData SelectedSeed => (_availableSeeds != null && _availableSeeds.Length > 0)
        ? _availableSeeds[_selectedSeedIndex] : null;

    public CropData[] AvailableSeeds => _availableSeeds;
    public int SelectedSeedIndex => _selectedSeedIndex;
    public string SelectedSeedName => SelectedSeed != null ? SelectedSeed.cropName : "None";

    public int GetSeedCount(int index)
    {
        if (_seedCounts == null || index < 0 || index >= _seedCounts.Length) return 0;
        return _seedCounts[index];
    }

    public void AddSeed(int index, int amount = 1)
    {
        if (_seedCounts == null || index < 0 || index >= _seedCounts.Length) return;
        _seedCounts[index] += amount;
        GameEvents.RaiseSeedCountChanged(index, _seedCounts[index]);
    }

    private void Awake()
    {
        if (_toolRenderer != null)
            _defaultToolLocalPosition = _toolRenderer.transform.localPosition;

        if (_gunSprite == null && _toolRenderer != null)
            _gunSprite = _toolRenderer.sprite;

        CurrentTool = _startingTool;

        if (_availableSeeds != null)
        {
            _seedCounts = new int[_availableSeeds.Length];
            for (int i = 0; i < _seedCounts.Length; i++)
                _seedCounts[i] = _startingSeedCount;
        }
        UpdateToolSprite();
    }

    public void SwitchTool(int slot)
    {
        CurrentTool = slot switch
        {
            1 => ToolType.Hoe,
            2 => ToolType.WateringCan,
            3 => ToolType.Sword,
            4 => ToolType.Gun,
            _ => CurrentTool
        };
        UpdateToolSprite();
    }

    public void EquipGun()
    {
        CurrentTool = ToolType.Gun;
        UpdateToolSprite();
    }

    public void FlipTool(bool facingLeft)
    {
        if (_toolRenderer == null) return;
        if (CurrentTool == ToolType.Gun) return;

        _toolRenderer.transform.localRotation = Quaternion.identity;
        _toolRenderer.flipY = false;
        _toolRenderer.flipX = facingLeft;
        Vector3 pos = _defaultToolLocalPosition;
        pos.x = facingLeft ? -Mathf.Abs(pos.x) : Mathf.Abs(pos.x);
        _toolRenderer.transform.localPosition = pos;
    }

    public void AimTool(Vector2 aimDirection)
    {
        if (_toolRenderer == null || CurrentTool != ToolType.Gun || aimDirection.sqrMagnitude <= 0.001f)
            return;

        Vector2 dir = aimDirection.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float distance = Mathf.Max(_defaultToolLocalPosition.magnitude, 0.1f);

        _toolRenderer.transform.localPosition = dir * distance;
        _toolRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        _toolRenderer.flipX = false;
        _toolRenderer.flipY = dir.x < 0f;
    }

    private void UpdateToolSprite()
    {
        if (_toolRenderer == null) return;
        _toolRenderer.sprite = CurrentTool switch
        {
            ToolType.Hoe => _hoeSprite,
            ToolType.WateringCan => _wateringCanSprite,
            ToolType.Sword => _swordSprite,
            ToolType.Gun => _gunSprite,
            _ => null
        };

        if (CurrentTool != ToolType.Gun)
        {
            _toolRenderer.transform.localRotation = Quaternion.identity;
            _toolRenderer.flipY = false;
        }
    }

    public void CycleSeed()
    {
        if (_availableSeeds == null || _availableSeeds.Length == 0) return;
        _selectedSeedIndex = (_selectedSeedIndex + 1) % _availableSeeds.Length;
        GameEvents.RaiseSeedChanged(SelectedSeed);
    }

    public void UseTool()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mouseWorld.x, mouseWorld.y);

        if (Vector2.Distance(transform.position, mousePos) > _interactRange) return;

        Collider2D hit = Physics2D.OverlapCircle(mousePos, 0.3f, _farmTileLayer);

        if (hit != null && hit.TryGetComponent<IInteractable>(out var interactable))
        {
            if (interactable.CanInteract(CurrentTool))
            {
                if (CurrentTool == ToolType.WateringCan && hit.TryGetComponent<FarmTile>(out var tile))
                {
                    if (_seedCounts != null && _seedCounts[_selectedSeedIndex] <= 0) return;
                    tile.SetCropData(SelectedSeed);
                    interactable.Interact(this);
                    _seedCounts[_selectedSeedIndex]--;
                    GameEvents.RaiseSeedCountChanged(_selectedSeedIndex, _seedCounts[_selectedSeedIndex]);
                    return;
                }
                interactable.Interact(this);
                if (CurrentTool == ToolType.Hoe)
                    AudioManager.Instance?.PlaySFX("sfx_digging");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
