using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/对话树/分支")]
	[NodeTypeTree("Action/DialogueTree/branch", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("分支节点.")]
	[NodeDescription("Node of branch.", "English")]
	public class GKToyBranch : GKToyNode
    {
        public GKToyBranch(int _id) : base(_id)
        {
        }
        
        override public void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
        }

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();

            NextAll();
			return 0;
		}
    }
}
