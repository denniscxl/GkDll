using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/决斗")]
	[NodeTypeTree("Action/Task/Duel", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("决斗任务节点.")]
	[NodeDescription("Node of duel task.", "English")]
	public class GKToyTaskDuel : GKToyTask
    {
        public GKToyTaskDuel(int _id) : base(_id){}

        // 决斗目标 ID.
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

        // 决斗前对话.
        [SerializeField]
        private GKToySharedString _dialogue = new GKToySharedString();
        public GKToySharedString Dialogue
        {
            get { return _dialogue; }
            set { _dialogue = value; }
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
