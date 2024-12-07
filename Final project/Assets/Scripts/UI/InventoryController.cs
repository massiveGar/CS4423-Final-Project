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
        Debug.LogError("InventoryController: Ring could not be added (is inventory full?)");
    }
    // Inserts a ring to the given slot
    public void AddRingToSlot(Ring ring, int inventorySlot) {
        inventorySlots[inventorySlot].SetRing(ring);
    }

    public void ToggleInventory() {
        if(hidden) {
            ShowInventory();
        } else {
            HideInventory();
        }
        hidden = !hidden;
    }

    public void SaveInventory() {
        int i = 0;
        foreach(InventorySlot slot in inventorySlots) {
            Ring slottedRing = slot.GetSlottedRing();
            if(slottedRing != null) {
                NDSaveLoad.SaveInt(Constants.nd_ISlot + i, slottedRing.ringNumber);
            }
            i++;
        }
    }
    public void LoadInventory() {
        int i = 0;
        foreach(InventorySlot slot in inventorySlots) {
            int ringNumber = NDSaveLoad.LoadInt(Constants.nd_ISlot + i, -1);

            if(ringNumber != -1) {
                slot.SetRing(GameController.Instance.GetRing(ringNumber));
            }
            i++;
        }
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
