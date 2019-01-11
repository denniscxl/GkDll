using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/狩猎")]
	[NodeTypeTree("Action/Task/Hunting", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("狩猎任务节点.")]
	[NodeDescription("Node of hunting task.", "English")]
	public class GKToyTaskHunting : GKToyTask
    {
        public GKToyTaskHunting(int _id) : base(_id){ }

        // 猎杀目标 ID.
        [SerializeField]
        private GKToySharedString _npcID = new GKToySharedString();
        public GKToySharedString NpcID
        {
            get { return _npcID; }
            set { _npcID = value; }
        }

        // Scene ID.
        [SerializeField]
        private GKToySharedString _sceneID = new GKToySharedString();
        public GKToySharedString SceneID
        {
            get { return _sceneID; }
            set { _sceneID = value; }
        }

        // 猎杀次数.
        [SerializeField]
        private GKToySharedInt _count = new GKToySharedInt();
        public GKToySharedInt Count
        {
            get { return _count; }
            set { _count = value; }
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
