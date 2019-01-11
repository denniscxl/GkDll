using System.Linq;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/减法")]
    [NodeTypeTree("Action/Math/Sub", "English")]
    [NodeDescription("基本的减法运算.")]
	[NodeDescription("Basic calculation, substruction.", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    public class GKToySub : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _inputValueA = 0;
        public GKToySharedFloat InputA
		{
            get { return _inputValueA; }
            set { _inputValueA = value; }
		}

        [SerializeField]
        GKToySharedFloat _inputValueB = 0;
        public GKToySharedFloat InputB
		{
            get { return _inputValueB; }
            set { _inputValueB = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToySub(int _id) : base(_id) { }

        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedFloat();
            outputObject = _output;
        }

		public override int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            _Minus();
            NextAll();
			return 0;
		}

        void _Minus()
        {
            _output.SetValue(InputA.Value - InputB.Value);
            outputObject = _output;
        }
	}
}
