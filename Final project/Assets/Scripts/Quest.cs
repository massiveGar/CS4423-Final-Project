using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;


public enum QuestType{Kill,Explore,Fetch};
public struct QuestReward {
    public string rewardText;
    public int rewardXP;
    public Ring rewardRing;
    public QuestReward(string rewardText, int rewardXP, Ring rewardRing=null) {
        this.rewardText = rewardText;
        this.rewardXP = rewardXP;
        this.rewardRing = rewardRing;
    }
};

[System.Serializable]
public class Quest
{
    public string text;    // Quest flavor text
    public string objective;
    public QuestType type; // Kill, explore, fetch quests
    public int requirement;    // Requirement can store info like 10 kills, area ID, or 1 of item ID 32
    public int mobID;  // If kill quest, store the mob's ID
    public QuestReward reward;  // Stores the reward text, rewardRing (if there is one), and reward xp
    public bool complete;
    public int progress;
    public bool active;
    public Quest(string text, string objective, QuestType type, int requirement, QuestReward reward, int mobID=0) {
        this.text = text;
        this.objective = objective;
        this.type = type;
        this.requirement = requirement;
        this.reward = reward;
        this.mobID = mobID;
        complete = false;
        progress = 0;
        active = false;
    }
}