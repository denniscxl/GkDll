using System.Linq;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/次方")]
    [NodeTypeTree("Action/Math/Pow", "English")]
    [NodeDescription("计算并返回Base的Pow次方.")]
    [NodeDescription("Calculate and return the Pow power of Base.", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
	public class GKToyPow : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _base = 0;
		public GKToySharedFloat Base
		{
            get { return _base; }
            set { _base = value; }
		}

        [SerializeField]
        GKToySharedFloat _pow = 0;
        public GKToySharedFloat Pow
		{
            get { return _pow; }
            set { _pow = value; }
		}
        GKToySharedFloat _output = 0;

        public GKToyPow(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Pow(Base.Value, Pow.Value));
            outputObject = _output;
		}
	}
}
