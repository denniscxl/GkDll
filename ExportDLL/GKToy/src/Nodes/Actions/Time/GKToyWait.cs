using UnityEngine;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("行为/时间/等待")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Wait.png")]
	[NodeDescription("等待一定的时间，单位：秒.")]
	[NodeTypeTree("Action/Time/Wait", "English")]
	[NodeDescription("Wait for certain time(s).", "English")]
	public class GKToyWait : GKToyNode
    {
		[SerializeField]
        GKToySharedFloat _waitTime = 0;
        public GKToySharedFloat WaitTime
		{
            get { return _waitTime; }
            set { _waitTime = value; }
		}
		float _curTime;

        public GKToyWait(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = null;
        }

        override public void Enter()
		{
			base.Enter();
            _curTime = 0;
		}

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            _curTime += Time.deltaTime;
            if (_curTime >= (float)WaitTime.Value)
			{
                NextAll();
			}
			return 0;
		}
	}
}
