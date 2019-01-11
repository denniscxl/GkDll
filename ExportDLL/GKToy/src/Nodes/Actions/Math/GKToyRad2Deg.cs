using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/弧度转角度")]
    [NodeTypeTree("Action/Math/Rad2Deg", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("将弧度转化到角度。")]
    [NodeDescription("Transform radian into degree.", "English")]
	public class GKToyRad2Deg : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _angle = 0;
        public GKToySharedFloat Angle
		{
            get { return _angle; }
            set { _angle = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyRad2Deg(int _id) : base(_id) { }

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
            _output.SetValue(Angle.Value * Mathf.Rad2Deg);
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
