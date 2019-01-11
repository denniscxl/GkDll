using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变量/设置字符串变量")]
	[NodeTypeTree("Action/Variable/Set string var", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Variable.png")]
	[NodeDescription("设置字符串变量.")]
	[NodeDescription("Set string variable.", "English")]
    public class GKToySetStringVar : GKToyNode
	{
		[SerializeField]
        GKToySharedString _key = "";
		public GKToySharedString Key
        {
            get { return _key; }
			set { _key = value;}
        }
        [SerializeField]
        GKToySharedString _target = "";
        public GKToySharedString Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public GKToySetStringVar(int _id) : base(_id) { }
        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = Target;
        }

        public override int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();

            if(null != _overlord)
            {
                _overlord.SetVariableValue(Key.Value, Target.Value);
                outputObject = Target;
            }

			NextAll();
			return 0;
		}
	}
}
