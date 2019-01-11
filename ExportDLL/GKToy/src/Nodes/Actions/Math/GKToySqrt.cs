using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/平方根")]
    [NodeTypeTree("Action/Math/Sqrt", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("计算并返回 Input 的平方根.")]
    [NodeDescription("Calculate and return the square root of Input.", "English")]
    public class GKToySqrt : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
		{
            get { return _input; }
            set { _input = value; }
		}

        GKToySharedInt _output = 0;

        public GKToySqrt(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedInt();
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0; 

            base.Update();
            _output.SetValue(Mathf.Sqrt(Input.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
