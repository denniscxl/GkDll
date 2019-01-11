using UnityEngine;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("行为/变换/设置平移")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Move.png")]
	[NodeDescription("用向量移动物体，返回物体移动后的坐标。")]
	[NodeTypeTree("Action/Transform/SetTranslate", "English")]
	[NodeDescription("Moves object a certain vector, return position of object after translation.", "English")]
	public class GKToySetTranslate : GKToyNode
    {
		[SerializeField]
		GKToySharedVector3 _translation = new GKToySharedVector3();
		public GKToySharedVector3 Translation
		{
            get { return _translation; }
            set { _translation = value; }
		}
        GKToySharedVector3 _output = Vector3.zero;
        Transform _transform;

        public GKToySetTranslate(int _id) : base(_id) { }

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
                _transform.Translate(Translation.Value);
                _output.SetValue(_transform.position);
                outputObject = _output;
            }
            NextAll();
			return 0;
		}
	}
}
