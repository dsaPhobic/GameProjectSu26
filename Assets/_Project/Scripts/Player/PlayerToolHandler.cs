using UnityEngine;

public class PlayerToolHandler : MonoBehaviour
{
    public ToolType CurrentTool { get; private set; } = ToolType.Hoe;

    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private LayerMask _farmTileLayer;
    [SerializeField] private CropData[] _availableSeeds;

    private int _selectedSeedIndex = 0;
    private PlayerInput _input;

    public CropData SelectedSeed => (_availableSeeds != null && _availableSeeds.Length > 0)
        ? _availableSeeds[_selectedSeedIndex] : null;

    public string SelectedSeedName => SelectedSeed != null ? SelectedSeed.cropName : "None";

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    public void SwitchTool(int slot)
    {
        CurrentTool = slot switch
        {
            1 => ToolType.Hoe,
            2 => ToolType.WateringCan,
            3 => ToolType.Sword,
            _ => CurrentTool
        };
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
                    tile.SetCropData(SelectedSeed);
                interactable.Interact(this);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
