using System.Collections;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject damageNumberText;
    Coroutine inCombatRoutine = null;
    float regenSpeed = 1;

    [Header("Status")]
    public int maxHealth = 100;
    public float currentHealth = 100;
    float healthRegenPercent = 0.05f;

    int maxMana = 100;
    public float currentMana = 100;
    float manaRegenAmount = 3;

    int maxStamina = 100;
    float currentStamina = 100;

    [Header("Movement")]
    [SerializeField] float speed = 10;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        damageNumberText = Resources.Load<GameObject>("Prefabs/Damage Number");
    }

    void Start() {
        StartCoroutine(PassiveRegen());
    }

    IEnumerator PassiveRegen() {
        while(true) {
            if(currentHealth != maxHealth) {
                currentHealth += healthRegenPercent*maxHealth;
                Mathf.Round(currentHealth);
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            }
            if(currentMana != maxMana) {
                currentMana += manaRegenAmount;
                currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            }
        
            GameController.Instance.UpdatePlayerInfo();
            yield return new WaitForSeconds(regenSpeed);
        }
    }

    // While in combat, reduce the amount of regen
    IEnumerator InCombat() {
        regenSpeed = 2.5f;

        yield return new WaitForSeconds(5);

        regenSpeed = 1;
    }

    public void UseMana(int amount) {
        if(inCombatRoutine != null) {
            StopCoroutine(inCombatRoutine);
        }
        inCombatRoutine = StartCoroutine(InCombat());

        currentMana -= amount;
        GameController.Instance.UpdatePlayerInfo();
    }
    public void TakeDamage(int power) {
        if(inCombatRoutine != null) {
            StopCoroutine(inCombatRoutine);
        }
        inCombatRoutine = StartCoroutine(InCombat());

        currentHealth -= power;
        SpawnDamageNumber(power);
    }
    void SpawnDamageNumber(int number) {
        Vector2 spawnPos = new Vector3(transform.position.x + Random.Range(-0.5f,0.5f), transform.position.y + Random.Range(-0.5f,0.5f), 0);
        Vector2 force = new Vector2(5 * Random.Range(-0.5f,0.5f), 3 * Random.Range(0,0.5f));

        GameObject text = Instantiate(damageNumberText, spawnPos, Quaternion.identity);
        
        text.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        text.GetComponent<TextMeshPro>().SetText(""+number);
        Destroy(text, 1);
    }

    public void SetCL(int rank, int level) {
        maxHealth = rank*100;
        maxHealth += level*10;
        GameController.Instance.UpdatePlayerInfo();
    }

    

    public void UseStamina(int amount) {
        currentStamina -= amount;
        GameController.Instance.UpdatePlayerInfo();
    }

    public void Move(Vector2 movement){
        rb.velocity = movement * speed;
    }

    public bool IsAlive() {
        return currentHealth > 0;
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }
    public int GetMaxHealth() {
        return maxHealth;
    }

    public float GetCurrentMana() {
        return currentMana;
    }
    public int GetMaxMana() {
        return maxMana;
    }

    public float GetCurrentStamina() {
        return currentStamina;
    }
    public int GetMaxStamina() {
        return maxStamina;
    }
}
