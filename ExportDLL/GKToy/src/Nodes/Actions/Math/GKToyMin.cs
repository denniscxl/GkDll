using UnityEngine;
using System.Collections.Generic;

namespace GKToy
{
    [NodeTypeTree("行为/数学/最小值")]
    [NodeTypeTree("Action/Math/Min", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回两个或更多值中最小的值.")]
    [NodeDescription("Returns the minimum value of two or more values.", "English")]
    public class GKToyMin : GKToyNode
	{
		[SerializeField]
        GKToySharedFloatLst _lst = new GKToySharedFloatLst();
        public GKToySharedFloatLst Lst
		{
            get { return _lst; }
            set { _lst = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyMin(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Min(Lst.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
