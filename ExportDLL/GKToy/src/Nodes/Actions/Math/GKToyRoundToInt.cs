using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/四舍五入")]
    [NodeTypeTree("Action/Math/RoundToInt", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("返回Input指定的值四舍五入到最近的整数。\n如果数字末尾是.5, 将返回偶数。")]
    [NodeDescription("Returns the value specified by Input rounded to the nearest integer. \nIf the number ends with .5, it returns even.", "English")]
	public class GKToyRoundToInt : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _input = 0;
        public GKToySharedFloat Input
		{
            get { return _input; }
            set { _input = value; }
		}

        GKToySharedInt _output = 0;

        public GKToyRoundToInt(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.RoundToInt(Input.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
