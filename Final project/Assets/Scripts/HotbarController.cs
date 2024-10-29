using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class HotbarController : MonoBehaviour
{
    [SerializeField] RingController ringController;
    Transform[] hotbarSlots = new Transform[5];
    void Start() {
        int i = 0;
        foreach(Transform slot in transform) {
            hotbarSlots[i] = slot;
            i++;
        }
    }

    void FixedUpdate() {
        int i = 0;
        // Change the hotbar slot image to be the corresponding ring, or a blank square if empty
        foreach(Transform slot in hotbarSlots) {
            Ring slotRing = ringController.GetRingInSlot(i);
            
            // Get the ring's name, which is "Empty" if no ring
            string ringName;
            if(slotRing == null) {
                ringName = "Empty";
            } else {
                slotRing.hotbarSlot = slot;
                ringName = slotRing.name;
            }

            Image slotImage = slot.GetComponent<Image>();
            slotImage.sprite = Resources.Load<Sprite>("Sprites/" + ringName);
            
            i++;
        }   
    }
}
