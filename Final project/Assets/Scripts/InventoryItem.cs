using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    [SerializeField] InventoryController inventoryController;
    RectTransform hotbarBounds;
    RectTransform rectTransform;
    [SerializeField] Canvas canvas;
    CanvasGroup canvasGroup;
    Vector3 homePosition;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        //canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        hotbarBounds = inventoryController.getHotbarBounds();
        homePosition = rectTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Optional: Make the UI element appear more prominent while dragging
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Allows UI underneath to be clickable
    }

    public void OnDrag(PointerEventData cursor) {
        // Move the UI element with the mouse cursor
        Vector2 position;
        //RectTransform parentTransform = GetComponentInParent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent,
            cursor.position, 
            transform.GetComponentInParent<Canvas>().worldCamera, 
            out position);
        
        rectTransform.localPosition = position;
    }

    public void OnPointerUp(PointerEventData cursor) {
        // Restore the original settings
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Is the item dropped in the hotbar?
        // if (RectTransformUtility.RectangleContainsScreenPoint(hotbarBounds, cursor.position, Camera.main)) {
        //     Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, hotbarBounds.position);
        //     float deltaWidth = hotbarBounds.sizeDelta.x;
        //     float rectWidth = hotbarBounds.rect.size.x;
        //     float slotWidth = deltaWidth/5;
        //     //Debug.Log("Bounds position: " + screenPosition.x);
        //     Debug.Log("Cursor position: " + cursor.position);
        //     //Debug.Log("Cursor distance from bounds: " + (cursor.position.x - screenPosition.x));
        //     Debug.Log("SizeDelta Width: " + deltaWidth);
        //     Debug.Log("Rect width: " + rectWidth);
        //     //Debug.Log("Final item slot: " + (int) (cursor.position.x % slotWidth));
        // }


        // If final position is not valid
        rectTransform.localPosition = homePosition;
    }

    public void SetHome(Vector3 position) {
        homePosition = position;
        rectTransform.localPosition = homePosition;
    }
}