using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AnimationState {IDLE, WALK, ATTACK}

public abstract class DefaultEnemyAI : MonoBehaviour
{
    public abstract void TakeDamage(int power);
    public abstract void Attack();
    public abstract void SetColor();

    // Enemy properties
    public abstract string EnemyName {get; set;}
    public abstract int MovementSpeed {get; set;}
    public abstract float AttackCooldown {get; set;}
    public abstract int BaseAttackPower {get; set;} // Attack power at CL 1.0
    public abstract int AttackRange {get; set;}
    [SerializeField] public abstract int AggroRange {get; set;}
    public abstract int MaxHealth {get; set;}   // Max health at CL 1.0
    public abstract int EnemyID {get; set;}

    // Methods to share with the derived AI
    [NonSerialized] public GameObject damageNumberText;
    [NonSerialized] public GameObject xpOrbPrefab;
    [NonSerialized] public Rigidbody2D rigidBody;
    [NonSerialized] public int currentHealth;
    [NonSerialized] public Animator animator;
    [NonSerialized] public bool onCooldown = false;
    [NonSerialized] public SpriteRenderer spriteRenderer;

    [SerializeField] int patrolWidth = 1;
    [SerializeField] int patrolHeight = 1;
    public int rank = 1;
    public int level = 0;

    Coroutine stateMachine = null;

    // Get references
    void Start() {
        player = GameController.Instance.GetPlayer().gameObject;
        patrolArea = transform.position;

        // Scale health with CL
        MaxHealth = Mathf.RoundToInt(MaxHealth * Mathf.Pow(1.3f, rank-1 + level/10));
        currentHealth = MaxHealth;

        // Scale power with CL
        BaseAttackPower = Mathf.RoundToInt(BaseAttackPower * Mathf.Pow(1.2f, rank-1 + level/10));

        ChangeState(IdleState); // Start the state machine
    }

    // Track if this enemy is on screen or not
    void OnBecameVisible() {
        stateMachine = StartCoroutine(StateMachineCoroutine());
        GameController.Instance.EnemyEnteredScreen(gameObject);
    }
    void OnBecameInvisible() {
        StopCoroutine(stateMachine);
        StopMoving();
        GameController.Instance.EnemyLeftScreen(gameObject);
    }

    // Used by the spawner to tell how far an enemy can roam
    public void SetPatrolArea(int width, int height) {
        patrolWidth = width;
        patrolHeight = height;
    }

    // Change the rank.level and change color according to rank
    public void SetCL(int rank, int level) {
        this.rank = rank;
        this.level = level;
        SetColor();
    }

    // Spawn a damage number near the enemy and fling it up and left/right
    public void SpawnDamageNumber(int number) {
        Vector2 spawnPos = new Vector3(transform.position.x + Random.Range(-0.5f,0.5f), transform.position.y + Random.Range(-0.5f,0.5f), 0);
        Vector2 force = new Vector2(5 * Random.Range(-0.5f,0.5f), 3 * Random.Range(0,0.5f));

        GameObject text = Instantiate(damageNumberText, spawnPos, Quaternion.identity); // Spawn the number
        
        text.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);  // Fling
        text.GetComponent<TextMeshPro>().SetText(""+number);
        Destroy(text, 1);
    }

    // Move in the direction of pos
    public void MoveToward(Vector3 pos) {
        pos.z = 0;
        Vector3 direction = pos - transform.position;
        Move(direction.normalized);
    }
    // Move in direction at a speed of MovementSpeed
    public void Move(Vector3 direction){
        rigidBody.velocity = direction * MovementSpeed;
        float currentX = transform.localScale.x;

        if (direction.x > 0 && currentX < 0) {
            transform.localScale = new Vector3(-currentX, transform.localScale.y, 1); // Face right
        } else if (direction.x < 0 && currentX > 0) {
            transform.localScale = new Vector3(-currentX, transform.localScale.y, 1); // Face left
        }
    }
    public void StopMoving() {
        rigidBody.velocity = Vector2.zero;
    }

    // Detach the target, tell the spawner, tally the kill, roll an xp drop, and destroy itself
    public void OnDeath()
    {
        // Detach the target if it is present
        foreach (Transform child in transform) {
            child.SetParent(null, false);
        }

        if(transform.parent != null) {
            EnemySpawner spawner = transform.parent.GetComponent<EnemySpawner>();
            spawner.SpawnedMobDied();
        }
        
        GameController.Instance.EnemyKilled(EnemyID);
        
        // Roll a random number that determines how much xp this enemy drops, influenced by rank
        float roll = Random.value;
        float skewStrength = Mathf.Lerp(2.5f, 0.75f, rank / 10f);    // Skew the curve by the enemy's rank
        float weightedRoll = Mathf.Pow(roll, skewStrength);          // Skew the random roll

        Debug.Log("Enemy: " + roll + " -> " + weightedRoll);
        int result = Mathf.FloorToInt(Mathf.Lerp(1, 10, weightedRoll));
        // for (int i = 0; i < result; i++) Instantiate(xpOrbPrefab);
        GameController.Instance.AddXP(result);
        Debug.Log("Enemy: Adding xp = " + result);

        Destroy(gameObject);
    }

    // Variables for the state machine of an enemy
    delegate void AIState();
    AIState currentState;
    AnimationState currentAnimation = AnimationState.IDLE;
    float stateTime = 0;

    GameObject player;

    Vector3 lastTargetPos;
    Vector3 patrolPos;
    Vector3 patrolArea;

    bool attacking = false;
    bool waiting = false;
    bool waitingDone = false;
    
    // Calls AITick every frame
    IEnumerator StateMachineCoroutine() {
        while(true) {
            AITick(); 
            yield return null;
        }
    }
    
    // Called every frame while the StateMachineCoroutine is active
    // Calls the currentState and tracks the stateTime
    void AITick() {
        currentState();
        stateTime += Time.deltaTime;
    }
    
    // Change the currentState that is called every frame
    void ChangeState(AIState newAIState) {
        currentState = newAIState;
        stateTime = 0;
    }

    // Return true if the player is within AggroRange
    bool CanSeeTarget() {
        return Vector3.Distance(transform.position, player.transform.position) <= AggroRange;
    }
    // Return true if the player is within AttackRange
    bool TargetInRange() {
        return Vector3.Distance(transform.position, player.transform.position) <= AttackRange;
    }

    IEnumerator WaitForGivenSeconds(float waitTime) {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        waitingDone = true;
    }

    // Each state has its own animation cycle

    // Do nothing, if player is within AttackRange, switch to FollowState
    // Otherwise, pick a patrol pos and switch to PatrolState
    void IdleState() {
        if(currentAnimation != AnimationState.IDLE) {
            currentAnimation = AnimationState.IDLE;
            animator.SetTrigger("TrIdle");
        }
        
        //do nothing
        if(CanSeeTarget()) {
            ChangeState(FollowState);
            return;
        }

        if(!waiting) {
            StartCoroutine(WaitForGivenSeconds(Random.Range(0.3f, 5)));
        }

        if(waiting && waitingDone) {
            PickNewPatrolPos();
            ChangeState(PatrolState);

            waiting = false;
            waitingDone = false;
            return;
        }
    }

    // When player is within AggroRange, or player has hit the enemy, move toward player
    // When the player is in attack range, change to AttackState
    void FollowState() {
        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
        
        if(distanceToTarget <= AttackRange) {
            ChangeState(AttackState);
        }

        if(distanceToTarget >= AggroRange) {
            lastTargetPos = player.transform.position;
            ChangeState(GoToLastTargetPosState);
            return;
        } else {
            MoveToward(player.transform.position);
        }
    }

    // When within AttackRange of the player, begin attacking
    // If out of AttackRange, change to FollowState
    void AttackState() {
        if(attacking) {
            return;
        }
        attacking = true;
        StopMoving();

        currentAnimation = AnimationState.ATTACK;

        StartCoroutine(WaitBeforeAttacking());
        IEnumerator WaitBeforeAttacking() {
            // Wait so the player isn't immediately attacked
            yield return new WaitForSeconds(0.5f);
            while(attacking) {
                StopMoving();
                if(TargetInRange()) {
                    Attack();
                    yield return null;
                } else {
                    ChangeState(FollowState);
                    attacking = false;
                }
                
            }
        }
    }

    // When not aggro'd, wander around its spawn position
    // If the player is seen, change to FollowState
    void PatrolState() {
        // Find the distance from patrol target.
        // If it's greater than 0.01, move towards it, otherwise, idle and wait
        // If it's taking too long to reach target (stuck?), stop and choose a new location
        if(CanSeeTarget()) {
            ChangeState(FollowState);
            return;
        }
        float distanceToTarget = Vector3.Distance(transform.position, patrolPos);

        if(distanceToTarget >= 0.1f && currentAnimation != AnimationState.WALK) {
            currentAnimation = AnimationState.WALK;
            animator.SetTrigger("TrMove");

        }

        if(distanceToTarget < 0.1f && currentAnimation != AnimationState.IDLE) {
            currentAnimation = AnimationState.IDLE;
            animator.SetTrigger("TrIdle");
        }

        if(stateTime > 4 || distanceToTarget < 0.1f) {
            StopMoving();
            if(!waiting) {
                StartCoroutine(WaitForGivenSeconds(Random.Range(0.3f, 2)));
            }

            if(waiting && waitingDone) {
                stateTime = 0f;
                PickNewPatrolPos();
                waiting = false;
                waitingDone = false;
            }
        } else {
            MoveToward(patrolPos);
        }
    }

    // When losing sight of the player, go to where player was last seen, wait a bit before returning to PatrolState
    // If the player is seen again during this state, change to FollowState
    void GoToLastTargetPosState() {
        if(currentAnimation != AnimationState.WALK) {
            currentAnimation = AnimationState.WALK;
            animator.SetTrigger("TrMove");
        }

        float distanceToTarget = Vector3.Distance(transform.position, lastTargetPos);

        if(CanSeeTarget()) {
            ChangeState(FollowState);
            return;
        }

        if(distanceToTarget < 0.1f) {
            StopMoving();
            if(!waiting) {
                StartCoroutine(WaitForGivenSeconds(Random.Range(0.3f, 2)));
            }
            
            if(waiting && waitingDone) {
                ChangeState(IdleState);
                waiting = false;
                waitingDone = false;
                return;
            }
        } else {
            MoveToward(lastTargetPos);
        }
    }

    // Find a random position that is within the patrol bounds
    void PickNewPatrolPos() {
        patrolPos = patrolArea + new Vector3(Random.Range(-patrolWidth, patrolWidth), Random.Range(-patrolHeight, patrolHeight));
    }
}
