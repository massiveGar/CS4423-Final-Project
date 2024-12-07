using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    DialogueController dialogueController;
    [SerializeField] Quest[] quests;
    int[] enemiesKilled = new int[10];  // Stores the number of each enemyID killed

    void Awake() {
        quests = Resources.LoadAll<Quest>("Quests");
        Array.Sort(quests, (a, b) => a.id.CompareTo(b.id));
    }

    void Start() {
        dialogueController = GameController.Instance.mainUI.GetDialogueController();
    }

    public void SetQuestAtID(Quest quest, int id) {
        quests[id] = quest;
    }
    public Quest GetQuest(int id) {
        if(id >= quests.Count()) {
            return null;
        }
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
        
        if(quest.reward.rewardRingID != -1) {
            GameController.Instance.CreateRing(quest.reward.rewardRingID);    // Grant ring (if there is one)
        }
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

    public int GetEnemiesKilled(int enemyID) {
        return enemiesKilled[enemyID];
    }

    public void SaveQuests() {
        int i = 0;
        foreach(Quest quest in quests) {
            NDSaveLoad.SaveDataDict(Constants.nd_Quest + i, quest.QuestToString()); // complete,progress,active
            i++;
        }
    }
    public void LoadQuests() {
        int i = 0;
        string questString = NDSaveLoad.GetData(Constants.nd_Quest + i, null);
        while(questString != null) {
            string[] questParts = questString.Split(',');

            quests[i].complete = bool.Parse(questParts[0]);
            quests[i].progress = int.Parse(questParts[1]);
            quests[i].active = bool.Parse(questParts[2]);

            i++;
            questString = NDSaveLoad.GetData(Constants.nd_Quest + i, null);
        }
    }
    
    public void SaveKillCount() {
        int i = 0;
        foreach(int tally in enemiesKilled) {
            NDSaveLoad.SaveInt(Constants.nd_EnemyKills + i, tally);
            i++;
        }
    }
    public void LoadKillCount() {
        int i = 0;
        foreach(int tally in enemiesKilled) {
            enemiesKilled[i] = NDSaveLoad.LoadInt(Constants.nd_EnemyKills + i, 0);
            i++;
        }
    }
}
