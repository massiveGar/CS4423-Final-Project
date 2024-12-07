using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    PlayerInputActions playerControls;
    [SerializeField] Player player;
    
    // All player InputActions, there's probably a better way to do this
    InputAction move;
    InputAction interact;
    InputAction useHotbar;
    InputAction toggleInventory;
    InputAction targetEnemy;
    InputAction pauseGame;

    void Awake() {
        playerControls = MainController.Instance.GetPlayerControls();
    }

    void Start() {
        if(playerControls == null) {
            Debug.Log("PlayerInputHandler: playerControls null");
        }
    }

    // Assign all InputActions and enable them, there's definitely a better way to do this
    void OnEnable() {
        move = playerControls.Player.Move;
        move.Enable();

        interact = playerControls.Player.Interact;
        interact.Enable();
        interact.performed += Interact;

        useHotbar = playerControls.Player.HotbarAction;
        useHotbar.Enable();
        useHotbar.performed += SelectHotbar;

        toggleInventory = playerControls.Player.ToggleInventory;
        toggleInventory.performed += ToggleInventory;
        toggleInventory.Enable();

        targetEnemy = playerControls.Player.TargetEnemy;
        targetEnemy.performed += TargetEnemy;
        targetEnemy.Enable();
        
        pauseGame = playerControls.Player.Pause;
        pauseGame.performed += PauseGame;
        pauseGame.Enable();
    }
    // Disable all input actions to prevent memory leaks, I think
    void OnDisable() {
        move.Disable();
        interact.Disable();
        useHotbar.Disable();
        toggleInventory.Disable();
        pauseGame.Disable();
    }

    // Player movement calculated every frame
    void Update() {
        if(GameController.Instance.IsPaused()) {
            return;
        }
        Vector2 movement = move.ReadValue<Vector2>();

        player.Move(movement);
    }

    // All player actions
    private void Interact(InputAction.CallbackContext context) {
        GameController.Instance.Interact();
    }
    private void SelectHotbar(InputAction.CallbackContext context) {
        int bindingIndex = context.action.GetBindingIndexForControl(context.control);
        
        GameController.Instance.ActivateHotbarSlot(bindingIndex);
    }
    private void ToggleInventory(InputAction.CallbackContext context) {
        GameController.Instance.ToggleInventory();
    }
    private void TargetEnemy(InputAction.CallbackContext context) {
        GameController.Instance.TargetEnemy();
    }
    void PauseGame(InputAction.CallbackContext context) {
        if(GameController.Instance.IsPaused()) {
            GameController.Instance.Unpause();
        } else {
            GameController.Instance.Pause();
        }
        GameController.Instance.TogglePauseUI();
    }

    public Player GetPlayer() {
        return player;
    }
}
