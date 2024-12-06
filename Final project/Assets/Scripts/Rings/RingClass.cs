using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RingClass {
    public int id;
    public string ringName;
    public int power;
    public int cooldownLength;
    public float cooldownTimer;
    public HotbarSlot hotbarSlot;
    public Image cooldownImage;
    public RingType ringType;
    public ResourceCost resource;

    public RingClass(int id, string ringName, int cooldownLength, int power) {
        this.id = id;
        this.ringName = ringName;
        this.cooldownLength = cooldownLength;
        this.power = power;
        hotbarSlot = null;
        cooldownTimer = 0;
    }

    public void SetSlot(HotbarSlot slot) {
        hotbarSlot = slot;
        cooldownImage = slot.GetCooldownImage();
    }

    public void ActivateAbility() {

        // TODO play animation

        

        // If ringType = attack, Attack the target
        GameController.Instance.Attack(power);
    }
}