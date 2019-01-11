using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/最大整数")]
    [NodeTypeTree("Action/Math/FloorToInt", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回小于或等于Input的最大整数.")]
    [NodeDescription("Returns the largest integer that less than or equal to Input.", "English")]
    public class GKToyFloorToInt : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
		{
            get { return _input; }
            set { _input = value; }
		}

        GKToySharedInt _output = 0;

        public GKToyFloorToInt(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.FloorToInt(Input.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
