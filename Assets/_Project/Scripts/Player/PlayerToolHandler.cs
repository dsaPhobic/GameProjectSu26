using UnityEngine;

public class PlayerToolHandler : MonoBehaviour
{
    public ToolType CurrentTool { get; private set; } = ToolType.Hoe;

    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private LayerMask _farmTileLayer;

    private PlayerInput _input;

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

    public void UseTool()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position + (Vector3)_input.AimDirection * _interactRange,
            0.3f, _farmTileLayer);

        if (hit != null && hit.TryGetComponent<IInteractable>(out var interactable))
        {
            if (interactable.CanInteract(CurrentTool))
                interactable.Interact(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
