using UnityEditor;
using UnityEngine;
using GKToy;

namespace GKToyTaskEditor
{
    [NodeTypeTree("辅助/任务/狩猎")]
	[NodeTypeTree("Decoration/Task/Hunting", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("狩猎任务节点.")]
	[NodeDescription("Node of hunting task.", "English")]
	public class GKToySubTaskHunting : GKToySubTask
    {
        public GKToySubTaskHunting(int _id) : base(_id)
        {
            doubleClickType = 2;
            TargetType = 2;
            TargetInfo = "猎杀 [HuntNpc]";
            TargetText = "猎杀 [HuntNpc]";
        }

        // 猎杀目标.
        [SerializeField]
        private GKToySharedString _huntNpc = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString HuntNpc
        {
            get { return _huntNpc; }
            set { _huntNpc = value; }
        }

        // 猎杀次数.
        [SerializeField]
        private GKToySharedInt _huntCount = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt HuntCount
        {
            get { return _huntCount; }
            set { _huntCount = value; }
        }
    }
}
