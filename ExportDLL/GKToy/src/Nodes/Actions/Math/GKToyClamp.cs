using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/固定")]
    [NodeTypeTree("Action/Math/Clamp", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("限制value的值在min和max之间， 如果value小于min，返回min。 如果value大于max，返回max，否则返回value.")]
    [NodeDescription("Limit the value of value between min and max. If value is less than min, return min. If value is greater than max, return to max, otherwise return value.", "English")]
	public class GKToyClamp : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
		{
            get { return _input; }
            set { _input = value; }
		}
        [SerializeField]
        GKToySharedFloat _min = 0;
        public GKToySharedFloat Min
        {
            get { return _min; }
            set { _min = value; }
        }
        [SerializeField]
        GKToySharedFloat _max = 0;
        public GKToySharedFloat Max
        {
            get { return _max; }
            set { _max = value; }
        }

        GKToySharedFloat _output = 0;

        public GKToyClamp(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Clamp(Input.Value, Min.Value, Max.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
