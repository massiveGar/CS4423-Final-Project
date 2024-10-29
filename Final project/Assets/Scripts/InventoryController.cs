using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] RectTransform hotbarBounds;
    CanvasGroup inventoryUI;
    bool hidden = true;

    void OnEnable() {
        PlayerInputHandler.OnInventoryPressed += ToggleInventory;
    }

    void OnDisable() {
        PlayerInputHandler.OnInventoryPressed -= ToggleInventory;
    }

    void Awake() {
        inventoryUI = GetComponent<CanvasGroup>();
    }

    public void ToggleInventory() {
        if(hidden) {
            ShowInventory();
        } else {
            HideInventory();
        }
        hidden = !hidden;
    }

    void ShowInventory() {
        inventoryUI.alpha = 1;             // Make panel visible
        inventoryUI.interactable = true;   // Allow interactions
        inventoryUI.blocksRaycasts = true; // Allow panel to block clicks
    }

    void HideInventory() {
        inventoryUI.alpha = 0;             // Make panel invisible
        inventoryUI.interactable = false;  // Disable interactions
        inventoryUI.blocksRaycasts = false; // Allow clicks to pass through
    }

    public RectTransform getHotbarBounds() {
        return hotbarBounds;
    }
}
