using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/对话树/离开对话")]
	[NodeTypeTree("Action/DialogueTree/dialogueExit", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("离开节点.")]
	[NodeDescription("Node of dialogue exit.", "English")]
	public class GKToyDialogueExit : GKToyNode
    {

        // 按钮内容.
        [SerializeField]
        [ExportClient]
        private GKToySharedString _menuText = new GKToySharedString();
        public GKToySharedString MenuText
        {
            get { return _menuText; }
            set { _menuText = value; }
        }
        
        public GKToyDialogueExit(int _id) : base(_id)
        {
            type = 4;
            doubleClickType = 5;
        }
        
        override public void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
        }

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();

            NextAll();
			return 0;
		}
    }
}
