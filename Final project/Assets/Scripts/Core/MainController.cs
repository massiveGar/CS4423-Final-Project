using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {
    public static MainController Instance { get; private set; }
    
    int resolutionWidth, resolutionHeight;
    PlayerInputActions playerControls;
    InputActionMap actionMap;

    bool vsyncEnabled, fullscreenEnabled;
    
    [SerializeField] OptionsMenu options;

    void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        playerControls = new PlayerInputActions();
        actionMap = playerControls.asset.FindActionMap("Player");   // We will only rebind keys from the Player action map

        string keybinds = PlayerPrefs.GetString("PlayerControls", null);
        if(keybinds != null) {
            actionMap.LoadBindingOverridesFromJson(keybinds);
        }

        vsyncEnabled = PlayerPrefs.GetInt(Constants.pref_Vsync, 1) == 1;
        fullscreenEnabled = PlayerPrefs.GetInt(Constants.pref_Fullscreen, 1) == 1;
    }

    void Start() {
        SwitchToUIControls();

        SetVsync(vsyncEnabled);
        SetFullscreen(fullscreenEnabled);
    }

    public void NewGame() {
        SceneManager.LoadScene("Gameplay");
        SwitchToGameplayControls();
    }

    public void ToggleOptions() {
        options.gameObject.SetActive(!options.gameObject.activeSelf);
    }

    public void Exit() {
        Application.Quit();
    }

    public PlayerInputActions GetPlayerControls() {
        if(playerControls == null) {
            Debug.Log("MainController: playerControls null");
        }
        return playerControls;
    }
    public InputActionMap GetActionMap() {
        return actionMap;
    }

    public void SetOptionsMenu(OptionsMenu options) {   // When switching scenes, the option menu would be lost
        this.options = options;
    }

    public void SwitchToUIControls() {
        playerControls.Player.Disable();
        playerControls.UI.Enable();
    }
    public void SwitchToGameplayControls() {
        playerControls.Player.Enable();
        playerControls.UI.Disable();
    }

    public void SetVsync(bool state) {
        int stateInt = state ? 1 : 0;
        PlayerPrefs.SetInt(Constants.pref_Vsync, stateInt);
        vsyncEnabled = state;
        QualitySettings.vSyncCount = stateInt;
    }
    public bool IsVsyncEnabled() {
        return vsyncEnabled;
    }
    public void SetFullscreen(bool state) {
        PlayerPrefs.SetInt(Constants.pref_Fullscreen, state ? 1 : 0);
        fullscreenEnabled = state;
        Screen.fullScreen = state;
    }
    public bool IsFullScreenEnabled() {
        return fullscreenEnabled;
    }

    public void SaveTheGame() {
        
    }
}
