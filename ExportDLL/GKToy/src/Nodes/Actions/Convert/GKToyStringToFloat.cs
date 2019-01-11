using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/转换/字符串转浮点")]
    [NodeTypeTree("Action/Convert/StringToFloat", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("字符串转浮点.")]
    [NodeDescription("String to float  .", "English")]
    public class GKToyStringToFloat : GKToyNode
	{
        [SerializeField]
        GKToySharedString _input = "";
        public GKToySharedString Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedFloat _output = 0;

        public GKToyStringToFloat(int _id) : base(_id) { }

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
            _output.SetValue(float.Parse(Input.Value));
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
