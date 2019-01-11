using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/基本/重置运行状态")]
    [NodeTypeTree("Action/Base/ClearState", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Remove.png")]
    [NodeDescription("重置所有后续节点的运行状态")]
    [NodeDescription("Reset running state of all subsequent nodes.", "English")]
    public class GKToyClearState : GKToyNode
    {
        public GKToyClearState(int _id) : base(_id) { }

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
#if UNITY_EDITOR
            ResetAfterState();
            machine.ClearResetList();
#endif
            NextAll();
            return 0;
        }

        public override void ResetAfterState()
        {
            List<int> lst = new List<int>();
            foreach (var l in links)
            {
                lst.Add(l.next);
            }
            machine.ResetState(id, lst);
        }
    }
}
