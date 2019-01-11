using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/对话")]
	[NodeTypeTree("Action/Task/Dialogue", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("对话任务节点.")]
	[NodeDescription("Node of dialogue task.", "English")]
	public class GKToyTaskDialogue : GKToyTask
    {

        public GKToyTaskDialogue(int _id) : base(_id){}

        // Npc ID.
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

        // 对话树ID.
        [SerializeField]
        private GKToySharedInt _treeID = new GKToySharedInt();
        public GKToySharedInt TreeID
        {
            get { return _treeID; }
            set { _treeID = value; }
        }

        // 交互类型.
        [SerializeField]
        private GKToySharedInt _interactiveType = new GKToySharedInt();
        public GKToySharedInt InteractiveType
        {
            get { return _interactiveType; }
            set { _interactiveType = value; }
        }

        // 追踪信息.
        [SerializeField]
        private GKToySharedString _trace = new GKToySharedString();
        public GKToySharedString Trace
        {
            get { return _trace; }
            set { _trace = value; }
        }

        // 交互次数.
        [SerializeField]
        private GKToySharedInt _count = new GKToySharedInt();
        public GKToySharedInt Count
        {
            get { return _count; }
            set { _count = value; }
        }

        // 交互动画.
        [SerializeField]
        private GKToySharedString _animation = new GKToySharedString();
        public GKToySharedString Animation
        {
            get { return _animation; }
            set { _animation = value; }
        }

        // 交互时间.
        [SerializeField]
        private GKToySharedFloat _time = new GKToySharedFloat();
        public GKToySharedFloat Time
        {
            get { return _time; }
            set { _time = value; }
        }

        // 交互效果.
        [SerializeField]
        private GKToySharedInt _effect = new GKToySharedInt();
        public GKToySharedInt Effect
        {
            get { return _effect; }
            set { _effect = value; }
        }
    }
}
