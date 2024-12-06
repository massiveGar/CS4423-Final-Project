using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [Header("Bars")]
    Image healthBarImage, manaBarImage, staminaBarImage;

    Player player;

    // Subscribe to relevant events, get player statistics, and setup player HUD
    void Awake() {
        GameController.OnPlayerInfoUpdated += UpdateFields;

        healthBarImage = transform.GetChild(0).GetComponent<Image>();
        manaBarImage = transform.GetChild(1).GetComponent<Image>();
        staminaBarImage = transform.GetChild(2).GetComponent<Image>();
    }
    void OnDisable() {
        GameController.OnPlayerInfoUpdated -= UpdateFields;
    }

    void Start() {
        player = GameController.Instance.GetPlayer();

        UpdateFields();
    }

    void UpdateFields() {
        UpdateHealthVisualizer();
        UpdateManaVisualizer();
        UpdateStaminaVisualizer();
    }

    // Called after taking damage or healing ⚕️
    void UpdateHealthVisualizer() {
        // Set the health bar to match the player's health
        float currentHealthPercent = (float) player.GetCurrentHealth()/player.GetMaxHealth();
        healthBarImage.fillAmount = currentHealthPercent;
    }
    // Called after using or regenerating mana
    void UpdateManaVisualizer() {
        // Set the health bar to match the player's health
        float currentManaPercent = (float) player.GetCurrentMana()/player.GetMaxMana();
        manaBarImage.fillAmount = currentManaPercent;
    }
    // Called after using or regenerating stamina
    void UpdateStaminaVisualizer() {
        // Set the health bar to match the player's health
        float currentStaminaPercent = (float) player.GetCurrentStamina()/player.GetMaxStamina();
        staminaBarImage.fillAmount = currentStaminaPercent;
    }
}
