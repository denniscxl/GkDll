using UnityEditor;
using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/对话树/条件")]
	[NodeTypeTree("Action/DialogueTree/condition", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("条件节点.")]
	[NodeDescription("Node of condition.", "English")]
	public class GKToyDialogueCondition : GKToyNode
    {
        public GKToyDialogueCondition(int _id) : base(_id)
        {
            // Dialogue condition.
            doubleClickType = 4;
            type = 3;
        }

        // 条件类型.
        [SerializeField]
        private GKToySharedInt _condPara = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt CondPara
        {
            get { return _condPara; }
            set { _condPara = value; }
        }

        // 条件值.
        [SerializeField]
        private GKToySharedInt _condValue = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt CondValue
        {
            get { return _condValue; }
            set { _condValue = value; }
        }

        // Yes节点链接ID.
        [SerializeField]
        private GKToySharedInt _ifYesNode = new GKToySharedInt();
		[ExportClient]
        [ExportServer]
        public GKToySharedInt IfYesNode
        {
            get { return _ifYesNode; }
            set { _ifYesNode = value; }
        }

        // No节点链接ID.
        [SerializeField]
        private GKToySharedInt _ifNoNode = new GKToySharedInt();
		[ExportClient]
        [ExportServer]
        public GKToySharedInt IfNoNode
        {
            get { return _ifNoNode; }
            set { _ifNoNode = value; }
        }

        // 输出类型.
        [SerializeField]
        private GKToySharedInt _outPutType = new GKToySharedInt();
		[ExportClient]
        [ExportServer]
        public GKToySharedInt OutPutType
        {
            get { return _outPutType; }
            set { _outPutType = value; }
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

        override public void LinkUpdate()
        {
            IfYesNode = 0;
            IfNoNode = 0;
            for (int i = 0; i < links.Count; i++)
            {
                if (0 == i)
                    IfYesNode.Value = links[i].next;
                if (1 == i)
                    IfNoNode.Value = links[i].next;
            }
        }
    }
}
