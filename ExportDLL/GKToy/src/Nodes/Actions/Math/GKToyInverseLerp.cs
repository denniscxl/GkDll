using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/反插值")]
    [NodeTypeTree("Action/Math/InverseLerp", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("计算两个值之间的Lerp参数. 也就是Current在From和To之间的比例值.")]
    [NodeDescription("Calculate the Lerp parameter between the two values. That is the ratio between Current and From and To.", "English")]
    public class GKToyInverseLerp : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _from = 0;
        public GKToySharedFloat From
		{
            get { return _from; }
            set { _from = value; }
		}
        [SerializeField]
        GKToySharedFloat _to = 0;
        public GKToySharedFloat To
        {
            get { return _to; }
            set { _to = value; }
        }
        [SerializeField]
        GKToySharedFloat _current = 0;
        public GKToySharedFloat Current
        {
            get { return _current; }
            set { _current = value; }
        }

        GKToySharedFloat _output = 0;

        public GKToyInverseLerp(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.InverseLerp(From.Value, To.Value, Current.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
