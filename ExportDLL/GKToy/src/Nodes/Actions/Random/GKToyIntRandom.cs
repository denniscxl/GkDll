using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/随机/随机整型")]
    [NodeTypeTree("Action/Random/IntRandom", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Random.png")]
    [NodeDescription("随机整型.")]
    [NodeDescription("Random int.", "English")]
	public class GKToyIntRandom : GKToyNode
	{
        [SerializeField]
        GKToySharedInt m_min = 0;
        public GKToySharedInt Min
        {
            get { return m_min; }
            set { m_min = value; }
        }

		[SerializeField]
        GKToySharedInt m_max = 0;
        public GKToySharedInt Max
        {
            get { return m_max; }
            set { m_max = value;}
        }
        GKToySharedInt _output = 0;

        public GKToyIntRandom(int _id) : base(_id) { }
        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedInt();
            outputObject = _output;
        }

        public override int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();
            _output.SetValue(Random.Range(Min.Value, Max.Value + 1));
            outputObject = _output;
			NextAll();
			return 0;
		}
	}
}
