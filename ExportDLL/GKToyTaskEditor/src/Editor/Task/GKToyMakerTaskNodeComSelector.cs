using UnityEngine;
using GKToy;

namespace GKToyTaskEditor
{
    class GKToyMakerTaskNodeComSelector : GKToyMakerNodeComSelector
    {
        static public void SelectCom(GKToyNode node, GKToyData data)
        {
            switch (node.doubleClickType)
            {
                // Task.
                case 0:
                    GKToyMakerTaskCom.PopupTaskWindow();
                    GKToyMakerTaskCom.InitSubData((GKToyTask)node, data);
                    break;
                // Interact Task.
                case 1:
                    GKToyMakerSubInteractCom.PopupTaskWindow();
                    GKToyMakerSubInteractCom.InitSubData((GKToySubTaskInteract)node, data);
                    break;
                // Hunt Task.
                case 2:
                    GKToyMakerSubHuntingCom.PopupTaskWindow();
                    GKToyMakerSubHuntingCom.InitSubData((GKToySubTaskHunting)node, data);
                    break;
                // Collect Task.
                case 4:
                    GKToyMakerSubCollectCom.PopupTaskWindow();
                    GKToyMakerSubCollectCom.InitSubData((GKToySubTaskCollect)node, data);
                    break;
                default:
                    break;
            }
        }
    }
}
