using UnityEngine;
using System.Linq;
using GKBase;
using XLua;

namespace GKToy
{
	[NodeTypeTree("行为/Lua/Lua流程")]
	[NodeDescription("执行Lua脚本.")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Code.png")]
	[NodeTypeTree("Action/Lua/LuaProcess", "English")]
	[NodeDescription("Run Lua script.", "English")]
	public class GKToyLuaProcess : GKToyNode
    {
        [SerializeField]
        GKToySharedString _scriptName = string.Empty;
		public GKToySharedString ScriptName
		{
            get { return _scriptName; }
            set { _scriptName = value; }
		}

        [SerializeField]
        GKToySharedString _methodName = string.Empty;
        public GKToySharedString MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }

        GKToySharedBool _output = false;

        public GKToyLuaProcess(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = false;
            outputObject = _output;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();

            LuaEnv luaenv = new LuaEnv();
            var asset = GK.TryLoadResource<TextAsset>(ScriptName.Value);
            if (null == asset)
            {
                Debug.LogError("Can't load lua script.");
                state = NodeState.Fail;
                _output = false;
                outputObject = _output;
            }
            else
            {
                luaenv.DoString(asset.text);
                luaenv.DoString(MethodName.Value + "()");
                state = NodeState.Success;
                _output = true;
                outputObject = _output;
            }

            NextAll();
			return 0;
		}
	}
}
