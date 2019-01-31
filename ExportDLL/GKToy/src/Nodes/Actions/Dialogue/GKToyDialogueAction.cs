using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/对话树/行为")]
	[NodeTypeTree("Action/DialogueTree/action", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("行为节点.")]
	[NodeDescription("Node of dialogue action.", "English")]
	public class GKToyDialogueAction : GKToyNode
    {
        public GKToyDialogueAction(int _id) : base(_id)
        {
            // Dialogue action.
            doubleClickType = 6;
            type = 4;
        }

        // 行为.
        [SerializeField]
        private GKToySharedInt _action = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt Action
        {
            get { return _action; }
            set { _action = value; }
        }

        // 行为值.
        [SerializeField]
        private GKToySharedString _actionValue = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString ActionValue
        {
            get { return _actionValue; }
            set { _actionValue = value; }
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
