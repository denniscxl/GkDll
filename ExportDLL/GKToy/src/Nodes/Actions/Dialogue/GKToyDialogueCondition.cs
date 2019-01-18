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
        }

        // 条件类型.
        [SerializeField]
        private GKToySharedInt _condPara = new GKToySharedInt();
        public GKToySharedInt CondPara
        {
            get { return _condPara; }
            set { _condPara = value; }
        }

        // 条件值.
        [SerializeField]
        private GKToySharedInt _condValue = new GKToySharedInt();
        public GKToySharedInt CondValue
        {
            get { return _condValue; }
            set { _condValue = value; }
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
