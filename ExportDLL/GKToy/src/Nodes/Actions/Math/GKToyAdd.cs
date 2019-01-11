using System.Linq;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/加法")]
    [NodeTypeTree("Action/Math/Add", "English")]
	[NodeDescription("基本的加法运算.")]
	[NodeDescription("Basic calculation, addition.", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
	public class GKToyAdd : GKToyNode
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

        public GKToyAdd(int _id) : base(_id) { }

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
            _Add();
            NextAll();
			return 0;
		}

		void _Add()
		{
            _output.SetValue(InputA.Value + InputB.Value);
            outputObject = _output;
		}
	}
}
