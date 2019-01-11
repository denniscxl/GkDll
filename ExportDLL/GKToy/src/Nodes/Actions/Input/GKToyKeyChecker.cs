using UnityEngine;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("行为/输入/按键判断")]
	[NodeDescription("判断某个按键是否处于被按下的状态：\n成功 触发链接路径. ")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Click.png")]
	[NodeTypeTree("Action/Input/KeyChecker", "English")]
	[NodeDescription("Check whether a key is pressing.\nIf true, go to next.", "English")]
	public class GKToyKeyChecker : GKToyNode
    {
		[SerializeField]
		KeyCode _key = 0;
		public KeyCode Key
		{
            get { return _key; }
            set { _key = value; }
		}

        bool _isSuccess = false;

        public GKToyKeyChecker(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = KeyCode.A;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            if (Input.GetKey(Key))
			{
                _isSuccess = true;
                outputObject = Key;
                NextAll();
			}
			return 0;
		}

        override public void Exit()
		{
            if (_isSuccess)
				state = NodeState.Success;
			else
				state = NodeState.Fail;
			base.Exit();
		}
	}
}
