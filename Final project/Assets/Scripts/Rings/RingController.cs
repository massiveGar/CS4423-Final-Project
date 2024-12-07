using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

[System.Serializable]
public enum RingType {ATTACK, DEFENSE};

[System.Serializable]
public enum ResourceType {HEALTH, MANA};

[System.Serializable]
public struct ResourceCost {
    public ResourceType type;
    public int cost;
};

public class RingController : MonoBehaviour
{
    InventoryController inventoryController;
    Ring[] ringDictionary;  // Ring dictionary will store all ring assets
    public List<Ring> rings = new List<Ring>();    // Rings array will store all inventory rings
    public Ring[] hotbarRings = new Ring[5];
    AudioSource audioSource;
    int ringCount = 0;
    
    // Build ringDictionary and add starter ring
    void Awake() {
        ringDictionary = Resources.LoadAll<Ring>("Rings");
        Array.Sort(ringDictionary, (a, b) => a.id.CompareTo(b.id));

        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        inventoryController = GameController.Instance.GetInventoryController();
    }

    public Ring AddNewRing(int ringID, int rank = 1, int level = 0) {
        if(ringID == -1) {
            return null;
        }
        Ring newRing = Instantiate(ringDictionary[ringID]); // Create new ring of ringID
        newRing.SetCL(rank, level);

        newRing.ringNumber = ringCount; // Track which ring this is 
        ringCount++;

        rings.Add(newRing); // Add to the list of owned rings
        return newRing; // Return so we can add it to the inventory if needed
    }

    // Equip a ring to targetIndex in hotbarRings, using the ringNumber in the rings array
    public void EquipRing(int targetIndex, int ringNumber) {
        Ring ring1;

        if(ringNumber == -1) {  // If equipping an empty item, ring1 is null
            ring1 = null;
        } else {
            ring1 = rings[ringNumber];  // Otherwise, get ring1 from the inventory array
        }
        Ring ring2 = hotbarRings[targetIndex]; // ring2 is the ring in the slot we're trying to equip to

        int ring1Index = Array.IndexOf(hotbarRings, ring1); // Find the hotbar index of ring1, if it is already equipped
        Debug.Log("RingController: Ring1Index: " + ring1Index);
        if(ring1Index > -1) {
            hotbarRings[ring1Index] = ring2;    // If ring1 is equipped, swap it with ring2
            if(ring2 != null) { // If ring2 was not empty, tell it which slot it's in now
                ring2.SetSlot(GameController.Instance.GetHotbarSlot(ring1Index));
            }
        } else if(ring2 != null) {
            ring2.SetSlot(null);    // Tell ring2 that it's not in a HotbarSlot, since ring1 wasn't
        }

        // Put ring1 in the hotbarRings array and tell ring1 which HotbarSlot it's in
        hotbarRings[targetIndex] = ring1;
        ring1.SetSlot(GameController.Instance.GetHotbarSlot(targetIndex));
        GameController.Instance.UpdatePlayerLevel();
    }

    public Ring GetRingInSlot(int slot) {
        if(slot > rings.Count-1) {
            return null;
        }
        return hotbarRings[slot];
    }
    public Ring GetRing(int ringNumber) {
        if(ringNumber >= rings.Count) {
            Debug.LogError("RingController: Tried to get ring out of range: " + ringNumber);
            return null;
        }
        return rings[ringNumber];
    }

    // Aim and get the Ring from the hotbar, activate the ring's ability
    public void UseRing(int hotbarIndex) {
        if(!GameController.Instance.Aim()) {    // Aim the ring and check its return value
            return; // If there is no target
        }

        // Grab the ring from the array, which corresponds to the hotbar, and activate its ability
        Ring selectedRing = hotbarRings[hotbarIndex];
        ActivateRingAbility(selectedRing);
    }

    void ActivateRingAbility(Ring ring) {
        if(ring == null || ring.cooldownTimer > 0 || !ring.TargetInRange()) {    // No ring in slot or ring is on cooldown
            if(ring == null){
                Debug.Log("RingController: Ring is null");
                return;
            }
            if(ring.cooldownTimer > 0) {
                Debug.Log("RingController: Ring on cooldown");
            }
            if(!ring.TargetInRange()) {
                Debug.Log("RingController: Ring out of range");
            }
            
            return;
        }
        
        
        // Activate the ring's ability, if it returns false, don't start the cooldown or play the sound
        if(!ring.ActivateAbility()) {
            return;
        }

        // Play sound
        AudioClip clip = Resources.Load<AudioClip>("Audio/"+ring.ringName);
        if(clip != null) {
            audioSource.PlayOneShot(clip);
        }

        // Start the cooldown and update the hotbar's cooldown display
        ring.cooldownTimer = ring.cooldownLength;
        StartCoroutine(CooldownRoutine());
        IEnumerator CooldownRoutine() {
            while(ring.cooldownTimer > 0) {
                ring.cooldownTimer -= Time.deltaTime;
                ring.cooldownImage.fillAmount = ring.cooldownTimer/ring.cooldownLength;
                yield return null;
            }
            ring.cooldownImage.fillAmount = 0;
        }
    }

    public int GetRingCount() {
        return rings.Count;
    }

    public void RemoveRing(int ringNumber) {
        rings.RemoveAt(ringNumber);

        int i = 0;
        foreach(Ring ring in rings) {
            ring.ringNumber = i;
            i++;
        }
    }

    // Saves rings as strings, starting from Ring0
    // Format: id,rank,level
    public void SaveRings() {
        int i = 0;
        foreach(Ring ring in rings) {
            NDSaveLoad.SaveDataDict(Constants.nd_Ring + i, ring.RingToString());
            i++;
        }
    }
    public void LoadRings() {
        Debug.Log("RingController: Loading rings");
        int i = 0;
        string ringData = NDSaveLoad.GetData(Constants.nd_Ring + i, null);
        Debug.Log("RingController: got ring: " + ringData);
        while(ringData != null) {
            string[] ringParts = ringData.Split(',');
            AddNewRing(int.Parse(ringParts[0]), int.Parse(ringParts[1]), int.Parse(ringParts[2]));
            
            i++;
            ringData = NDSaveLoad.GetData(Constants.nd_Ring + i, null);
        }
        Debug.Log("RingController: Loading done");
    }
}
