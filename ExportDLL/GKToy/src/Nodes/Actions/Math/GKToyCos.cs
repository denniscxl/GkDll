using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/余弦")]
    [NodeTypeTree("Action/Math/Cos", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回由参数 f 指定的角的余弦值（介于 -1.0 与 1.0 之间的值）.")]
    [NodeDescription("Returns the cosine value of the angle specified by parameter f (between -1.0 and 1).", "English")]
    public class GKToyCos : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _angle = 0;
        public GKToySharedFloat Angle
		{
            get { return _angle; }
            set { _angle = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyCos(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Cos(Angle.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
