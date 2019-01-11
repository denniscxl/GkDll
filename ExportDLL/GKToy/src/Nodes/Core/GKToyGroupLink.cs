using GKBase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GKToy
{
    /// <summary>
    /// 在节点组内表示外部链接的虚拟节点
    /// </summary>
    [System.Serializable]
    public class GKToyGroupLink : GKToyNode
    {
        // 源节点的节点Id.
        public int sourceNodeId;
        // 虚拟节点类型：从外部链入&链出到外部.
        public GroupLinkType linkType;
        // 所在组节点id.
        public int groupId;
        // 所连到组节点id.
        public int otherGroupId;
        // 所连到的节点
        public List<int> linkNodeIds;

        public GKToyGroupLink(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            _overlord = ovelord;
            state = NodeState.Inactive;
        }

        override public int CheckLink(GKToyNode _selectNode, Vector2 mousePos, ref string paramKey)
        {
            // 禁止连接到虚拟入节点、禁止虚拟节点相连.
            if (GroupLinkType.LinkIn == linkType || NodeType.VirtualNode == _selectNode.nodeType)
                return 0;
            return base.CheckLink(_selectNode, mousePos, ref paramKey);
        }
    }

    public enum GroupLinkType
    {
        LinkIn = 0,
        LinkOut
    }
}
