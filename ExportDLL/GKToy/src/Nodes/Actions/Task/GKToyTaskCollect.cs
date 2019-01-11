using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/采集")]
	[NodeTypeTree("Action/Task/Collect", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("采集任务节点.")]
	[NodeDescription("Node of collect task.", "English")]
	public class GKToyTaskCollect : GKToyTask
    {
        public GKToyTaskCollect(int _id) : base(_id){ }
        
        // 采集道具 ID.
        [SerializeField]
        private GKToySharedString _itemID = new GKToySharedString();
        public GKToySharedString ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        // Scene ID.
        [SerializeField]
        private GKToySharedString _sceneID = new GKToySharedString();
        public GKToySharedString SceneID
        {
            get { return _sceneID; }
            set { _sceneID = value; }
        }

        // 采集次数.
        [SerializeField]
        private GKToySharedInt _count = new GKToySharedInt();
        public GKToySharedInt Count
        {
            get { return _count; }
            set { _count = value; }
        }

        // 采集Npc ID.
        [SerializeField]
        private GKToySharedString _npcID = new GKToySharedString();
        public GKToySharedString NpcID
        {
            get { return _npcID; }
            set { _npcID = value; }
        }

        // 采集几率.
        [SerializeField]
        private GKToySharedInt _percent = new GKToySharedInt();
        public GKToySharedInt Percent
        {
            get { return _percent; }
            set { _percent = value; }
        }

        // 追踪信息.
        [SerializeField]
        private GKToySharedString _trace = new GKToySharedString();
        public GKToySharedString Trace
        {
            get { return _trace; }
            set { _trace = value; }
        }
    }
}
