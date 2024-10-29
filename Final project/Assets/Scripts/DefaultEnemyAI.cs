using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DefaultEnemyAI : MonoBehaviour
{
    [SerializeField] GameObject damageNumberText;
    void OnBecameVisible() {
        // Called once when the object enters the camera's view
        GameController.Instance.EnemyEnteredScreen(gameObject);
    }

    void OnBecameInvisible() {
        // Called once when the object leaves the camera's view
        GameController.Instance.EnemyLeftScreen(gameObject);
    }

    public void SpawnDamageNumber(int number) {
        Vector2 spawnPos = new Vector3(transform.position.x + Random.Range(-0.5f,0.5f), transform.position.y + Random.Range(-0.5f,0.5f), 0);
        Vector2 force = new Vector2(5 * Random.Range(-0.5f,0.5f), 3 * Random.Range(0,0.5f));

        GameObject text = Instantiate(damageNumberText, spawnPos, Quaternion.identity);
        
        text.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        text.GetComponent<TextMeshPro>().SetText(""+number);

        StartCoroutine(DespawnDamageNumber());
        IEnumerator DespawnDamageNumber() {
            yield return new WaitForSeconds(1);
            Destroy(text);
        }
    }

    public abstract void TakeDamage(int power);

    public abstract void OnDeath();

    public abstract int GetAggroRange();

    public abstract void Attack();

    public abstract void MoveToward(Vector3 position);

    public abstract int GetAttackRange();

    // Default state machine for an enemy
    delegate void AIState();
    AIState currentState;
    Vector3 lastTargetPos;
    float stateTime = 0;
    GameObject player;
    int aggroRange;
    int attackRange;
    Vector3 patrolPos;
    Vector3 patrolArea;
    void Start() {
        ChangeState(IdleState);
        player = GameController.Instance.GetPlayer().gameObject;
        aggroRange = GetAggroRange();
        attackRange = GetAttackRange();
        patrolArea = transform.position;
    }

    void Update() {
        AITick(); 
    }
    
    void AITick() {
        currentState();
        stateTime += Time.deltaTime;
    }
    
    void ChangeState(AIState newAIState) {
        currentState = newAIState;
        stateTime = 0;
    }

    bool CanSeeTarget() {
        return Vector3.Distance(transform.position, player.transform.position) < aggroRange;
    }

    void IdleState() {
        //do nothing
        if(CanSeeTarget()) {
            ChangeState(FollowState);
            return;
        }

        ChangeState(PatrolState);
    }

    void FollowState() {
        MoveToward(player.transform.position);
        //Aim(targetBro.transform);

        if(Vector3.Distance(transform.position, player.transform.position) <= attackRange) {
            ChangeState(AttackState);
        }

        if(Vector3.Distance(transform.position, player.transform.position) >= aggroRange) {
            lastTargetPos = player.transform.position;
            ChangeState(GoToLastTargetPosState);
            return;
        }
    }

    void AttackState() {
        if(Vector3.Distance(transform.position, player.transform.position) <= attackRange) {
            Attack();
        } else {
            ChangeState(FollowState);
        }

    }
    
    void PatrolState() {
        // pick random position
        // move towards it
        // pick new direction once reached
        if(CanSeeTarget()) {
            ChangeState(FollowState);
            return;
        }

        // Don't set new patrol position
        // if(stateTime < 0.1) { // == 0 does not work for some reason
        //     Debug.Log("Setting new patrolArea");
        //     patrolArea = transform.position;
        //     patrolPos = patrolArea + new Vector3(Random.Range(-aggroRange, aggroRange), Random.Range(-aggroRange, aggroRange));
        // }

        if(stateTime > 5 || Vector3.Distance(transform.position, patrolPos) < 0.1f) {
            stateTime = 0f;
            patrolPos = patrolArea + new Vector3(Random.Range(-aggroRange, aggroRange), Random.Range(-aggroRange, aggroRange));
        }

        //Aim(patrolPos);
        MoveToward(patrolPos);
    }

    void GoToLastTargetPosState() {
        //Aim(lastTargetPos);
        MoveToward(lastTargetPos);

        if(CanSeeTarget()) {
            ChangeState(FollowState);
            return;
        }

        if(Vector3.Distance(transform.position, lastTargetPos) < 1) {
            ChangeState(IdleState);
            return;
        }
    }

}
