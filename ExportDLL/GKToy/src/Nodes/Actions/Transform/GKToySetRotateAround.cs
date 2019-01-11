using UnityEngine;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("行为/变换/设置围绕旋转")]
    [NodeTypeTree("Action/Transform/SetRotateAround", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Rotation.png")]
	[NodeDescription("设置物体围绕世界坐标的特定点和轴旋转。")]
	[NodeDescription("Rotate object about axis passing through point in world coordinates by angle degrees.", "English")]
	public class GKToySetRotateAround : GKToyNode
    {
		[SerializeField]
		GKToySharedVector3 _point = Vector3.zero;
        [SerializeField]
        GKToySharedVector3 _axis = Vector3.zero;
        [SerializeField]
        GKToySharedFloat _angle = 0;
        public GKToySharedVector3 Point
		{
            get { return _point; }
            set { _point = value; }
		}
        public GKToySharedVector3 Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }
        public GKToySharedFloat Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
        GKToySharedVector3 _output = Vector3.zero;
        Transform _transform;

        public GKToySetRotateAround(int _id) : base(_id) { }

		public override void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
            _output = new GKToySharedVector3();
            outputObject = _output;
            _transform = ovelord.gameObject.GetComponent<Transform>();
		}

		public override int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            if (null != _transform)
			{
                _transform.RotateAround(Point.Value, Axis.Value, Angle.Value);
                _output.SetValue(_transform.rotation.eulerAngles);
                outputObject = _output;
            }
            NextAll();
			return 0;
		}
	}
}
