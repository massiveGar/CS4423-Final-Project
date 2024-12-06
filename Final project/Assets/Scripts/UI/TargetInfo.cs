using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfo : MonoBehaviour
{
    TextMeshProUGUI targetText;
    Image targetHealthBar;
    DefaultEnemyAI target;
    CanvasGroup canvasGroup;
    bool updatingFields = false;
    Coroutine updatingCoroutine;

    void Awake() {
        targetText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        targetHealthBar = transform.GetChild(1).GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start() {
        canvasGroup.alpha = 0;
    }

    void OnEnable() {
        GameController.OnTargetChanged += ChangeTarget;
    }
    void OnDisable() {
        GameController.OnTargetChanged -= ChangeTarget;
    }
    
    // Called when selecting a new target (Q)
    void ChangeTarget() {
        GameObject targetObject =  GameController.Instance.GetTarget();
        
        if(targetObject == null) {  // No target, hide the target display
            canvasGroup.alpha = 0;
            target = null;
            StopCoroutine(updatingCoroutine);
            updatingFields = false;
            return;
        } else {    // Set the target variable to access its stats for the status bars
            target = targetObject.GetComponent<DefaultEnemyAI>();
            canvasGroup.alpha = 1;
            if(!updatingFields) {
                updatingCoroutine = StartCoroutine(UpdateFields());
            }
        }
    }

    IEnumerator UpdateFields() {
        updatingFields = true;
        while(target != null) {
            UpdateHealthBar();
            UpdateTargetText();
            yield return null;
        }
    }

    void UpdateHealthBar() {
        float targetHealthPercentage = (float) target.currentHealth / target.MaxHealth;
        targetHealthBar.fillAmount = targetHealthPercentage;
    }

    void UpdateTargetText() {
        targetText.SetText("Target: " + target.EnemyName + "\nC.L. " + target.rank + "." + target.level);
    }
}
