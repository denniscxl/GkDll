using UnityEditor;
using UnityEngine;

namespace GKToy
{
	public class GKToyTask : GKToyNode
    {
        public GKToyTask(int _id) : base(_id)
        {
            // Task.
            doubleClickType = 1;
        }

        // 需求类型链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedIntLst _requestTypeLst = new GKToySharedIntLst();
        public GKToySharedIntLst RequestTypeLst
        {
            get { return _requestTypeLst; }
            set { _requestTypeLst = value; }
        }

        // 需求数量链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedIntLst _requestCountLst = new GKToySharedIntLst();
        public GKToySharedIntLst RequestCountLst
        {
            get { return _requestCountLst; }
            set { _requestCountLst = value; }
        }

        // 起始对话索引链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedIntLst _beginDialogueIdxLst = new GKToySharedIntLst();
        public GKToySharedIntLst BeginDialogueIdxLst
        {
            get { return _beginDialogueIdxLst; }
            set { _beginDialogueIdxLst = value; }
        }

        // 起始对话内容链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedStringLst _beginDialogueContentLst = new GKToySharedStringLst();
        public GKToySharedStringLst BeginDialogueContentLst
        {
            get { return _beginDialogueContentLst; }
            set { _beginDialogueContentLst = value; }
        }
        
        // 完结对话索引链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedIntLst _endDialogueIdxLst = new GKToySharedIntLst();
        public GKToySharedIntLst EndDialogueIdxLst
        {
            get { return _endDialogueIdxLst; }
            set { _endDialogueIdxLst = value; }
        }

        // 完结对话内容链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedStringLst _endDialogueContentLst = new GKToySharedStringLst();
        public GKToySharedStringLst EndDialogueContentLst
        {
            get { return _endDialogueContentLst; }
            set { _endDialogueContentLst = value; }
        }

        // 奖励类型链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedIntLst _rewardTypeLst = new GKToySharedIntLst();
        public GKToySharedIntLst RewardTypeLst
        {
            get { return _rewardTypeLst; }
            set { _rewardTypeLst = value; }
        }

        // 奖励数量链表.
        [SerializeField]
        [ExportClient]
        private GKToySharedIntLst _rewardCountLst = new GKToySharedIntLst();
        public GKToySharedIntLst RewardCountLst
        {
            get { return _rewardCountLst; }
            set { _rewardCountLst = value; }
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
