using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/反正切")]
    [NodeTypeTree("Action/Math/ATan", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("计算并返回参数 f 中指定的数字的反正切值。返回值介于负二分之 pi 与正二分之 pi 之间.")]
    [NodeDescription("Calculates and returns the arc tangent of the number specified in parameter f. The return value is between PI of minus two and Pi of positive two.", "English")]
	public class GKToyATan : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _radian = 0;
        public GKToySharedFloat Radian
		{
            get { return _radian; }
            set { _radian = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyATan(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Atan(Radian.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
