using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : DefaultEnemyAI
{
    int slimeAggroRange = 5;
    int slimeSpeed = 3;
    int slimeAttackCooldown = 3;
    int slimeAttackPower = 3;
    int slimeAttackRange = 2;
    float slimeAttackTimer = 0;
    int slimeHealth = 10;
    int slimeID = 1;
    Rigidbody2D rb;
    
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void TakeDamage(int power)
    {
        SpawnDamageNumber(power);
        slimeHealth -= power;
        if (slimeHealth <= 0) {
            GameController.Instance.EnemyKilled(slimeID);
            OnDeath();
            Destroy(gameObject);
        }
    }

    public override void OnDeath()
    {
        // Detach the target if it is present
        foreach (Transform child in transform)
        {
            child.SetParent(null, false);
        }
        transform.parent.GetComponent<EnemySpawner>().SpawnedMobDied();
    }

    public override int GetAggroRange()
    {
        return slimeAggroRange;
    }
    public override int GetAttackRange() {
        return slimeAttackRange;
    }

    public override void MoveToward(Vector3 pos) {
        pos.z = 0;
        Vector3 direction = pos - transform.position;
        Move(direction.normalized);
    }

    public void Move(Vector3 movement){
        rb.velocity = movement * slimeSpeed;
    }

    public void StopMoving() {
        rb.velocity = Vector2.zero;
    }

    public override void Attack()
    {
        StopMoving();
        if(slimeAttackTimer > 0) {
            return;
        }
        
        GameController.Instance.AttackPlayer(slimeAttackPower);

        slimeAttackTimer = slimeAttackCooldown;
        // Start the cooldown for the attack
        StartCoroutine(CooldownRoutine());
        IEnumerator CooldownRoutine() {
            while(slimeAttackTimer > 0) {
                slimeAttackTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
