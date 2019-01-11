using System.Linq;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/绝对值")]
    [NodeTypeTree("Action/Math/Abs", "English")]
    [NodeDescription("绝对值运算.")]
    [NodeDescription("Basic calculation, absolute.", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
	public class GKToyAbs : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _inputValue = 0;

        public GKToySharedFloat Input
		{
            get { return _inputValue; }
            set { _inputValue = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyAbs(int _id) : base(_id) { }

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
            _Abs();
            NextAll();
			return 0;
		}

		void _Abs()
		{
            _output.SetValue(Mathf.Abs(Input.Value));
            outputObject = _output;
		}
	}
}
