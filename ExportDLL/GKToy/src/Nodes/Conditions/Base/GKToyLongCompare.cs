using System.Collections.Generic;
using UnityEngine;

namespace GKToy
{
    /// <summary>
    /// 长整型比较.
    /// </summary>
	[NodeTypeTree("条件/基本/长整型比较")]
    [NodeTypeTree("Condition/Base/IntCompare", "English")]
	[NodeDescription("判断两长整型数之间的关系是否满足条件\n若不满足，走第一条连接；若满足，走其他连接")]
	[NodeDescription("Check whether the relation of two long integers meet requirement.\nIf true, go to the first link, or go to others.", "English")]
    public class GKToyLongCompare : GKToyNode
	{
        GKToySharedLong _current = 0;
        public GKToySharedLong Current
        {
            get { return _current; }
            set { _current = value; }
        }

        [SerializeField]
        GKToySharedLong _target = 0;
        public GKToySharedLong Target
        {
            get { return _target; }
            set { _target = value; }
        }

        [SerializeField]
        CompareType _compareType = CompareType.EqualTo;
        public CompareType CompareType
        {
            get { return _compareType; }
            set { _compareType = value; }
        }

        /// <summary>
        /// 输出节点列表.
        /// </summary>
        List<int> _lst;

        public GKToyLongCompare(int _id) : base(_id) { }

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
                switch(CompareType)
                {
                    case CompareType.BiggerThan:
                        if ((int)Current.Value > (int)Target.Value)
                        {
                            success = true;
                        }
                        break;
                    case CompareType.EqualTo:
                        if ((int)Current.Value == (int)Target.Value)
                        {
                            success = true;
                        }
                        break;
                    case CompareType.LessThan:
                        if ((int)Current.Value < (int)Target.Value)
                        {
                            success = true;
                        }
                        break;
                    case CompareType.NotEqualTo:
                        if ((int)Current.Value != (int)Target.Value)
                        {
                            success = true;
                        }
                        break;
                }

                if (success)
                {
                    for (int i = 1; i < links.Count; i++)
                        _lst.Add(links[i].next);
                }
                else
                {
                    _lst.Add(links[0].next);
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

    public enum CompareType
    {
        LessThan = 0,
        BiggerThan,
        EqualTo,
        NotEqualTo
    }
}
