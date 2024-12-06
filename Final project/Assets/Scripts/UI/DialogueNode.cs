using System.Collections.Generic;

[System.Serializable]
public class DialogueNode {
    public List<string> dialogueText;
    public List<DialogueResponse> dialogueResponses;
    public bool grantQuest = false;
    public bool openShop = false;

    public bool HasBranch() {
        return dialogueResponses.Count > 0;
    }
}