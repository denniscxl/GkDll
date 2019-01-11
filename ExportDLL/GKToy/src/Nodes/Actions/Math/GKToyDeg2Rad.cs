using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/角度转弧度")]
    [NodeTypeTree("Action/Math/Deg2Rad", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("将角度值转化成弧度值。")]
    [NodeDescription("Transform degree into radian.", "English")]
    public class GKToyDeg2Rad : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _angle = 0;
        public GKToySharedFloat Angle
		{
            get { return _angle; }
            set { _angle = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyDeg2Rad(int _id) : base(_id) { }

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
            _output.SetValue(Angle.Value * Mathf.Deg2Rad);
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
