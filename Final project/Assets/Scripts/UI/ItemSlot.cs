using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ItemSlot : MonoBehaviour, IDropHandler {
    public InventoryItem slottedItem;
    public Ring slottedRing;

    void Awake() {
        slottedItem = GetComponentInChildren<InventoryItem>();
    }

    public abstract void OnDrop(PointerEventData eventData);
    public abstract void SetRing(Ring ring);
    
    public void SetSlottedItem(InventoryItem newItem) {
        slottedItem = newItem;
        slottedRing = slottedItem.GetRing();

        slottedItem.transform.SetParent(transform);
        slottedItem.transform.localPosition = Vector3.zero;
    }
    public InventoryItem GetSlottedItem() {
        return slottedItem;
    }
    public Ring GetSlottedRing() {
        return slottedRing;
    }

    

    // Swap the slotted items and their slot references from this slot and slot2
    public void Swap(ItemSlot slot2) {
        InventoryItem slot2Item = slot2.GetSlottedItem();

        slottedItem.SetSlotTransform(slot2.transform);
        slot2.SetSlottedItem(slottedItem);

        slot2Item.SetSlotTransform(transform);
        SetSlottedItem(slot2Item);
    }
}