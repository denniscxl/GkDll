using System.Collections.Generic;
using System.Linq;

namespace GKToy
{
	[InvisibleNode]
	[NodeTypeTree("行为/基本/开始")]
	[NodeTypeTree("Action/Base/Start", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Start.png")]
	[NodeDescription("流程开始的节点，唯一。")]
	[NodeDescription("Node where the process starts from. Unique.", "English")]
	public class GKToyStart : GKToyNode
    {

        public GKToyStart(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = null;
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
