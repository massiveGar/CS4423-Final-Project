using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewRing", menuName = "Ring")]
public class Ring : ScriptableObject
{
    public int id = -1;
    public string ringName;
    public string ringDescription;
    public float basePower;
    public float attackRange;
    public int cooldownLength;
    public RingType ringType;
    public ResourceCost resource;

    [NonSerialized] public int rank = 1;
    [NonSerialized] public int level = 0;
    [NonSerialized] public int power;
    [NonSerialized] public float cooldownTimer;
    [NonSerialized] public HotbarSlot hotbarSlot;
    [NonSerialized] public Image cooldownImage;
    [NonSerialized] public int ringNumber;
    [NonSerialized] public int totalXP = 0;

    void OnEnable() {
        power = (int)math.round(basePower);
    }

    public void SetSlot(HotbarSlot slot) {
        hotbarSlot = slot;
        if(slot == null) {
            cooldownImage = null;
            return;
        }
        cooldownImage = slot.GetCooldownImage();
    }

    // Activate the ring's ability
    // If the player's resource is not enough, return false
    // If the ring is an attack ring and there was no target, return false
    // Otherwise, return true
    public bool ActivateAbility() {
        // If the player does not have enough resource, return false
        if(resource.type == ResourceType.HEALTH) {
            if(GameController.Instance.GetPlayerHealth() < resource.cost) {
                return false;
            }
        } else if(resource.type == ResourceType.MANA) { 
            if(GameController.Instance.GetPlayerMana() < resource.cost) {
                return false;
            }
        } else if(resource.type == ResourceType.STAMINA) {
            if(GameController.Instance.GetPlayerStamina() < resource.cost) {
                return false;
            }
        }

        if(ringType == RingType.ATTACK) {   // Attack the target
            if(!GameController.Instance.Attack(power)) {
               return false;    // If there was no target to attack, return false
            }
        }
        
        // TODO play animation
        
        // Use resource after using the ring
        if(resource.type == ResourceType.HEALTH) {
            GameController.Instance.AttackPlayer(resource.cost);
        } else if(resource.type == ResourceType.MANA) { 
            GameController.Instance.UseMana(resource.cost);
        } else if(resource.type == ResourceType.STAMINA) {
            GameController.Instance.UseStamina(resource.cost);
        }

        return true;
    }

    public void SetLevel(int level) {
        this.level = level;
        UpdatePower();
    }
    public void IncrementLevel() {
        level += 1;
        totalXP += rank;
        if(level == 10) {
            basePower *= 1.5f;
            rank++;
            level = 0;
        }
        UpdatePower();

        if(hotbarSlot != null) {
            GameController.Instance.UpdatePlayerLevel();
        }
    }

    void UpdatePower() {
        power = (int) Mathf.Ceil(basePower * (rank + level/10f));
    }

    public bool IsOnCooldown() {
        return cooldownTimer > 0;
    }

    // Return half of the total xp spent on this ring, rounding up from .5
    public int GetSalvageValue() {
        if(totalXP == 0) {
            return 1;
        }
        return (int)Mathf.Round((totalXP + 0.01f) / 2.0f);
    }

    public void Salvage() {
        GameController.Instance.DestroyRing(this);
        
        Destroy(this);
    }

    // Calculates the distance between the player and the current target and returns true if distance <= attackRange
    public bool TargetInRange() {
        Player player = GameController.Instance.GetPlayer();
        GameObject target = GameController.Instance.GetTarget();

        float distance = Vector3.Distance(player.transform.position, target.transform.position);

        return distance <= attackRange;
    }
}