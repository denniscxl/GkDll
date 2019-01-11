using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/基本/日志")]
	[NodeTypeTree("Action/Base/NLog", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Log.png")]
	[NodeDescription("控制台日志输出.")]
	[NodeDescription("Console log output.", "English")]
	public class GKToyNLog : GKToyNode
	{
		[SerializeField]
        GKToySharedString _content = "";

		public GKToySharedString Content
        {
            get { return _content; }
			set 
            {
                _content = value;
                outputObject = _content;
            }
        }

        public GKToyNLog(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = Content;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();
			Debug.Log(Content);
			NextAll();
			return 0;
		}
	}
}
