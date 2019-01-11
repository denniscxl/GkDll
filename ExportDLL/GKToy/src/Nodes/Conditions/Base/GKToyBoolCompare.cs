using System.Collections.Generic;
using UnityEngine;

namespace GKToy
{
    /// <summary>
    /// 布尔比较.
    /// </summary>
	[NodeTypeTree("条件/基本/布尔比较")]
    [NodeTypeTree("Condition/Base/BoolCompare", "English")]
	[NodeDescription("判断两布尔之间的关系是否满足条件\n若不满足，走第一条连接；若满足，走其他连接")]
	[NodeDescription("Check bool requirement.\nIf true, go to the first link, or go to others.", "English")]
	public class GKToyBoolCompare : GKToyNode
	{
        GKToySharedBool _current = true;
        public GKToySharedBool Current
        {
            get { return _current; }
            set { _current = value; }
        }

        [SerializeField]
        GKToySharedBool _target = true;
        public GKToySharedBool Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// 输出节点列表.
        /// </summary>
        List<int> _lst;

        public GKToyBoolCompare(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = false;
        }

        override public void Enter()
		{
			base.Enter();
            _lst = new List<int>();
            _lst.Clear();
        }

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            bool success = false;

            // 如果输出节点为空, Hold On.
            if (0 == links.Count)
                return 0;

            // 根据触发状态, 决策输出节点.
            if(links.Count > 1)
            {
                if(Current.Value != Target.Value)
                {
                    success = false;
                    _lst.Add(links[0].next);
                }
                else
                {
                    success = true;
                    for (int i = 1; i < links.Count; i++)
                        _lst.Add(links[i].next);
                }  
            }
            else
            {
                _lst.Add(links[0].next);
            }

            outputObject = success;

            NextAll(_lst);

            return 0;
		}
	}
}
