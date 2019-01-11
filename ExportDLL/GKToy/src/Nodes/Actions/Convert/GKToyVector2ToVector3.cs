using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/二维向量转三维向量")]
    [NodeTypeTree("Action/Convert/Vector2ToVector3", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("二维向量转三维向量.")]
	[NodeDescription("Vector2 to vector3.", "English")]
    public class GKToyVector2ToVector3 : GKToyNode
	{
        [SerializeField]
        GKToySharedVector2 _input = Vector2.zero;
        public GKToySharedVector2 Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedVector3 _output = Vector3.zero;

        public GKToyVector2ToVector3(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedVector3();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();
            _output.SetValue((Vector3)Input.Value);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
