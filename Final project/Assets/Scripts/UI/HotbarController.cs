using UnityEngine;

public class HotbarController : MonoBehaviour
{
    HotbarSlot[] hotbarSlots = new HotbarSlot[5];

    void Start() {
        hotbarSlots = GetComponentsInChildren<HotbarSlot>();

        UpdatePlayerLevel();
    }

    // Get the CL average of all hotbar rings and send it to GameController
    public void UpdatePlayerLevel() {
        float totalLevel = 0;
        foreach(HotbarSlot slot in hotbarSlots) {
            Ring slotRing = slot.GetSlottedRing();
            if(slotRing == null) {
                totalLevel += 1;
                continue;
            }

            totalLevel += slotRing.rank;
            totalLevel += slotRing.level/10f;
        }
        totalLevel /= 5f;
        Debug.Log("Hotbar: " + totalLevel);
        
        int rank = (int)totalLevel;
        int level = Mathf.RoundToInt((totalLevel-rank)*10);
        if(level == 10) {
            rank++;
            level = 0;
        }

        GameController.Instance.SetPlayerLevel(level);
        GameController.Instance.SetPlayerRank(rank);

        GameController.Instance.UpdateCLText();
    }

    public HotbarSlot GetSlot(int id) {
        return hotbarSlots[id];
    }
}
