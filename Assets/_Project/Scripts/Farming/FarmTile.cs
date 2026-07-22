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

    public Crop CurrentCrop => _currentCrop;
    public bool HasLivingCrop => _currentCrop != null && !_currentCrop.IsDead;

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
                PlantCrop();
                break;
            case ToolType.Sword:
                var stats = player.GetComponent<PlayerStats>();
                _currentCrop?.Harvest(stats);
                _currentCrop = null;
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
            TileState.Planted => _wateredSprite != null ? _wateredSprite : _emptySprite,
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
        _currentCrop?.Init(_pendingCropData, HandleCropDestroyedByDamage);
        SetState(_currentCrop != null ? TileState.Planted : TileState.Watered);
    }

    public void SetCropData(CropData data) => _pendingCropData = data;

    public TileSaveData GetSaveData()
    {
        return new TileSaveData
        {
            x = GridX,
            y = GridY,
            state = State,
            cropType = _currentCrop != null ? _currentCrop.CropType : CropType.Wheat,
            cropStage = _currentCrop != null ? _currentCrop.StageIndex : 0,
            cropHP = _currentCrop != null ? _currentCrop.CurrentHP : 0
        };
    }

    public void LoadSaveData(TileSaveData data, CropData cropData)
    {
        if (_currentCrop != null)
            Destroy(_currentCrop.gameObject);

        _currentCrop = null;

        if (data.state != TileState.Planted)
        {
            SetState(data.state);
            return;
        }

        if (cropData == null || _cropPrefab == null)
        {
            SetState(TileState.Watered);
            return;
        }

        var go = Instantiate(_cropPrefab, transform.position, Quaternion.identity);
        _currentCrop = go.GetComponent<Crop>();
        _currentCrop?.Restore(cropData, (CropStage)data.cropStage, data.cropHP, HandleCropDestroyedByDamage);
        SetState(_currentCrop != null ? TileState.Planted : TileState.Watered);
    }

    private void HandleCropDestroyedByDamage()
    {
        _currentCrop = null;
        SetState(TileState.Empty);
        ServiceLocator.Get<FarmManager>()?.NotifyCropDestroyedByDamage();
    }
}
