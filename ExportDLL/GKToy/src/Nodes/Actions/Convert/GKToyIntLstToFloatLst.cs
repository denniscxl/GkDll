using System.Collections.Generic;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/转换/整型链表转浮点链表")]
    [NodeTypeTree("Action/Convert/IntLstToFloatLst", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("整型链表转浮点链表.")]
    [NodeDescription("Int list to float list.", "English")]
	public class GKToyIntLstToFloatLst : GKToyNode
	{
        [SerializeField]
        GKToySharedIntLst _input;
        public GKToySharedIntLst Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedFloatLst _output;

        public GKToyIntLstToFloatLst(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedFloatLst();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();

            List<float> lst = new List<float>();
            foreach(var v in Input.Value)
            {
                lst.Add(v);
            }
            _output.SetValue(lst);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
