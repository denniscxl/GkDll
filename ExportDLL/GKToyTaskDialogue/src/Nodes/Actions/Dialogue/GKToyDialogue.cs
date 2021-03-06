﻿using UnityEditor;
using UnityEngine;
using GKToy;

namespace GKToyTaskDialogue
{
    [NodeTypeTree("行为/任务对话/对话")]
	[NodeTypeTree("Action/TaskDialogue/dialogue", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("任务对话节点.")]
	[NodeDescription("Node of task dialogue.", "English")]
	public class GKToyDialogue : GKToyNode
    {
        public GKToyDialogue(int _id) : base(_id)
        {
            // Dialogue.
            doubleClickType = 2;
            type = 1;
        }

        // 字面id，显示在界面上.
        public override int LiteralId
        {
            get { return _nodeID.Value; }
        }

        // 对话节点ID.
        [SerializeField]
        private GKToySharedInt _nodeID = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt NodeID
        {
            get { return _nodeID; }
            set { _nodeID = value; }
        }

        // 是否开始节点.
        [SerializeField]
        private GKToySharedInt _isStartNode = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt IsStartNode
        {
            get { return _isStartNode; }
            set { _isStartNode = value; }
        }

        // 起始对话说话人ID.
        [SerializeField]
        private GKToySharedString _entity = new GKToySharedString();
        [ExportClient]
        public GKToySharedString Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }
        // 起始对话说话人名称.
        [SerializeField]
        private GKToySharedString _speaker = new GKToySharedString();
        [ExportClient]
        public GKToySharedString Speaker
        {
            get { return _speaker; }
            set { _speaker = value; }
        }

        // 起始对话内容.
        [SerializeField]
        private GKToySharedString _speakText = new GKToySharedString();
        [ExportClient]
        public GKToySharedString SpeakText
        {
            get { return _speakText; }
            set { _speakText = value; }
        }

        // 按钮内容.
        [SerializeField]
        private GKToySharedString _menuText = new GKToySharedString();
        [ExportClient]
        public GKToySharedString MenuText
        {
            get { return _menuText; }
            set { _menuText = value; }
        }

        // 表情内容.
        [SerializeField]
        private GKToySharedString _speakText2 = new GKToySharedString();
        [ExportClient]
        public GKToySharedString SpeakText2
        {
            get { return _speakText2; }
            set { _speakText2 = value; }
        }

        // 摄像机类型.
        [SerializeField]
        private GKToySharedInt _cameraRes = new GKToySharedInt();
        [ExportClient]
        public GKToySharedInt CameraRes
        {
            get { return _cameraRes; }
            set { _cameraRes = value; }
        }

        //摄像机值.
        [SerializeField]
        private GKToySharedString _cameraValue = new GKToySharedString();
        [ExportClient]
        public GKToySharedString CameraValue
        {
            get { return _cameraValue; }
            set { _cameraValue = value; }
        }

        // 动画.
        [SerializeField]
        private GKToySharedString _animationRes = new GKToySharedString();
        [ExportClient]
        public GKToySharedString AnimationRes
        {
            get { return _animationRes; }
            set { _animationRes = value; }
        }

        // 声音类型.
        [SerializeField]
        private GKToySharedInt _soundRes = new GKToySharedInt();
        [ExportClient]
        public GKToySharedInt SoundRes
        {
            get { return _soundRes; }
            set { _soundRes = value; }
        }

        //声音值.
        [SerializeField]
        private GKToySharedString _soundValue = new GKToySharedString();
        [ExportClient]
        public GKToySharedString SoundValue
        {
            get { return _soundValue; }
            set { _soundValue = value; }
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
