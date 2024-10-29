using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Status")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] float health = 100;
    [SerializeField] float regenSpeed = 2;

    [Header("Movement")]
    [SerializeField] float speed = 2;
    [SerializeField] float speedLimit = 10;
    [SerializeField] float rotationSpeed = 10;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        StartCoroutine(PassiveRegen());
    }

    IEnumerator PassiveRegen() {
        while(true) {
            if(health == maxHealth) {
                yield return null;
            }
            health += regenSpeed;
            health = math.min(health, maxHealth);
            yield return new WaitForSeconds(1);
        }
    }

    public void TakeDamage(int power) {
        health -= power;
    }
    
    void FixedUpdate() {
        if(rb.velocity.magnitude > speedLimit) {
            rb.velocity = rb.velocity.normalized * speedLimit;
        }
    }

    public void Move(Vector3 movement){
        rb.velocity = movement * speed;
    }

    public bool IsAlive() {
        return health > 0;
    }
}
