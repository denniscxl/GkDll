using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/浮点转字符串")]
    [NodeTypeTree("Action/Convert/FloatToString", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
	[NodeDescription("浮点转字符串.")]
	[NodeDescription("Float to string.", "English")]
    public class GKToyFloatToString : GKToyNode
	{
        [SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedString _output = "";

        public GKToyFloatToString(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedString();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            _output.SetValue(Input.Value.ToString());
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
