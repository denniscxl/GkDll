using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/变换/设置注视")]
    [NodeTypeTree("Action/Transform/SetLookAt", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Rotation.png")]
	[NodeDescription("旋转物体，使前向向量指向目标位置，返回物体的角度。")]
	[NodeDescription("Rotate object so the forward vector points at target position, return the rotation of object.", "English")]
	public class GKToySetLookAt : GKToyNode
    {
		[SerializeField]
		GKToySharedVector3 _target = new GKToySharedVector3();
        [SerializeField]
        GKToySharedVector3 _axis = Vector3.up;
        public GKToySharedVector3 Target
		{
            get { return _target; }
            set { _target = value; }
		}
        public GKToySharedVector3 Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }
        GKToySharedVector3 _output = Vector3.zero;
        Transform _transform;

        public GKToySetLookAt(int _id) : base(_id) { }

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
                _transform.LookAt(Target.Value, Axis.Value);
                _output.SetValue(_transform.rotation.eulerAngles);
                outputObject = _output;
            }
            NextAll();
			return 0;
		}
	}
}
