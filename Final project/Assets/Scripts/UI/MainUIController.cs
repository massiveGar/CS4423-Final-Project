using TMPro;
using UnityEngine;

public class MainUIController : MonoBehaviour
{
    InventoryController inventoryController;
    SelectedRingController selectedRingController;
    HoverText hoverText;

    GameObject HUD;
    HotbarController hotbarController;
    PlayerInfo playerInfoPanel;
    TargetInfo targetInfoPanel;
    TextMeshProUGUI xpCountText, chargeLevelText;

    Canvas mainUICanvas;
    DialogueController dialogueController;
    GameObject pauseMenu;
    GameObject optionsMenu;

    void Awake() {
        inventoryController = GetComponentInChildren<InventoryController>();
        hoverText = GetComponentInChildren<HoverText>(true);
        mainUICanvas = GetComponent<Canvas>();
        selectedRingController = GetComponentInChildren<SelectedRingController>();

        HUD = transform.Find("HUD").gameObject;
        hotbarController = HUD.GetComponentInChildren<HotbarController>();
        targetInfoPanel = HUD.GetComponentInChildren<TargetInfo>();

        playerInfoPanel = HUD.GetComponentInChildren<PlayerInfo>();
        xpCountText = playerInfoPanel.transform.Find("XP Count Text").GetComponent<TextMeshProUGUI>();
        SetXPCountText(0);
        chargeLevelText = playerInfoPanel.transform.Find("Charge Level Text").GetComponent<TextMeshProUGUI>();

        dialogueController = GetComponentInChildren<DialogueController>();

        pauseMenu = transform.Find("Pause Menu").gameObject;
        optionsMenu = pauseMenu.GetComponentInChildren<OptionsMenu>(true).gameObject; 
        pauseMenu.SetActive(false);
    }    

    public InventoryController GetInventoryController() {
        return inventoryController;
    }

    public HoverText GetHoverText() {
        return hoverText;
    }

    public HotbarController GetHotbarController() {
        return hotbarController;
    }

    public PlayerInfo GetPlayerInfo() {
        return playerInfoPanel;
    }

    public TargetInfo GetTargetInfo() {
        return targetInfoPanel;
    }

    public Canvas GetMainCanvas() {
        return mainUICanvas;
    }

    public SelectedRingController GetSelectedRingController() {
        return selectedRingController;
    }

    public void SetXPCountText(int xp) {
        xpCountText.SetText(""+xp);
    }
    public void SetChargeLevelText(int rank, int level) {
        
        chargeLevelText.SetText("CL: "+rank+"."+level);
    }

    public DialogueController GetDialogueController() {
        return dialogueController;
    }

    public void TogglePauseScreen() {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void BackToGameButton() {
        TogglePauseScreen();
        GameController.Instance.Unpause();
    }
    public void ToggleOptionsMenu() {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }
    public void SaveAndQuitButton() {
        GameController.Instance.Unpause();
        MainController.Instance.SaveAndQuit();
    }
}
