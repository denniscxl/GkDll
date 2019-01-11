using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/正切")]
    [NodeTypeTree("Action/Math/Tan", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("计算并返回以弧度为单位 Radian 指定角度的正切值.")]
    [NodeDescription("Calculates and returns the tangent value of the specified angle in radian Radian.", "English")]
    public class GKToyTan : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _radian = 0;
        public GKToySharedFloat Radian
		{
            get { return _radian; }
            set { _radian = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyTan(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Tan(Radian.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
