using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperNPC : MonoBehaviour, IInteractable
{
    public void Interact(PlayerToolHandler player)
    {
        Debug.Log("Hello! Welcome to the shop.");
    }
    public bool CanInteract(ToolType tool)
    {
        return true;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
