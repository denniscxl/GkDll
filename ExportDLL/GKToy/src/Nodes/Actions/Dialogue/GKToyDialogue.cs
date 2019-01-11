using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/对话树/对话")]
	[NodeTypeTree("Action/DialogueTree/dialogue", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("对话节点.")]
	[NodeDescription("Node of dialogue tree.", "English")]
	public class GKToyDialogue : GKToyNode
    {
        public GKToyDialogue(int _id) : base(_id)
        {
            // Dialogue.
            doubleClickType = 2;
        }

        // 起始对话索引链表.
        [SerializeField]
        private GKToySharedIntLst _idxLst = new GKToySharedIntLst();
        public GKToySharedIntLst _IDLst
        {
            get { return _idxLst; }
            set { _idxLst = value; }
        }

        // 起始对话内容链表.
        [SerializeField]
        private GKToySharedStringLst _contentLst = new GKToySharedStringLst();
        public GKToySharedStringLst ContentLst
        {
            get { return _contentLst; }
            set { _contentLst = value; }
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
