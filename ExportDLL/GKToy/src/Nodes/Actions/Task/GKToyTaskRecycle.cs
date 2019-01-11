using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/回收")]
	[NodeTypeTree("Action/Task/Recycle", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("回收任务节点.")]
	[NodeDescription("Node of recycle task.", "English")]
	public class GKToyTaskRecycle : GKToyTask
    {
        public GKToyTaskRecycle(int _id) : base(_id){}
        
        // 回收道具 ID.
        [SerializeField]
        private GKToySharedString _itemID = new GKToySharedString();
        public GKToySharedString ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        // 回收次数.
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
