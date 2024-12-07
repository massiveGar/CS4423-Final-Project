using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] int NPCID = -1;
    [SerializeField] Dialogue initialDialogue;
    [SerializeField] Dialogue questInProgressDialogue;
    [SerializeField] Dialogue questCompleteDialogue;
    [SerializeField] Dialogue afterQuestDialogue;
    DialogueController dialogueController;
    Quest quest;
    bool playerIsClose = false;
    

    void OnBecameVisible() {
        GameController.OnInteract += InteractWithPlayer;
    }
    void OnBecameInvisible() {
        GameController.OnInteract -= InteractWithPlayer;
    }

    void Start() {
        if(NPCID == -1) {
            Debug.Log("Set the NPC ID for " + gameObject.name);
            return;
        }
        dialogueController = GameController.Instance.mainUI.GetDialogueController();
        quest = GameController.Instance.GetQuest(NPCID);    // Get quest from controller using NPCID (same as quest)

        initialDialogue.id = NPCID;
        if(quest != null) {
            questInProgressDialogue.id = NPCID;
            questCompleteDialogue.id = NPCID;
            afterQuestDialogue.id = NPCID;
        }
    }

    // Trigger interaction logic if player is nearby
    void InteractWithPlayer() {
        if(!playerIsClose) {
            return;
        }

        if(quest != null) {
            // Chat with the player if the quest is active and not complete
            if(quest.active && !quest.complete) {
                dialogueController.StartDialogue(questInProgressDialogue);
                return;
            }

            // Finish the quest if it is active and complete
            if(quest.active && quest.complete) {
                dialogueController.StartDialogue(questCompleteDialogue);
                GameController.Instance.CompleteQuest(NPCID);
                return;
            }

            // Chat with the player if the quest is not active but is complete
            if(!quest.active && quest.complete) {
                dialogueController.StartDialogue(afterQuestDialogue);
                return;
            }
        }

        if(NPCID == 0) {
            GameController.Instance.CreateRing(0);
        }
        // If the quest is not active or complete
        dialogueController.StartDialogue(initialDialogue);    // Talk to the player
    }

    // Detect when the player is nearby
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            playerIsClose = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            playerIsClose = false;
        }
    }
}
