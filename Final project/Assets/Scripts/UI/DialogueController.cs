using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Dialogue rootDialogue;
    TextMeshProUGUI dialogueText;
    GameObject continueTextObject;
    DialogueNode currentNode;
    Button continueButton;

    [SerializeField] Transform choiceButtonArea;
    [SerializeField] GameObject choiceButtonPrefab;

    int textIndex = 0;
    bool nodeFinished = false;
    bool dialogueFinished = true;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
         
        continueButton = GetComponent<Button>();
        
        dialogueText = transform.Find("Dialogue Text").GetComponent<TextMeshProUGUI>();
        continueTextObject = transform.Find("Continue Text").gameObject;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // Receive a dialogue node and start the dialogue sequence
    public void StartDialogue(Dialogue dialogue) {
        if(dialogue == null) {
            return;
        }

        dialogueFinished = false;
        textIndex = 0;

        rootDialogue = dialogue;
        currentNode = rootDialogue.rootnode;
        NextDialogue();

        ToggleVisibility();
        GameController.Instance.Pause();
    }

    // Called by continue button
    // Pause the game, display the next line of dialogue, and remove the dialogue from the array
    // If there is no more dialogue, hide the UI and unpause
    public void NextDialogue() {
        if(currentNode == null) {
            return;
        }

        if(textIndex < currentNode.dialogueText.Count) {
            // Dialogue not finished, display the next line
            SetText(currentNode.dialogueText[textIndex]);
            textIndex++;
            EnableClickToContinue();
        } 

        if(textIndex >= currentNode.dialogueText.Count) {
            DisableClickToContinue();
            nodeFinished = true;
        }

        if(nodeFinished && !currentNode.HasBranch()) {  // Dialogue finished, and there is no branch
            // Show "End Conversation" button
            GameObject newButton = Instantiate(choiceButtonPrefab, choiceButtonArea);
            newButton.GetComponentInChildren<TextMeshProUGUI>().SetText("End Conversation");
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectResponse(null));

            return;
        } else if(nodeFinished) {   // Dialogue finished, and there is a branch
            // Show dialogue choices, disable continue
            continueButton.enabled = false;

            foreach(DialogueResponse response in currentNode.dialogueResponses) {
                GameObject newButton = Instantiate(choiceButtonPrefab, choiceButtonArea);
                newButton.GetComponentInChildren<TextMeshProUGUI>().SetText(response.responseText);
                newButton.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
            }

            return;
        }

        
    }

    public void SelectResponse(DialogueResponse response) {
        foreach(Transform buttonTransform in choiceButtonArea) {
            Destroy(buttonTransform.gameObject);
        }

        if(currentNode.grantQuest) {
            GameController.Instance.StartQuest(rootDialogue.id);  // Start the quest associated with this dialogue
        }

        if(response == null) {
            FinishDialogue();
            return;
        }

        currentNode = response.nextNode;
        if(currentNode != null) {
            nodeFinished = false;
            textIndex = 0;
            NextDialogue();
        } else {    
            FinishDialogue();
        }
    }

    void FinishDialogue() {
        ToggleVisibility();

        GameController.Instance.Unpause();
        currentNode = null;
        rootDialogue = null;
    }

    void EnableClickToContinue() {
        continueTextObject.SetActive(true);
        continueButton.enabled = true;
    }
    void DisableClickToContinue() {
        continueTextObject.SetActive(false);
        continueButton.enabled = false;
    }

    void SetText(string text) {
        dialogueText.SetText(text);
    }

    public void ToggleVisibility() {
        canvasGroup.alpha = -(canvasGroup.alpha - 1);
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }

    public bool DialogueFinished() {
        return dialogueFinished;
    }

    
}
