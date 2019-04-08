using UnityEditor;
using UnityEngine;
using GKToy;

namespace GKToyTaskEditor
{
    [NodeTypeTree("辅助/任务/采集")]
	[NodeTypeTree("Decoration/Task/Collect", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("采集任务节点.")]
	[NodeDescription("Node of collect task.", "English")]
	public class GKToySubTaskCollect : GKToySubTask
    {
        public GKToySubTaskCollect(int _id) : base(_id)
        {
            doubleClickType = 4;
            TargetType = 4;
            TargetInfo = "采集 [GatherNpc]";
            TargetText = "采集 [GatherNpc]";
        }
        
        // 采集目标.
        [SerializeField]
        private GKToySharedString _gatherItem = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString GatherItem
        {
            get { return _gatherItem; }
            set { _gatherItem = value; }
        }

        // 采集类型.
        [SerializeField]
        private GKToySharedInt _gatherType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt GatherType
        {
            get { return _gatherType; }
            set { _gatherType = value; }
        }

        // 采集Npc.
        [SerializeField]
        private GKToySharedString _gatherNpc = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString GatherNpc
        {
            get { return _gatherNpc; }
            set { _gatherNpc = value; }
        }

        // 采集数量.
        [SerializeField]
        private GKToySharedInt _gatherCount = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt GatherCount
        {
            get { return _gatherCount; }
            set { _gatherCount = value; }
        }
    }
}
