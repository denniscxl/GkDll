using UnityEditor;
using UnityEngine;
using GKToy;

namespace GKToyTaskEditor
{
    public class GKToySubTask : GKToyNode
    {
        public int initTargetId;
        public int mainTask;
        public int subId;
        // 字面id，显示在界面上.
        public override int LiteralId
        {
            get { return _targetId.Value; }
        }
        public GKToySubTask(int _id) : base(_id){}

        // 子任务编号.
        [SerializeField]
        private GKToySharedInt _targetId = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt TargetID
        {
            get { return _targetId; }
            set { _targetId = value; }
        }

        // 子任务类型.
        [SerializeField]
        private GKToySharedInt _targetType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt TargetType
        {
            get { return _targetType; }
            set { _targetType = value; }
        }

        // 进行场景.
        [SerializeField]
        private GKToySharedString _scene = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }

        // 追踪信息.
        [SerializeField]
        private GKToySharedString _targetInfo = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TargetInfo
        {
            get { return _targetInfo; }
            set { _targetInfo = value; }
        }

        // 追踪文字.
        [SerializeField]
        private GKToySharedString _targetText = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString TargetText
        {
            get { return _targetText; }
            set { _targetText = value; }
        }

        virtual public void ChangeTaskID(int id)
        {
            TargetID.SetValue(id);
        }
    }
}
