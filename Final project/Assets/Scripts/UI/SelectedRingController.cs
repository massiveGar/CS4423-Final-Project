using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedRingController : MonoBehaviour
{
    public InventoryItem selectedItem;
    public Ring selectedRing;
    Image itemImage;
    Sprite emptySprite;
    Button upgradeButton, salvageButton;
    TextMeshProUGUI itemStatNumbers, upgradeValue, salvageValue, itemDescription;

    void Awake() {
        GameController.OnXPUpdated += UpdateUpgradeButton;

        itemImage = transform.Find("Item Image").GetComponent<Image>();
        emptySprite = itemImage.sprite;

        upgradeButton = transform.Find("Upgrade Button").GetComponent<Button>();
        salvageButton = transform.Find("Salvage Button").GetComponent<Button>();

        itemStatNumbers = transform.Find("Item Stat Numbers").GetComponent<TextMeshProUGUI>();
        upgradeValue = transform.Find("Upgrade Value").GetComponent<TextMeshProUGUI>();
        salvageValue = transform.Find("Salvage Value").GetComponent<TextMeshProUGUI>();
        
        itemDescription = transform.Find("Item Description").GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnDisable() {
        GameController.OnXPUpdated -= UpdateUpgradeButton;
    }

    void Start() {
        ClearFields();
    }

    public void SelectItem(InventoryItem item) {
        if(selectedItem == item) {
            return;
        }
        selectedItem = item;

        selectedRing = selectedItem.GetRing();
        if(selectedRing == null) {
            ClearFields();
            return;
        }

        UpdateFields();
    }

    public void UnselectItem() {
        ClearFields();
    }

    void UpdateFields() {
        if(selectedItem == null || selectedItem.GetRing() == null) {
            return;
        }

        itemImage.sprite = selectedItem.GetRingImage().sprite;
        itemDescription.SetText(selectedRing.ringDescription);
        itemStatNumbers.SetText(selectedRing.power + "\n" + selectedRing.resource.cost + "\n" + selectedRing.rank + "." + selectedRing.level);
        
        upgradeValue.SetText("Cost: " + selectedRing.rank);
        salvageValue.SetText("Gain: " + selectedRing.GetSalvageValue());

        UpdateUpgradeButton();
        if(GameController.Instance.GetRingCount() == 1) {
            salvageButton.interactable = false;
        } else {
            salvageButton.interactable = true;
        }
    }
    void ClearFields() {
        selectedItem = null;
        selectedRing = null;

        itemImage.sprite = emptySprite;
        itemDescription.SetText("");
        itemStatNumbers.SetText("");

        upgradeValue.SetText("Cost: ");
        salvageValue.SetText("Gain: ");

        upgradeButton.interactable = false;
        salvageButton.interactable = false;
    }
    void UpdateUpgradeButton() {
        if(selectedRing == null) {
            return;
        }

        if(selectedRing.rank < 10 && selectedRing.rank <= GameController.Instance.GetCurrentXP()) {
            upgradeButton.interactable = true;
        } else {
            upgradeButton.interactable = false;
        }
    }

    public void UpgradeRing() {
        GameController.Instance.AddXP(-selectedRing.rank);

        selectedRing.IncrementLevel();

        UpdateFields();
    }

    public void SalvageRing() {
        selectedItem.Clear();
        selectedRing.Salvage();

        ClearFields();
    }
}
