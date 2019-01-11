using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/数学/增量角")]
    [NodeTypeTree("Action/Math/DeltaAngle", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Calculate.png")]
    [NodeDescription("计算给定的两个角之间最短的差异.")]
    [NodeDescription("Calculate the shortest difference between two given angles.", "English")]
	public class GKToyDeltaAngle : GKToyNode
	{
		[SerializeField]
        GKToySharedFloat _angleA = 0;
        public GKToySharedFloat AngleA
		{
            get { return _angleA; }
            set { _angleA = value; }
		}

        [SerializeField]
        GKToySharedFloat _angleB = 0;
        public GKToySharedFloat AngleB
        {
            get { return _angleB; }
            set { _angleB = value; }
        }

        GKToySharedFloat _output = 0;

        public GKToyDeltaAngle(int _id) : base(_id) { }

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
            _output.SetValue(Mathf.DeltaAngle(AngleA.Value, AngleB.Value));
            outputObject = _output;
            NextAll();
			return 0;
		}
	}
}
