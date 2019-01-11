using System.Linq;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/乘法")]
    [NodeTypeTree("Action/Math/Multiply", "English")]
    [NodeDescription("基本的乘法运算.")]
    [NodeDescription("Basic calculation, Multiply.", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
	public class GKToyMultiply : GKToyNode
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

        public GKToyMultiply(int _id) : base(_id) { }

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
            _Multiply();
            NextAll();
			return 0;
		}

        void _Multiply()
        {
            _output.SetValue(InputA.Value * InputB.Value);
            outputObject = _output;
        }
	}
}
