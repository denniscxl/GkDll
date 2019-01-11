using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/自然常数指数")]
    [NodeTypeTree("Action/Math/Exp", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回 e 的 power 次方的值.")]
    [NodeDescription("Returns the value of the power power of E.", "English")]
	public class GKToyExp : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _power = 0;
        public GKToySharedFloat Power
		{
            get { return _power; }
            set { _power = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyExp(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Exp(Power.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
