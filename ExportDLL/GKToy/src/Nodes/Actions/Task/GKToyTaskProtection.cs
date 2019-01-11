using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/护送")]
	[NodeTypeTree("Action/Task/Protection", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("护送任务节点.")]
	[NodeDescription("Node of protection task.", "English")]
	public class GKToyTaskProtection : GKToyTask
    {
        public GKToyTaskProtection(int _id) : base(_id){ }

        // 护送对象 ID.
        [SerializeField]
        private GKToySharedString _targetID = new GKToySharedString();
        public GKToySharedString TargetID
        {
            get { return _targetID; }
            set { _targetID = value; }
        }

        // 目标场景 ID.
        [SerializeField]
        private GKToySharedString _sceneID = new GKToySharedString();
        public GKToySharedString SceneID
        {
            get { return _sceneID; }
            set { _sceneID = value; }
        }

        // 坐标X.
        [SerializeField]
        private GKToySharedFloat _x = new GKToySharedFloat();
        public GKToySharedFloat X
        {
            get { return _x; }
            set { _x = value; }
        }

        // 坐标Y.
        [SerializeField]
        private GKToySharedFloat _y = new GKToySharedFloat();
        public GKToySharedFloat Y
        {
            get { return _y; }
            set { _x = value; }
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
