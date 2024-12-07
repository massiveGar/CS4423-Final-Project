using System.Linq;
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

    // Save/load the ringNumber that each slot is carrying
    public void SaveHotbar() {
        for(int i = 0; i < hotbarSlots.Count(); i++) {
            Ring slottedRing = hotbarSlots[i].GetSlottedRing();
            if(slottedRing == null) {
                continue;
            }
            NDSaveLoad.SaveInt(Constants.nd_Hotbar + i, slottedRing.ringNumber);
        }
    }
    public void LoadHotbar() {
        for(int i = 0; i < hotbarSlots.Count(); i++) {
            int ringNumber = NDSaveLoad.LoadInt(Constants.nd_Hotbar + i, -1);
            if(ringNumber == -1) {
                return;
            }
            Debug.Log("HotbarController: Equipping ring " + ringNumber);
            hotbarSlots[i].SetRing(GameController.Instance.GetRing(ringNumber));
            GameController.Instance.EquipRing(i, ringNumber);
        }
    }
}
