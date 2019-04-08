using System.Collections.Generic;
using System.Linq;
using GKStateMachine;

namespace GKToy
{
    [InvisibleNode]
    [NodeTypeTree("行为/基本/结束")]
	[NodeTypeTree("Action/Base/End", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/End.png")]
	[NodeDescription("该节点完成后，整个流程终止，所有未完成的节点将被打断。")]
	[NodeDescription("Process stops after this node finished, and all nodes unfinished will be interupted.", "English")]
	public class GKToyEnd : GKToyNode
	{
        public GKToyEnd(int _id) : base(_id)
        {
            type = 100;
        }

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
            machine.StopAll(id);
			return 0;
		}
	}
}
