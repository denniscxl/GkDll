using System.Collections.Generic;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/浮点链表转整型链表")]
    [NodeTypeTree("Action/Convert/FloatLstToIntLst", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("浮点链表转整型链表.")]
	[NodeDescription("Float list to int list.", "English")]
    public class GKToyFloatLstToIntLst : GKToyNode
	{
        [SerializeField]
        GKToySharedFloatLst _input;
        public GKToySharedFloatLst Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedIntLst _output;

        public GKToyFloatLstToIntLst(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedIntLst();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();

            List<int> lst = new List<int>();
            foreach(var v in Input.Value)
            {
                lst.Add((int)v);
            }
            _output.SetValue(lst);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
