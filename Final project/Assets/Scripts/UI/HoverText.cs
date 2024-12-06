using TMPro;
using UnityEngine;

public class HoverText : MonoBehaviour {
    TextMeshProUGUI textBox;

    void Awake() {
        textBox = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    public void ShowText(string ringName) {
        textBox.SetText(ringName);

        gameObject.SetActive(true);
    }

    public void HideText() {
        gameObject.SetActive(false);
    }
}