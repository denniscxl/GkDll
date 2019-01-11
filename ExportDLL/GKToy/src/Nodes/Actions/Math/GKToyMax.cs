using UnityEngine;
using System.Collections.Generic;

namespace GKToy
{
    [NodeTypeTree("行为/数学/最大值")]
    [NodeTypeTree("Action/Math/Max", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回两个或更多值中最大的值.")]
    [NodeDescription("Returns the maximum value of two or more values.", "English")]
	public class GKToyMax : GKToyNode
	{
		[SerializeField]
        GKToySharedFloatLst _lst = new GKToySharedFloatLst();
        public GKToySharedFloatLst Lst
		{
            get { return _lst; }
            set { _lst = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyMax(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedFloat();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0; 
            
            base.Update();
            _output.SetValue(Mathf.Max(Lst.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
