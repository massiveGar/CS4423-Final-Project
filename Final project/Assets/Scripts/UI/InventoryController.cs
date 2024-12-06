using UnityEngine;

public class InventoryController : MonoBehaviour
{
    InventorySlot[] inventorySlots;
    CanvasGroup inventoryUI;
    bool hidden = true;

    void Awake() {
        GameController.OnInventory += ToggleInventory;
        inventoryUI = GetComponent<CanvasGroup>();
        inventorySlots = GetComponentsInChildren<InventorySlot>();

        HideInventory();
    }

    void OnDisable() {
        GameController.OnInventory -= ToggleInventory;
    }

    // Inserts a Ring into the first empty inventory slot. If inventory is full, tough luck
    public void AddRingToInventory(Ring ring) {
        foreach(InventorySlot slot in inventorySlots) {
            if(slot.GetSlottedRing() == null) {
                slot.SetRing(ring);
                return;
            }
        }
    }

    public void ToggleInventory() {
        if(hidden) {
            ShowInventory();
        } else {
            HideInventory();
        }
        hidden = !hidden;
    }

    // Show/hide the inventory UI by changing its alpha and allow the user to click within it
    void ShowInventory() {
        inventoryUI.alpha = 1;
        inventoryUI.interactable = true;
        inventoryUI.blocksRaycasts = true;
    }
    void HideInventory() {
        inventoryUI.alpha = 0;
        inventoryUI.interactable = false;
        inventoryUI.blocksRaycasts = false;
    }
}
