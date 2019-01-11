using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/浮点转长整型")]
    [NodeTypeTree("Action/Convert/FloatToLong", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("浮点转长整型.")]
	[NodeDescription("Float to long.", "English")]
	public class GKToyFloatToLong : GKToyNode
	{
        [SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedLong _output = 0;

        public GKToyFloatToLong(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedLong();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();
            _output.SetValue((long)Input.Value);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
