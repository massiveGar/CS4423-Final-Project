using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RingController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    List<Ring> rings = new List<Ring>();
    AudioSource audioSource;

    // Build RingMap
    public void Start() {
        // Rings array will store hotbar rings from indices 0-4, and inventory rings from 5+
        rings.Add(new Ring(0, "Slash", 2, 3));  // Starter ring
        audioSource = GetComponent<AudioSource>();
    }

    public void AddRing(Ring ring) {
        if(ring == null) {
            return;
        }
        rings.Add(ring);
    }

    public Ring GetRingInSlot(int slot) {
        if(slot > rings.Count-1) {
            return null;
        }
        return rings[slot];
    }

    public void UseRing(int hotbarIndex) {
        // Set the ring position
        if(!gameController.Aim()) { // If there is no target
            return;
        }

        // Grab the ring from the array, which corresponds to the hotbar, and activate its ability
        // TODO add case for defensive rings
        if(hotbarIndex > rings.Count) {   // No ring in slot
            return;
        }
        int selectedRingID = rings[hotbarIndex-1].id;
        ActivateRingAbility(selectedRingID);
    }

    void ActivateRingAbility(int ringID) {
        Ring ring = rings[ringID];
        // Check for cooldown
        if(ring.cooldownTimer > 0) {
            return;
        }
        // TODO play animation

        // Play sound
        AudioClip clip = Resources.Load<AudioClip>("Audio/"+ring.name);
        audioSource.clip = clip;
        audioSource.Play();

        // Attack the target
        gameController.Attack(ring.power);

        // Set the cooldown timer and get the cooldown overlay
        ring.cooldownTimer = ring.cooldownLength;
        Image cooldownImage = ring.hotbarSlot.transform.GetChild(0).GetComponent<Image>();

        // Start the cooldown and update the hotbar's cooldown display
        StartCoroutine(CooldownRoutine());
        IEnumerator CooldownRoutine() {
            while(ring.cooldownTimer > 0) {
                ring.cooldownTimer -= Time.deltaTime;
                cooldownImage.fillAmount = ring.cooldownTimer/ring.cooldownLength;
                yield return null;
            }
            cooldownImage.fillAmount = 0;
        }
    }
}
