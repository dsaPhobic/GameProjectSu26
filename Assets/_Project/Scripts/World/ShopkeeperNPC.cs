using System.Collections;
using UnityEngine;

public class ShopkeeperNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private PetEggData[] _eggsForSale;
    [SerializeField] private int _selectedEggIndex;
    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private KeyCode _buyKey = KeyCode.E;
    [SerializeField] private KeyCode _cycleEggKey = KeyCode.Q;

    private PlayerToolHandler _nearbyPlayer;

    private void Update()
    {
        _nearbyPlayer = FindNearbyPlayer();
        if (_nearbyPlayer == null) return;

        if (Input.GetKeyDown(_cycleEggKey))
            CycleEgg();

        if (Input.GetKeyDown(_buyKey))
            BuySelectedEgg(_nearbyPlayer);
    }

    public void Interact(PlayerToolHandler player)
    {
        BuySelectedEgg(player);
    }

    public bool CanInteract(ToolType tool)
    {
        return true;
    }

    private void BuySelectedEgg(PlayerToolHandler player)
    {
        PetEggData egg = SelectedEgg;
        if (egg == null)
        {
            Debug.Log("Shop has no pet eggs for sale.");
            return;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        if (!stats.SpendGold(egg.price))
        {
            Debug.Log($"Need {egg.price} gold to buy {egg.eggName}.");
            return;
        }

        PlayerPetInventory.AddEgg(egg);
        Debug.Log($"Bought {egg.eggName} for {egg.price} gold.");
    }

    private void CycleEgg()
    {
        if (_eggsForSale == null || _eggsForSale.Length == 0) return;

        _selectedEggIndex = (_selectedEggIndex + 1) % _eggsForSale.Length;
        Debug.Log($"Selected egg: {SelectedEgg.eggName} ({SelectedEgg.price} gold)");
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

    private PetEggData SelectedEgg =>
        _eggsForSale != null && _eggsForSale.Length > 0
            ? _eggsForSale[Mathf.Clamp(_selectedEggIndex, 0, _eggsForSale.Length - 1)]
            : null;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _nearbyPlayer != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
