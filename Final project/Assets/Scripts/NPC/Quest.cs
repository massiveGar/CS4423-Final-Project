using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;


public enum QuestType{Kill,Explore,Fetch};

[System.Serializable]
public struct QuestReward {
    public List<string> rewardText;
    public int rewardXP;
    public int rewardRingID;
};

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public int id;
    public string objective;
    public QuestType type; // Kill, explore, fetch quests
    public int requirement;    // Requirement can store info like 10 kills, area ID, or 1 of item ID 32
    public int mobID;  // If kill quest, store the mob's ID
    public QuestReward reward;  // Stores the reward text, rewardRing (if there is one), and reward xp
    [NonSerialized] public bool complete = false;
    [NonSerialized] public int progress = 0;
    [NonSerialized] public bool active = false;

    // Return the quest as complete,progress,active
    public string QuestToString() {
        return complete + "," + progress + "," + active;
    }
}