using UnityEditor;
using UnityEngine;
using GKToy;
using System.Collections.Generic;

namespace GKToyTaskEditor
{
    [NodeTypeTree("行为/任务/主任务")]
    [NodeTypeTree("Action/Task/Main Task", "English")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
    [NodeDescription("主任务节点.")]
    [NodeDescription("Main node.", "English")]
    public class GKToyTask : GKToyNode
    {
        public int initTaskId;
        public List<int> subTasks = new List<int>();
        public List<int> preTaskIds = new List<int>();
        public List<int> nextTaskIds = new List<int>();
        public List<string> preSeperator = new List<string>();

        // 字面id，显示在界面上.
        public override int LiteralId
        {
            get { return _taskId.Value; }
        }
        public GKToyTask(int _id) : base(_id)
        {
            // Task.
            doubleClickType = 0;
            IfTell = 1;
            TaskType = 1;
            FailedType = 1;
        }

        // 任务ID.
        [SerializeField]
        private GKToySharedInt _taskId = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt TaskID
        {
            get { return _taskId; }
            set { _taskId = value; }
        }

        // 任务名.
        [SerializeField]
        private GKToySharedString _taskName = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        // 任务目标.
        [SerializeField]
        private GKToySharedString _taskTarget = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TaskTarget
        {
            get { return _taskTarget; }
            set { _taskTarget = value; }
        }

        // 任务描述.
        [SerializeField]
        private GKToySharedString _taskDesc = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TaskDesc
        {
            get { return _taskDesc; }
            set { _taskDesc = value; }
        }

        // 任务可提交文字.
        [SerializeField]
        private GKToySharedString _taskCanSubmit = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TaskCanSubmit
        {
            get { return _taskCanSubmit; }
            set { _taskCanSubmit = value; }
        }

        // 接/交是否有systeminfo.
        [SerializeField]
        private GKToySharedInt _ifTell = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt IfTell
        {
            get { return _ifTell; }
            set { _ifTell = value; }
        }

        // 任务类型标识.
        [SerializeField]
        private GKToySharedInt _tasktype = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt TaskType
        {
            get { return _tasktype; }
            set { _tasktype = value; }
        }

        // 重置类型（day/week/month）.
        [SerializeField]
        private GKToySharedInt _resetType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt ResetType
        {
            get { return _resetType; }
            set { _resetType = value; }
        }

        // 能否放弃.
        [SerializeField]
        private GKToySharedInt _ifGiveUp = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt IfGiveUp
        {
            get { return _ifGiveUp; }
            set { _ifGiveUp = value; }
        }

        // 任务脚本.
        [SerializeField]
        private GKToySharedString _taskLua = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TaskLua
        {
            get { return _taskLua; }
            set { _taskLua = value; }
        }

        // 接任务npc.
        [SerializeField]
        private GKToySharedString _acceptNpc = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString AcceptNpc
        {
            get { return _acceptNpc; }
            set { _acceptNpc = value; }
        }

        // 接任务npc场景.
        [SerializeField]
        private GKToySharedString _acceptScene = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString AcceptScene
        {
            get { return _acceptScene; }
            set { _acceptScene = value; }
        }

        // 接任务等级.
        [SerializeField]
        private GKToySharedInt _acceptLv = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt AcceptLv
        {
            get { return _acceptLv; }
            set { _acceptLv = value; }
        }

        // 接任务声望.
        [SerializeField]
        private GKToySharedInt _acceptReput = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt AcceptReput
        {
            get { return _acceptReput; }
            set { _acceptReput = value; }
        }

        // 接取类型.
        [SerializeField]
        private GKToySharedInt _acceptType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt AcceptType
        {
            get { return _acceptType; }
            set { _acceptType = value; }
        }

        // 任务脚本.
        [SerializeField]
        private GKToySharedString _acceptMovie = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString AcceptMovie
        {
            get { return _acceptMovie; }
            set { _acceptMovie = value; }
        }

        // 接任务对话.
        [SerializeField]
        private GKToySharedGameObject _acceptDfgObject = new GKToySharedGameObject();
        [ExportClient]
        [ExportServer]
        public GKToySharedGameObject AcceptDfgObject
        {
            get { return _acceptDfgObject; }
            set { _acceptDfgObject = value; }
        }
        // 接任务对话ID.
        [ExportClient]
        [ExportServer]
        public GKToySharedString AcceptDfg
        {
            get { return _acceptDfgObject.Value.GetComponent<GKToyBaseOverlord>().internalData.name; }
        }

        // 接任务npc.
        [SerializeField]
        private GKToySharedString _submitNpc = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString SubmitNpc
        {
            get { return _submitNpc; }
            set { _submitNpc = value; }
        }

        // 接任务npc场景.
        [SerializeField]
        private GKToySharedString _submitScene = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString SubmitScene
        {
            get { return _submitScene; }
            set { _submitScene = value; }
        }

        // 接任务等级.
        [SerializeField]
        private GKToySharedInt _submitLv = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt SubmitLv
        {
            get { return _submitLv; }
            set { _submitLv = value; }
        }

        // 接任务声望.
        [SerializeField]
        private GKToySharedInt _submitReput = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt SubmitReput
        {
            get { return _submitReput; }
            set { _submitReput = value; }
        }

        // 交任务类型.
        [SerializeField]
        private GKToySharedInt _submitType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt SubmitType
        {
            get { return _submitType; }
            set { _submitType = value; }
        }

        // 失败处理.
        [SerializeField]
        private GKToySharedInt _failedType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt FailedType
        {
            get { return _failedType; }
            set { _failedType = value; }
        }

        // 奖励id.
        [SerializeField]
        private GKToySharedString _prize = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString Prize
        {
            get { return _prize; }
            set { _prize = value; }
        }

        // 交任务对话.
        [SerializeField]
        private GKToySharedGameObject _submitDfgObject = new GKToySharedGameObject();
        [ExportClient]
        [ExportServer]
        public GKToySharedGameObject SubmitDfgObject
        {
            get { return _submitDfgObject; }
            set { _submitDfgObject = value; }
        }
        // 交任务对话ID.
        [ExportClient]
        [ExportServer]
        public GKToySharedString SubmitDfg
        {
            get { return _submitDfgObject.Value.GetComponent<GKToyBaseOverlord>().internalData.name; }
        }

        // 交任务动画.
        [SerializeField]
        private GKToySharedString _submitMovie = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString SubmitMovie
        {
            get { return _submitMovie; }
            set { _submitMovie = value; }
        }

        // 前置任务.
        [SerializeField]
        private GKToySharedString _preTask = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString PreTask
        {
            get { return _preTask; }
            set { _preTask = value; }
        }

        // 后续任务.
        [SerializeField]
        private GKToySharedString _nextTask = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString NextTask
        {
            get { return _nextTask; }
            set { _nextTask = value; }
        }

        public void ChangeTaskID(int id)
        {
            TaskID.SetValue(id);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(AcceptDfgObject.Value), string.Format("AcceptDfg_{0}.prefab", id));
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(SubmitDfgObject.Value), string.Format("SubmitDfg_{0}.prefab", id));
            GKToyBaseOverlord acceptOverlord = SubmitDfgObject.Value.GetComponent<GKToyBaseOverlord>();
            GKToyBaseOverlord submitOverlord = AcceptDfgObject.Value.GetComponent<GKToyBaseOverlord>();
            acceptOverlord.internalData.name = string.Format("AcceptDfg_{0}", id);
            submitOverlord.internalData.name = string.Format("SubmitDfg_{0}", id);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(acceptOverlord.internalData), string.Format("AcceptDfg_{0}.Asset", id));
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(submitOverlord.internalData), string.Format("SubmitDfg_{0}.Asset", id));
            AssetDatabase.Refresh();
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
