using UnityEngine;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("行为/变换/设置旋转")]
    [NodeTypeTree("Action/Transform/SetRotation", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Rotation.png")]
	[NodeDescription("设置对象旋转")]
	[NodeDescription("Set target rotation.", "English")]
	public class GKToySetRotation : GKToyNode
    {
		[SerializeField]
		GKToySharedVector3 _rotation = new GKToySharedVector3();
        public GKToySharedVector3 Rotation
		{
            get { return _rotation; }
            set { _rotation = value; }
		}
        Transform _transform;

        public GKToySetRotation(int _id) : base(_id) { }

		public override void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
            outputObject = Rotation;
            _transform = ovelord.gameObject.GetComponent<Transform>();
		}

		public override int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            if (null != _transform)
			{
                _transform.rotation = Quaternion.Euler(Rotation.Value);
                outputObject = Rotation;
			}
            NextAll();
			return 0;
		}
	}
}
