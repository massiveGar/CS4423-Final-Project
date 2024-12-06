using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    RectTransform rectTransform;
    Transform currentSlot;

    MainUIController mainUI;
    Canvas mainUICanvas;
    SelectedRingController selectedRingController;
    CanvasGroup canvasGroup;
    RectTransform canvasRect;
    
    public Ring ring; 
    Image slotImage;
    Sprite emptySprite;

    HoverText hoverText;
    Coroutine popupCoroutine;
    
    
    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        currentSlot = transform.parent;

        slotImage = GetComponent<Image>();
        emptySprite = slotImage.sprite;
    }

    void Start() {
        mainUI = GameController.Instance.mainUI;
        mainUICanvas = mainUI.GetMainCanvas();
        hoverText = mainUI.GetHoverText();
        selectedRingController = mainUI.GetSelectedRingController();
        canvasRect = mainUICanvas.GetComponent<RectTransform>();
    }

    // Make the item appear transparent while dragging or when clicking
    public void OnPointerDown(PointerEventData cursorData) {
        selectedRingController.SelectItem(this);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    // Restore the original look and position
    public void OnPointerUp(PointerEventData cursorData) {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Move item back to the currentSlot's localPosition (0,0)
        transform.SetParent(currentSlot);
        rectTransform.localPosition = Vector3.zero;
    }

    // Prevent dragging if there is no ring assigned
    public void OnBeginDrag(PointerEventData cursorData) {
        if (ring == null || ring.cooldownTimer > 0) {
            cursorData.pointerDrag = null;
            return;
        }
    }
    // Move the UI element with the mouse cursor
    public void OnDrag(PointerEventData cursorData) {
        transform.SetParent(mainUICanvas.transform);    // Change parent so it always appears on top
        
        transform.localPosition = GetMousePosition(cursorData);
    }

    // Display the ring's name in a popup text when the mouse is hovering the item
    public void OnPointerEnter(PointerEventData cursorData) {
        if(ring == null) {
            return;
        }

        popupCoroutine = StartCoroutine(WaitBeforePopupText(cursorData));
    }
    public void OnPointerExit(PointerEventData cursorData) {
        if(ring == null) {
            return;
        }
        
        StopCoroutine(popupCoroutine);
        hoverText.HideText();
    }
    IEnumerator WaitBeforePopupText(PointerEventData cursorData) {
        yield return new WaitForSeconds(1);
        hoverText.transform.localPosition = GetMousePosition(cursorData);
        hoverText.ShowText(ring.ringName);
    }

    // Swap slots with this item and item2
    public void Swap(InventoryItem item2) {
        Transform newSlot = item2.GetSlotTransform();

        item2.SetSlotTransform(currentSlot);
        SetSlotTransform(newSlot);        
    }

    public void SetSlotTransform(Transform newSlot) {
        currentSlot = newSlot;
        transform.SetParent(newSlot);
    }
    public Transform GetSlotTransform() {
        return currentSlot;
    }

    // Get the mouse position in relation to the canvas
    Vector2 GetMousePosition(PointerEventData cursorData) {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, cursorData.position, mainUICanvas.worldCamera, out position);
        return position;
    }

    public void SetRing(Ring ring) {
        this.ring = ring;
        
        slotImage.sprite = Resources.Load<Sprite>("Sprites/" + ring.ringName);

        gameObject.name = ring.name;
    }
    public Ring GetRing() {
        return ring;
    }
    public int GetRingNumber() {
        if(ring == null) {
            return -1;
        }
        return ring.ringNumber;
    }
    public bool RingOnCooldown() {
        if(ring == null) {
            return false;
        }
        return ring.IsOnCooldown();
    }

    public bool HasRing() {
        return ring != null;
    }

    public Image GetRingImage() {
        return slotImage;
    }

    public void Clear() {
        ring = null;
        slotImage.sprite = emptySprite;
    }
}