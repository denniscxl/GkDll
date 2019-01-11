using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/正弦")]
    [NodeTypeTree("Action/Math/Sin", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("计算并返回以弧度为单位指定的角 Angle 的正弦值.")]
    [NodeDescription("Calculates and returns the sine value of the angle Angle specified in the radian unit.", "English")]
    public class GKToySin : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _angle = 0;
        public GKToySharedFloat Angle
		{
            get { return _angle; }
            set { _angle = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToySin(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Sin(Angle.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
