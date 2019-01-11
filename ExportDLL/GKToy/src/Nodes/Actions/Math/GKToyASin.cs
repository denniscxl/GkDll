using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/反正弦")]
    [NodeTypeTree("Action/Math/ASin", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("以弧度为单位计算并返回参数 f 中指定的数字的反正弦值.")]
    [NodeDescription("Calculates and returns the arcsine value of the number specified in parameter f in radian.", "English")]
	public class GKToyASin : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _radian = 0;
        public GKToySharedFloat Radian
		{
            get { return _radian; }
            set { _radian = value; }
		}

        GKToySharedFloat _output = 0;

        public GKToyASin(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.Asin(Radian.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
