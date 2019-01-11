using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/插值角度")]
    [NodeTypeTree("Action/Math/LerpAngle", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("根据浮点数Current返回角度From到To之间的插值，Current限制在0～1之间.")]
    [NodeDescription("Based on floating point Current, the interpolation between angle From and To is returned, and Current is limited to 0~1.", "English")]
    public class GKToyLerpAngle : GKToyNode
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

        public GKToyLerpAngle(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.LerpAngle(From.Value, To.Value, Current.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
