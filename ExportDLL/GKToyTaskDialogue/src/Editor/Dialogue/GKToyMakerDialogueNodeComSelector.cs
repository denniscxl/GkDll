using UnityEngine;
using GKToy;

namespace GKToyTaskDialogue
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
                default:
                    break;
            }
        }
    }
}
