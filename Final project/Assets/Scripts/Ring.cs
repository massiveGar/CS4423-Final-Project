using System;
using UnityEngine;

public class Ring {
        public int id;
        public string name;
        public int power;
        public int cooldownLength;
        public float cooldownTimer;
        public Transform hotbarSlot;

        public Ring(int id, string name, int cooldownLength, int power) {
            this.id = id;
            this.name = name;
            this.cooldownLength = cooldownLength;
            this.power = power;
            hotbarSlot = null;
            cooldownTimer = 0;
        }
    }