using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/浮点转整型")]
    [NodeTypeTree("Action/Convert/FloatToInt", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("浮点转整型.")]
	[NodeDescription("Float to int.", "English")]
    public class GKToyFloatToInt : GKToyNode
	{
        [SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedInt _output = 0;

        public GKToyFloatToInt(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedInt();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();
            _output.SetValue((int)Input.Value);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
