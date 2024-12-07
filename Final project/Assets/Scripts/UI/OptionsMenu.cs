using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
    PlayerInputActions playerControls;

    // Keybind references
    KeybindButton[] keybindButtons;
    bool rebinding = false;
    InputActionMap actionMap;
    GameObject waitForKeyImage;
    
    // Resolution references
    [SerializeField] Toggle fullscreenToggle, vsyncToggle;
    TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    List<Resolution> filteredResolutions;
    float currentRefreshRate;
    int currentResolutionIndex = 0;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    AudioMixer audioMixer;

    void Awake() {
        keybindButtons = GetComponentsInChildren<KeybindButton>();
        resolutionDropdown = GetComponentInChildren<TMP_Dropdown>();
        Array.Sort(keybindButtons, (a, b) => a.buttonID.CompareTo(b.buttonID));

        waitForKeyImage = transform.Find("WaitForKey Image").gameObject;
        waitForKeyImage.SetActive(false);
    }

    void OnEnable() {

    }
    void OnDisable() {
        SavePreferences();
    }

    void Start() {
        MainController.Instance.SetOptionsMenu(this);

        InitializeResolutions();
        // Load resolution from PlayerPrefs, otherwise use current resolution
        resolutionDropdown.value = PlayerPrefs.GetInt(Constants.pref_ResolutionIndex, currentResolutionIndex);   

        playerControls = MainController.Instance.GetPlayerControls();
        actionMap = MainController.Instance.GetActionMap();
        
        audioMixer = Resources.Load<AudioMixer>("Audio/Main");
        // Load the volume settings from the PlayerPrefs, these will update the slider display and the audio volume
        masterVolumeSlider.value = PlayerPrefs.GetFloat(Constants.s_MasterVolume, 0);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.s_MusicVolume, 0);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(Constants.s_SfxVolume, 0);

        UpdateToggleButtons();
        UpdateKeybindButtons();
    }

    // Used by the rebind buttons
    public void ChooseRebindAction(int buttonID) {
        StartRebinding(GetActionIndex(buttonID), GetBindingIndex(buttonID));
    }

    InputAction GetInputAction(int actionIndex) {
        return actionMap.actions[actionIndex];
    }

    // Hard code map the buttons to action/binding index, there's probably a better way to do this
    int GetActionIndex(int buttonID) {
        if(buttonID <= 4) { // Buttons 1-4 = action 0 (Move)
           return 0;
        } else if(buttonID <= 9) {  // Buttons 5-9 = action 1 (Hotbar Action)
            return 1; 
        } else {
            return buttonID-8;  // Buttons 10,11,12 = actions 2,3,4
        }
    }
    int GetBindingIndex(int buttonID) {
        if(buttonID <= 4) { // Buttons 1-4 = binding 1-4 (up, left, down, right)
           return buttonID;
        } else if(buttonID <= 9) {  // Buttons 5-9 = binding 0-4
            return buttonID-5; 
        } else {
            return 0;  // Buttons 10,11,12 = actions 2,3,4
        }
    }

    // Read the key values for all of the binds and update the keybind text
    void UpdateKeybindButtons() {
        for(int i = 0; i < keybindButtons.Count(); i++) {
            InputAction action = GetInputAction(GetActionIndex(i+1));
            KeybindButton button = keybindButtons[i];
            
            button.keybindText.SetText(action.bindings[GetBindingIndex(i+1)].ToDisplayString());
        }
    }

    // Called by rebind button
    // For example, StartRebinding(0,1) would rebind the Move, Up keybind
    public void StartRebinding(int actionIndex, int bindingIndex) {
        if(rebinding) {
            StopRebinding();
        }
        waitForKeyImage.SetActive(true);
        rebinding = true;
        InputAction rebindingAction = GetInputAction(actionIndex);   // Get the action from the list using the actionIndex (Move, Interact, etc)
        
        // Set the rebind operation, excluding mouse inputs
        var rebind = rebindingAction.PerformInteractiveRebinding(bindingIndex).WithControlsExcluding("Mouse");  
        rebind.OnComplete(operation => {
            StopRebinding();
            operation.Dispose();
        });
        // check for conflicting bind
        
        rebind.Start(); // Start the rebind operation, which waits for first keyboard input
    }
    public void StopRebinding() {
        rebinding = false;
        waitForKeyImage.SetActive(false);
        // Update button text to show new keybind
        UpdateKeybindButtons();
    }

    public void SetVsync() {
        bool state = vsyncToggle.isOn;
        MainController.Instance.SetVsync(state);
    }
    public void SetFullscreen() {
        bool state = fullscreenToggle.isOn;
        MainController.Instance.SetFullscreen(state);
    }

    void InitializeResolutions() {
        resolutions = Screen.resolutions;   // Get all resolutions
        filteredResolutions = new List<Resolution>();   // Will store only relevant resolutions

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        // Filter the resolutions by refresh rate of the monitor
        for(int i = 0; i < resolutions.Length; i++) {
            if((float)resolutions[i].refreshRateRatio.value == currentRefreshRate) {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        // Sort by width/height
        filteredResolutions.Sort((a, b) => {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();
        for(int i = 0; i < filteredResolutions.Count; i++) {
            // Get all resolutions from the filteredResolutions list as strings for the dropdown text
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);

            // Find the current resolution so we have a default fallback
            if(filteredResolutions[i].width == Screen.width && 
                filteredResolutions[i].height == Screen.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
    }
    // Select the dropdown option at resolutionIndex and save this in PlayerPrefs
    public void SetResolution(int resolutionIndex) {
        Resolution resolution = filteredResolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, MainController.Instance.IsFullScreenEnabled());

        resolutionDropdown.RefreshShownValue();
        PlayerPrefs.SetInt(Constants.pref_ResolutionIndex, resolutionIndex);
    }

    public void ChangeMasterVolume(float value) {
        audioMixer.SetFloat(Constants.s_MasterVolume, value);
        PlayerPrefs.SetFloat(Constants.s_MasterVolume, value);
    }
    public void ChangeMusicVolume(float value) {
        audioMixer.SetFloat(Constants.s_MusicVolume, value);
        PlayerPrefs.SetFloat(Constants.s_MusicVolume, value);
    }
    public void ChangeSfxVolume(float value) {
        audioMixer.SetFloat(Constants.s_SfxVolume, value);
        PlayerPrefs.SetFloat(Constants.s_SfxVolume, value);
    }

    void UpdateToggleButtons() {
        vsyncToggle.isOn = MainController.Instance.IsVsyncEnabled();
        fullscreenToggle.isOn = MainController.Instance.IsFullScreenEnabled();
    }

    void SavePreferences() {
        string keybinds = actionMap.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(Constants.pref_PlayerControls, keybinds);
        PlayerPrefs.Save();
    }
}
