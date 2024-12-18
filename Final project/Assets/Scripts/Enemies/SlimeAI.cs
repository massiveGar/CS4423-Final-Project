using System.Collections;
using UnityEngine;

public class SlimeAI : DefaultEnemyAI
{
    // Set the properties for this enemy
    public override string EnemyName {get; set;} = "Slime";
    public override int MovementSpeed {get; set;} = 2;
    public override float AttackCooldown {get; set;} = 3;
    public override int BaseAttackPower {get; set;} = 3;
    public override int AttackRange {get; set;} = 3;
    public override int AggroRange {get; set;} = 10;
    public override int MaxHealth {get; set;} = 10;
    public override int EnemyID {get; set;} = 0;
    
    Coroutine aggroCoroutine = null;
    
    void Awake() {
        xpOrbPrefab = Resources.Load<GameObject>("Prefabs/XPOrb");
        damageNumberText = Resources.Load<GameObject>("Prefabs/Damage Number");
        
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Damage the enemy and aggro it
    public override void TakeDamage(int power) {
        SpawnDamageNumber(power);
        currentHealth -= power;
        if (currentHealth <= 0) {
            OnDeath();
        }

        // Reset the timer for following the player every time the enemy takes damage
        if (aggroCoroutine != null) {
            StopCoroutine(aggroCoroutine);
        }
        aggroCoroutine = StartCoroutine(Aggro());
    }

    // When the enemy is attacked, it should be able to follow the player for a while
    IEnumerator Aggro() {
        int oldRange = AggroRange;
        AggroRange = 500; 
        yield return new WaitForSeconds(3f);
        AggroRange = oldRange;
        aggroCoroutine = null;
    }

    // Attack player every 3 seconds
    public override void Attack()
    {
        if(onCooldown) {
            return;
        }
        onCooldown = true;
        animator.SetTrigger("TrAttack");

        // AttackPlayer called by animation
        
        // Start the cooldown for the attack
        StartCoroutine(CooldownRoutine());
        IEnumerator CooldownRoutine() {
            yield return new WaitForSeconds(AttackCooldown);
            onCooldown = false;
        }
    }

    // Called at frame 2, 0:20 of animation WolfAttack
    public void AttackPlayer() {
        GameController.Instance.AttackPlayer(BaseAttackPower);
    }

    public override void SetColor()
    {
        Color rankColor = Color.green;
        if(rank > 3 && rank <= 6) {
            rankColor = Color.yellow;
        } else if(rank > 6 && rank <= 9) {
            rankColor = Color.red;
        } else if(rank == 10){
            rankColor = Color.magenta;
        }
        spriteRenderer.color = rankColor;
    }
}
