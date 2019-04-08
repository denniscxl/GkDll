using UnityEditor;
using UnityEngine;
using GKToy;

namespace GKToyTaskEditor
{
    [NodeTypeTree("辅助/任务/交互")]
	[NodeTypeTree("Decoration/Task/Interact", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("交互任务节点.")]
	[NodeDescription("Node of interact task.", "English")]
	public class GKToySubTaskInteract : GKToySubTask
    {

        public GKToySubTaskInteract(int _id) : base(_id)
        {
            doubleClickType = 1;
            TargetType = 1;
            TargetInfo = "与 [InteractNpc] 交谈";
            TargetText = "与 [InteractNpc] 交谈";
        }

        // 交互npc.
        [SerializeField]
        private GKToySharedString _interactNpc = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString InteractNpc
        {
            get { return _interactNpc; }
            set { _interactNpc = value; }
        }

        // 交互类型.
        [SerializeField]
        private GKToySharedInt _interactType = new GKToySharedInt();
        [ExportClient]
        [ExportServer]
        public GKToySharedInt InteractType
        {
            get { return _interactType; }
            set { _interactType = value; }
        }

        // 交互对话.
        [SerializeField]
        private GKToySharedGameObject _interactDfgObject = new GKToySharedGameObject();
        [ExportClient]
        [ExportServer]
        public GKToySharedGameObject InteractDfgObject
        {
            get { return _interactDfgObject; }
            set { _interactDfgObject = value; }
        }
        // 交互对话ID.
        [ExportClient]
        [ExportServer]
        public GKToySharedString InteractDfg
        {
            get { return _interactDfgObject.Value.GetComponent<GKToyBaseOverlord>().internalData.name; }
        }

        // 交互时间.
        [SerializeField]
        private GKToySharedString _interactTime = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString InteractTime
        {
            get { return _interactTime; }
            set { _interactTime = value; }
        }

        // 交互道具.
        [SerializeField]
        private GKToySharedString _interactItem = new GKToySharedString();
        [ExportClient]
        [ExportServer]
        public GKToySharedString InteractItem
        {
            get { return _interactItem; }
            set { _interactItem = value; }
        }

        public override void ChangeTaskID(int id)
        {
            base.ChangeTaskID(id);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(InteractDfgObject.Value), string.Format("Interact_{0}.prefab", id));
            GKToyBaseOverlord interactOverlord = InteractDfgObject.Value.GetComponent<GKToyBaseOverlord>();
            interactOverlord.internalData.name = string.Format("Interact_{0}", id);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(interactOverlord.internalData), string.Format("Interact_{0}.Asset", id));
            AssetDatabase.Refresh();
        }
    }
}
