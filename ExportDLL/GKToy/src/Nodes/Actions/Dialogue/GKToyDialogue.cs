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

        // 起始对话索引.
        [SerializeField]
        private GKToySharedInt _idx = new GKToySharedInt();
        public GKToySharedInt ID
        {
            get { return _idx; }
            set { _idx = value; }
        }

        // 起始对话内容.
        [SerializeField]
        private GKToySharedString _content = new GKToySharedString();
        public GKToySharedString Content
        {
            get { return _content; }
            set { _content = value; }
        }

        // 按钮内容.
        [SerializeField]
        private GKToySharedString _menuText = new GKToySharedString();
        public GKToySharedString MenuText
        {
            get { return _menuText; }
            set { _menuText = value; }
        }

        // 摄像机类型.
        [SerializeField]
        private GKToySharedInt _camera = new GKToySharedInt();
        public GKToySharedInt Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        //摄像机值.
        [SerializeField]
        private GKToySharedString _cameraValue = new GKToySharedString();
        public GKToySharedString CameraValue
        {
            get { return _cameraValue; }
            set { _cameraValue = value; }
        }

        // 行为.
        [SerializeField]
        private GKToySharedInt _action = new GKToySharedInt();
        public GKToySharedInt Action
        {
            get { return _action; }
            set { _action = value; }
        }

        // 行为值.
        [SerializeField]
        private GKToySharedString _actionValue = new GKToySharedString();
        public GKToySharedString ActionValue
        {
            get { return _actionValue; }
            set { _actionValue = value; }
        }

        override public void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
        }

        // 声音类型.
        [SerializeField]
        private GKToySharedInt _sound = new GKToySharedInt();
        public GKToySharedInt Sound
        {
            get { return _sound; }
            set { _sound = value; }
        }

        //声音值.
        [SerializeField]
        private GKToySharedString _soundValue = new GKToySharedString();
        public GKToySharedString SoundValue
        {
            get { return _soundValue; }
            set { _soundValue = value; }
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
