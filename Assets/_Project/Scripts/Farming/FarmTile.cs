using UnityEngine;

public class FarmTile : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite _emptySprite;
    [SerializeField] private Sprite _tilledSprite;
    [SerializeField] private Sprite _wateredSprite;
    [SerializeField] private GameObject _cropPrefab;
    [SerializeField] private CropData _defaultCropData;

    public TileState State { get; private set; } = TileState.Empty;
    private Crop _currentCrop;
    private SpriteRenderer _spriteRenderer;
    private CropData _pendingCropData;

    public int GridX { get; private set; }
    public int GridY { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y)
    {
        GridX = x;
        GridY = y;
        SetState(TileState.Empty);
    }

    public bool CanInteract(ToolType tool)
    {
        return tool switch
        {
            ToolType.Hoe => State == TileState.Empty,
            ToolType.WateringCan => State == TileState.Tilled,
            ToolType.Sword => _currentCrop != null && _currentCrop.IsMature,
            _ => false
        };
    }

    public void Interact(PlayerToolHandler player)
    {
        switch (player.CurrentTool)
        {
            case ToolType.Hoe:
                SetState(TileState.Tilled);
                break;
            case ToolType.WateringCan:
                SetState(TileState.Watered);
                PlantCrop();
                break;
            case ToolType.Sword:
                var stats = player.GetComponent<PlayerStats>();
                _currentCrop?.Harvest(stats);
                SetState(TileState.Empty);
                break;
        }
    }

    private void SetState(TileState newState)
    {
        State = newState;
        _spriteRenderer.sprite = newState switch
        {
            TileState.Tilled => _tilledSprite != null ? _tilledSprite : _emptySprite,
            TileState.Watered => _wateredSprite != null ? _wateredSprite : _emptySprite,
            _ => _emptySprite
        };
        _spriteRenderer.color = Color.white;
    }

    private void PlantCrop()
    {
        if (_pendingCropData == null) _pendingCropData = _defaultCropData;
        if (_pendingCropData == null || _cropPrefab == null) return;
        var go = Instantiate(_cropPrefab, transform.position, Quaternion.identity);
        _currentCrop = go.GetComponent<Crop>();
        _currentCrop?.Init(_pendingCropData, () => {
            _currentCrop = null;
            SetState(TileState.Empty);
        });
    }

    public void SetCropData(CropData data) => _pendingCropData = data;
}
