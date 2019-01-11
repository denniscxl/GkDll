using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/对数")]
    [NodeTypeTree("Action/Math/Log", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回指定基数的指定对数。")]
    [NodeDescription("Returns the logarithm of a specified number in a specified base.", "English")]
	public class GKToyLog : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _base = 0;
        public GKToySharedFloat Base
		{
            get { return _base; }
            set { _base = value; }
		}
        [SerializeField]
        GKToySharedFloat _real = 0;
        public GKToySharedFloat Real
        {
            get { return _real; }
            set { _real = value; }
        }

        GKToySharedFloat _output = 0;

        public GKToyLog(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Log(Base.Value, Real.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
