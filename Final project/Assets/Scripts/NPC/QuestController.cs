using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    DialogueController dialogueController;
    [SerializeField] Quest[] quests;
    int[] enemiesKilled = new int[10];

    void Awake() {
        quests = Resources.LoadAll<Quest>("Quests");
        Array.Sort(quests, (a, b) => a.id.CompareTo(b.id));
    }

    void Start() {
        dialogueController = GameController.Instance.mainUI.GetDialogueController();
    }

    public void AddQuest(Quest quest, int id) {
        quests[id] = quest;
    }
    public Quest GetQuest(int id) {
        return quests[id];
    }

    // Set the quest as active and display the quest dialogue
    public void StartQuest(int id) {
        Quest quest = quests[id];
        quest.active = true;
        // StartCoroutine(WaitBeforeStartingQuest(quest));
    }
    public void CompleteQuest(int id) {
        Quest quest = quests[id];
        
        GameController.Instance.CreateRing(quest.reward.rewardRingID);    // Grant ring (if there is one)
        GameController.Instance.AddXP(quest.reward.rewardXP); // Grant XP

        quest.active = false;
    }

    IEnumerator WaitBeforeStartingQuest(Quest quest) {
        while(!dialogueController.DialogueFinished()) {
            yield return null;
        }
        quest.active = true;
        // Display text "Quest start! + objective"
        Debug.Log("Quest start! " + quest.objective);
    }

    public void AddKill(int id) {
        enemiesKilled[id]++;

        foreach(Quest quest in quests) {
            if(quest == null) {
                continue;
            }
            if(!quest.active || quest.complete){
                continue;
            }
            if(quest.mobID == id) {
                quest.progress++;
                Debug.Log("Quest progress: " + quest.progress);
                if(quest.progress == quest.requirement) {
                    quest.complete = true;
                    Debug.Log("Quest done!");
                }
            }
        }
    }
}
