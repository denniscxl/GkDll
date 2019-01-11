using UnityEngine;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("行为/变换/设置坐标")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Move.png")]
	[NodeDescription("移动物体到某一点。")]
	[NodeTypeTree("Action/Transform/SetPosition", "English")]
	[NodeDescription("Move object to a point.", "English")]
	public class GKToySetPosition : GKToyNode
    {
		[SerializeField]
		GKToySharedVector3 _position = new GKToySharedVector3();
		public GKToySharedVector3 Position
		{
            get { return _position; }
            set { _position = value; }
		}
        Transform _transform;

        public GKToySetPosition(int _id) : base(_id) { }

		public override void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
            outputObject = Position;
            _transform = ovelord.gameObject.GetComponent<Transform>();
		}

		public override int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            if (null != _transform)
			{
                _transform.position = Position.Value;
                outputObject = Position;
			}
            NextAll();
			return 0;
		}
	}
}
