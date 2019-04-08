using UnityEngine;
using GKToy;

namespace GKToyDialogue
{
    class GKToyMakerDialogueNodeComSelector : GKToyMakerNodeComSelector
    {
        static public new void SelectCom(GKToyNode node)
        {
            switch (node.doubleClickType)
            {
                // Dialogue.
                case 2:
                    GKToyMakerDialogueCom.PopupTaskWindow();
                    GKToyMakerDialogueCom.InitSubData((GKToyDialogue)node);
                    break;
                // Dialogue condition.
                case 4:
                    GKToyMakerDialogueConditionCom.PopupTaskWindow();
                    GKToyMakerDialogueConditionCom.InitSubData((GKToyDialogueCondition)node);
                    break;
                // Dialogue exit.
                case 5:
                    GKToyMakerDialogueExitCom.PopupTaskWindow();
                    GKToyMakerDialogueExitCom.InitSubData((GKToyDialogueExit)node);
                    break;
                // Dialogue action.
                case 6:
                    GKToyMakerDialogueActionCom.PopupTaskWindow();
                    GKToyMakerDialogueActionCom.InitSubData((GKToyDialogueAction)node);
                    break;
                default:
                    break;
            }
        }
    }
}
