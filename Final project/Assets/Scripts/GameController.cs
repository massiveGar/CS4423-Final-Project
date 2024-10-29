using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{   

    [SerializeField] Canvas mainCanvas;
    [Header("Player")]
    [SerializeField] PlayerInputHandler playerInputHandler;
    [SerializeField] Player player;
    TargetController targetController;  // From player
    RingController ringController;  // From player

    [Header("Game mechanics")]
    RingAnimationController ringAnimationController;    // From player
    [SerializeField] InventoryController inventoryController;
    [SerializeField] QuestController questController;

    public static GameController Instance { get; private set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep object alive across scenes
        } else {    // Prevent duplicates
            Destroy(gameObject); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        targetController = player.GetComponentInChildren<TargetController>();
        ringController = player.GetComponentInChildren<RingController>();
        ringAnimationController = player.GetComponentInChildren<RingAnimationController>();
    }

    public Player GetPlayer() {
        return player;
    }

    public void EnemyEnteredScreen(GameObject enemy) {
        targetController.AddVisibleEnemy(enemy);
    }
    public void EnemyLeftScreen(GameObject enemy) {
        targetController.RemoveInvisibleEnemy(enemy);
    }

    public bool Aim() {
        return ringAnimationController.AimRingAtEnemy(targetController.GetTarget());
    }

    public void Attack(int power) {
        targetController.GetTarget().GetComponent<DefaultEnemyAI>().TakeDamage(power);
    }

    public void PlayAnimation() {
        //ringAnimationController.Animate();
    }

    public void TargetEnemy() {
        targetController.TargetEnemy();
    }

    public void AddRing(Ring ring) {
        ringController.AddRing(ring);
    }

    public void UseRing(int i) {
        ringController.UseRing(i);
    }

    public void ToggleInventory() {
        inventoryController.ToggleInventory();
    }

    public Quest GetQuest(int id) {
        return questController.GetQuest(id);
    }

    public void ReceiveQuest(int id) {
        questController.ActivateQuest(id);
    }

    public void EnemyKilled(int id) {
        questController.AddKill(id);
    }

    public void AttackPlayer(int power) {
        player.TakeDamage(power);
    }

    public Canvas GetCanvas() {
        return mainCanvas;
    }
}
