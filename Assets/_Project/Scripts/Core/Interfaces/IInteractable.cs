public interface IInteractable
{
    void Interact(PlayerToolHandler player);
    bool CanInteract(ToolType tool);
}
