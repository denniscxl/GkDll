using UnityEngine;
using System.Collections.Generic;

namespace GKToy
{
    [NodeTypeTree("行为/数学/圆周率")]
    [NodeTypeTree("Action/Math/PI", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回圆周率的值.")]
    [NodeDescription("Returns the value of the circumference.", "English")]
	public class GKToyPI : GKToyNode
	{
        GKToySharedFloat _output = 0;

        public GKToyPI(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.PI);
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
