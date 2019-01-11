using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/转换/三维向量转二维向量")]
    [NodeTypeTree("Action/Convert/Vector3ToVector2", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("三维向量转二维向量.")]
	[NodeDescription("Vector3 to vector2.", "English")]
    public class GKToyVector3ToVector2 : GKToyNode
	{
        [SerializeField]
        GKToySharedVector3 _input = Vector3.zero;
        public GKToySharedVector3 Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedVector2 _output = Vector2.zero;

        public GKToyVector3ToVector2(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedVector2();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();
            _output.SetValue((Vector2)Input.Value);
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
