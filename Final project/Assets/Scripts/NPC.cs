using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] int NPCID = -1;
    bool playerIsClose = false;
    bool questActive = false;
    Quest quest;
    // Start is called before the first frame update
    void Start()
    {
        if(NPCID == -1) {
            Debug.Log("Set the NPC ID for " + gameObject.name);
            return;
        }
        quest = GameController.Instance.GetQuest(NPCID);   // Get quest from controller using quest ID
    }

    void InteractWithPlayer() {
        // Trigger interaction logic if player is nearby
        if(!playerIsClose) {
            return;
        }

        if(questActive) {
            if(quest.complete) {
                QuestReward reward = quest.reward;
                // Show quest complete text
                string displayText = reward.rewardText;
                // Grant ring (if there is one)
                GameController.Instance.AddRing(reward.rewardRing);
                // Grant xp (not implemented)
                
            }
            return;
        }
        //Debug.Log("Giving quest!");
        questActive = true;
        GameController.Instance.ReceiveQuest(NPCID);
    }

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

    void OnEnable() {
        PlayerInputHandler.OnInteractPressed += InteractWithPlayer;
    }

    void OnDisable() {
        PlayerInputHandler.OnInteractPressed -= InteractWithPlayer;
    }
}
