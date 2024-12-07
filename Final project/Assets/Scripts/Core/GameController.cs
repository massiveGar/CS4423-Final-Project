using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{   
    [Header("Player")]
    PlayerInputHandler playerInputHandler;  // From find
    public Player player;  // From input handler
    TargetController targetController;  // From player
    RingController ringController;  // From player
    int currentXP = 0;
    int playerRank = 1;
    int playerLevel = 0;

    [Header("Game mechanics")]
    RingAnimationController ringAnimationController;    // From player
    QuestController questController;    // From self
    bool isPaused = false;

    [Header("UI")]
    [NonSerialized] public MainUIController mainUI;
    InventoryController inventoryController;    // From mainUI
    HotbarController hotbarController;   // From mainUI

    public static GameController Instance { get; private set; }
    public static event Action OnTarget;
    public static event Action OnInventory;
    public static event Action OnInteract;
    public static event Action OnPlayerInfoUpdated;
    public static event Action OnTargetChanged;
    public static event Action OnXPUpdated;

    public static event Action OnSave;
    public static event Action OnLoad;

    // Get references
    void Awake() {
        Instance = this;

        mainUI = GameObject.Find("Main UI").GetComponent<MainUIController>();
        inventoryController = mainUI.GetInventoryController();
        hotbarController = mainUI.GetHotbarController();

        questController = GetComponent<QuestController>();
        playerInputHandler = GameObject.Find("PlayerInputHandler").GetComponent<PlayerInputHandler>();
    }

    // Get references from player
    void Start() {
        targetController = player.GetComponentInChildren<TargetController>();
        ringController = player.GetComponentInChildren<RingController>();
        ringAnimationController = player.GetComponentInChildren<RingAnimationController>();

        // debug
        AddXP(3000);

        MainController.Instance.GameControllerLoaded();
    }
    
    // Get components
    public Player GetPlayer() {
        if(player == null) {
            player = playerInputHandler.GetPlayer();
        }
        return player;
    }
    public RingController GetRingController() {
        if(ringController == null) {
            ringController = GetPlayer().GetComponentInChildren<RingController>();
        }
        return ringController;
    }
    public RingAnimationController GetRingAnimationController() {
        if(ringAnimationController == null) {
            ringAnimationController = GetPlayer().GetComponentInChildren<RingAnimationController>();
        }
        return ringAnimationController;
    }

    // Save / Load
    public void Save() {
        // location of every enemy
        // Has to be called with an event otherwise I'd have to reference every single spawner
        OnSave?.Invoke();

        // quests completed
        questController.SaveQuests();

        // enemies killed
        questController.SaveKillCount();

        // xp
        NDSaveLoad.SaveInt(Constants.nd_XP, currentXP);

        // inventory
        ringController.SaveRings();
        inventoryController.SaveInventory();

        // hotbar
        hotbarController.SaveHotbar();

        // player info
        NDSaveLoad.SaveVector3(Constants.nd_PlayPos, player.transform.position);
        NDSaveLoad.SaveFloat(Constants.nd_PHealth, player.GetCurrentHealth());
    }
    public void Load() {
        // location of every enemy
        OnLoad?.Invoke();

        // quests completed
        questController.LoadQuests();

        // enemies killed
        questController.LoadKillCount();

        // xp
        currentXP = NDSaveLoad.LoadInt(Constants.nd_XP, 0);
        AddXP(0);   // So the UI updates

        // inventory
        ringController.LoadRings();
        inventoryController.LoadInventory();

        // hotbar 
        hotbarController.LoadHotbar();

        // player info
        player.transform.position = NDSaveLoad.LoadVector3(Constants.nd_PlayPos, player.transform.position);
        player.SetHealth(NDSaveLoad.LoadFloat(Constants.nd_PHealth, 100));
    }

    public void Quit() {
        MainController.Instance.SaveAndQuit();
    }

    // Pause
    public void Pause() {
        if(isPaused) {
            return;
        }
        Debug.Log("GameController: Game is paused!");
        isPaused = true;
        Time.timeScale = 0;
        MainController.Instance.SwitchToUIControls();
    }
    public void Unpause() {
        if(!isPaused) {
            return;
        }
        Debug.Log("GameController: Game is unpaused!");
        isPaused = false;
        Time.timeScale = 1;
        MainController.Instance.SwitchToGameplayControls();
    }
    public bool IsPaused() {
        return isPaused;
    }
    public void TogglePauseUI() {
        mainUI.TogglePauseScreen();
    }

    // Player interactions
    public bool Aim() {
        return ringAnimationController.AimRingAtEnemy(targetController.GetTarget());
    }
    public bool Attack(int power) {
        return targetController.AttackTarget(power);
    }
    public void TargetEnemy() {
        OnTarget?.Invoke();
    }
    public void Interact() {
        OnInteract?.Invoke();
    }
    public float GetPlayerHealth() {
        return player.GetCurrentHealth();
    }
    public void AttackPlayer(int power) {
        player.TakeDamage(power);
        UpdatePlayerInfo();
    }
    public float GetPlayerMana() {
        return player.GetCurrentMana();
    }
    public void UseMana(int amount) {
        player.UseMana(amount);
    }

    public void UpdatePlayerInfo() {
        OnPlayerInfoUpdated?.Invoke();
    }
    public void AddXP(int xp) {
        currentXP += xp;
        mainUI.SetXPCountText(currentXP);

        OnXPUpdated?.Invoke();
    }
    public int GetCurrentXP() {
        return currentXP;
    }
    public void SetPlayerRank(int rank) {
        playerRank = rank;
    }
    public void SetPlayerLevel(int level) {
        playerLevel = level;
    }

    // Target functions
    public void SignalTargetChanged() {
        OnTargetChanged?.Invoke();
    }
    public GameObject GetTarget() {
        return targetController.GetTarget();
    }
    public void EnemyEnteredScreen(GameObject enemy) {
        targetController.AddVisibleEnemy(enemy);
    }
    public void EnemyLeftScreen(GameObject enemy) {
        targetController.RemoveInvisibleEnemy(enemy);
    }

    // Inventory functions
    public InventoryController GetInventoryController() {
        return inventoryController;
    }
    public void ToggleInventory() {
        OnInventory?.Invoke();
    }

    // Ring functions
    public void CreateRing(int ringID) {
        Ring newRing = ringController.AddNewRing(ringID);
        inventoryController.AddRingToInventory(newRing);    // Add the new ring to the inventory
    }
    public void EquipRing(int hotbarID, int ringNumber) {
        ringController.EquipRing(hotbarID, ringNumber);
    }
    public int GetRingCount() {
        return ringController.GetRingCount();
    }
    public void DestroyRing(Ring victim) {
        AddXP(victim.GetSalvageValue());
        ringController.RemoveRing(victim.ringNumber);
    }
    public Ring GetRing(int ringNumber) {
        return ringController.GetRing(ringNumber);
    }

    // Hotbar functions
    public void ActivateHotbarSlot(int i) {
        ringController.UseRing(i);
    }
    public HotbarSlot GetHotbarSlot(int id) {
        return hotbarController.GetSlot(id);
    }
    public void UpdatePlayerLevel() {
        hotbarController.UpdatePlayerLevel();
        player.SetCL(playerRank, playerLevel);
    }
    public void UpdateCLText() {
        mainUI.SetChargeLevelText(playerRank, playerLevel);
    }

    // Quest functions
    public Quest GetQuest(int id) {
        return questController.GetQuest(id);
    }
    public void EnemyKilled(int id) {
        questController.AddKill(id);
    }
    public void CompleteQuest(int id) {
        questController.CompleteQuest(id);
    }
    public void StartQuest(int id) {
        questController.StartQuest(id);
    }
}
