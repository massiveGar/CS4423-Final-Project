using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : ItemSlot {
    void Awake() {
        slottedItem = GetComponentInChildren<InventoryItem>();
    }

    public override void OnDrop(PointerEventData cursor) {
        GameObject pendingObject = cursor.pointerDrag;
        if(pendingObject != null) {
            InventoryItem pendingItem = pendingObject.GetComponent<InventoryItem>();

            if(pendingItem == slottedItem) {
                return;
            }

            Swap(pendingItem.GetSlotTransform().GetComponent<ItemSlot>());            
        }
    }

    public override void SetRing(Ring ring) {
        slottedRing = ring;
        slottedItem.SetRing(ring);
        slottedItem.SetSlotTransform(transform);
    }
}
