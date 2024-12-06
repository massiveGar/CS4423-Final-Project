using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlot : ItemSlot {
    [SerializeField] int hotbarID = -1;
    Image cooldownImage;

    void Awake() {
        cooldownImage = transform.GetChild(0).GetComponent<Image>();
        cooldownImage.GetComponent<Canvas>().sortingLayerName = "UI";

        slottedItem = GetComponentInChildren<InventoryItem>();
    }

    public Image GetCooldownImage() {
        return cooldownImage;
    }
    public void SetImage(Sprite newImage) {
        cooldownImage.sprite = newImage;
    }

    /*
     * If the mouse is released on this object while carrying an item, that means the item was dropped in the slot
     * Tell ringController, through the GameController, to equip the ringNumber in slot hotbarID
     * This will not trigger if either item is on cooldown
     */
    public override void OnDrop(PointerEventData cursor) {
        if(slottedItem.RingOnCooldown()) {
            return;
        }

        GameObject pendingObject = cursor.pointerDrag;
        if(pendingObject == null) {
            return;
        }

        InventoryItem pendingItem = pendingObject.GetComponent<InventoryItem>();

        if(pendingItem.RingOnCooldown()) {
            Debug.Log("Hotbar slot: pending item is on cooldown");
            return;
        }

        Swap(pendingItem.GetSlotTransform().GetComponent<ItemSlot>());
        
        // Tell the ringController that the corresponding ring is now equipped in this slot
        GameController.Instance.EquipRing(hotbarID, slottedItem.GetRingNumber());  
    }
}