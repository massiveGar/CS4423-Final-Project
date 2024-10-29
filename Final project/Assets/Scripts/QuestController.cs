using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField] Quest[] quests;
    int[] enemiesKilled = new int[10];

    void Awake() {
        quests = new Quest[10];
    }

    void Start() {
        

        // Tutorial quest, kill 5 slimes
        Ring rewardRing = new Ring(1, "Hack", 5, 5);
        QuestReward reward = new QuestReward("Great job!", 10, rewardRing);
        Quest quest = new Quest("Flavor text", "Kill 3 slimes", QuestType.Kill, 3, reward, 1);
        AddQuest(quest, 1);
    }

    public void AddQuest(Quest quest, int id) {
        quests[id] = quest;
    }
    public Quest GetQuest(int id) {
        return quests[id];
    }
    public void ActivateQuest(int id) {
        Debug.Log("Quest '" + quests[id].objective + "' start!");
        quests[id].active = true;
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
