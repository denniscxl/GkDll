using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/三维向量转字符串")]
    [NodeTypeTree("Action/Convert/Vector3ToString", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
	[NodeDescription("三维向量转为字符串。")]
	[NodeDescription("Convert Vector3 to string.", "English")]
    public class GKToyVector3ToString : GKToyNode
	{
        [SerializeField]
        GKToySharedVector3 _input = Vector3.zero;
        public GKToySharedVector3 Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedString _output = "";

        public GKToyVector3ToString(int _id) : base(_id) { }

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
            _output.SetValue(string.Format("{0},{1},{2}", Input.Value.x, Input.Value.y, Input.Value.y));
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
