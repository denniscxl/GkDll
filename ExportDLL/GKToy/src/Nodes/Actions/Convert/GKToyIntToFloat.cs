using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/转换/整型转浮点")]
    [NodeTypeTree("Action/Convert/IntToFloat", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("整型转浮点.")]
	[NodeDescription("Int to float.", "English")]
    public class GKToyIntToFloat : GKToyNode
	{
        [SerializeField]
        GKToySharedInt _input = 0;
        public GKToySharedInt Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedFloat _output = 0;

        public GKToyIntToFloat(int _id) : base(_id) { }

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
            _output.SetValue((float)Input.Value);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
