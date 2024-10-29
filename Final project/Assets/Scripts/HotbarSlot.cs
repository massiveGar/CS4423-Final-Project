using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
    [SerializeField] int hotbarID = -1;
    GameObject currentItem, pendingItem;
    [SerializeField] Image cooldownImage;


    /*
     * When an item is dragged in to the slot, store it in a variable
     * If the mouse is released while the variable is not null, that means the item was dropped in the slot
     * Tell the ring controller that this ring is now in slot x, and if there was a ring already here,
     * swap indices with that ring
     * EXPERIMENTAL CODE, INVENTORY NOT IMPLEMENTED YET
     */
    public void OnDrop(PointerEventData cursor) {
        if(pendingItem != null) {
            Debug.Log("Item received");
            pendingItem.transform.SetParent(transform);
            // Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            // Debug.Log("Position: " + screenPosition);
            pendingItem.GetComponent<InventoryItem>().SetHome(Vector3.zero);
            if(currentItem != null) {
                
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        // Track the ring being dragged onto the slot
        pendingItem = eventData.pointerDrag;
        if(pendingItem == currentItem) {
            return;
        }

        if(pendingItem != null) {
            Debug.Log("Item entered " + gameObject.name);
            // Do something when the ring enters the slot
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(pendingItem == eventData.pointerDrag) {
            pendingItem = null;
        }
    }
}