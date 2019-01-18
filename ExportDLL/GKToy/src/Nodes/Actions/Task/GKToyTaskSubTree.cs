using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/任务/子任务树")]
	[NodeTypeTree("Action/Task/SubTree", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("子任务树节点.")]
	[NodeDescription("Node of sub task tree.", "English")]
	public class GKToyTaskSubTree : GKToyNode
    {
        public GKToyTaskSubTree(int _id) : base(_id)
        {
            // Sub task tree.
            doubleClickType = 3;
        }
        
        //子树ID.
        [SerializeField]
        private GKToySharedInt _subTreeID = new GKToySharedInt();
        public GKToySharedInt SubTreeID
        {
            get { return _subTreeID; }
            set { _subTreeID = value; }
        }
    }
}
