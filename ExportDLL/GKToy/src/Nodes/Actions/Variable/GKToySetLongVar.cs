using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变量/设置长整型变量")]
    [NodeTypeTree("Action/Variable/Set long var", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Variable.png")]
    [NodeDescription("设置长整型变量.")]
    [NodeDescription("Set long variable.", "English")]
	public class GKToySetLongVar : GKToyNode
	{
		[SerializeField]
        GKToySharedString _key = "";
		public GKToySharedString Key
        {
            get { return _key; }
			set { _key = value;}
        }
        [SerializeField]
        GKToySharedLong _target = 0;
        public GKToySharedLong Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public GKToySetLongVar(int _id) : base(_id) { }
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
