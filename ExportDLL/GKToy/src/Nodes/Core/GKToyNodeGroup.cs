using GKBase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GKToy
{
    /// <summary>
    /// 节点组.
    /// </summary>
	[System.Serializable]
	public class GKToyNodeGroup : GKToyNode
    {
        #region PublicField
        // 组内节点.
        public List<int> subNodes = new List<int>();
        // 外部链接虚拟节点.
        public List<int> groupLinkNodes = new List<int>();
        [System.NonSerialized]
        public GKToyData data;
        #endregion

        #region PrivateField
        // 组内节点的运行状态.
        List<NodeState> _subStates;
        #endregion

        #region PublicMethod
        public GKToyNodeGroup(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            _overlord = ovelord;
        }
        /// <summary>
        /// 添加虚拟节点
        /// </summary>
        /// <param name="_id">虚拟节点id</param>
        /// <param name="_sourceNodeId">源节点Id</param>
        /// <param name="isIn">是否链入节点</param>
        /// <param name="_otherGroupId">源节点的组节点Id</param>
        /// <returns></returns>
        public GKToyGroupLink AddGroupLink(int _id, int _sourceNodeId, bool isIn, int _otherGroupId)
        {
            GKToyGroupLink newGroupLink = new GKToyGroupLink(_id);
            newGroupLink.id = _id;
            newGroupLink.nodeType = NodeType.VirtualNode;
            newGroupLink.className = "GKToy.GKToyGroupLink";
            newGroupLink.sourceNodeId = _sourceNodeId;
            newGroupLink.groupId = id;
            newGroupLink.otherGroupId = _otherGroupId;
            newGroupLink.linkNodeIds = new List<int>();
            if (isIn)
                newGroupLink.linkType = GroupLinkType.LinkIn;
            else
                newGroupLink.linkType = GroupLinkType.LinkOut;
            return newGroupLink;
        }
        /// <summary>
        /// 查找在传入节点组的入虚拟节点中没有的子节点
        /// </summary>
        /// <param name="otherGroup"></param>
        /// <returns></returns>
        public List<int> GetUnlinkedInNodes(GKToyNodeGroup otherGroup)
        {
            GKToyGroupLink groupLink;
            List<int> res = new List<int>();
            res.AddRange(subNodes);
            foreach (int groupLinkId in otherGroup.groupLinkNodes)
            {
                groupLink = (GKToyGroupLink)data.nodeLst[groupLinkId];
                if (GroupLinkType.LinkIn == groupLink.linkType && subNodes.Contains(groupLink.sourceNodeId))
                    res.Remove(groupLink.sourceNodeId);
            }
            return res;
        }
        /// <summary>
        /// 查找在传入节点组的出虚拟节点中没有的子节点
        /// </summary>
        /// <param name="otherGroup"></param>
        /// <returns></returns>
        public List<int> GetUnlinkedOutNodes(GKToyNodeGroup otherGroup)
        {
            GKToyGroupLink groupLink;
            List<int> res = new List<int>();
            res.AddRange(subNodes);
            foreach (int groupLinkId in otherGroup.groupLinkNodes)
            {
                groupLink = (GKToyGroupLink)data.nodeLst[groupLinkId];
                if (GroupLinkType.LinkOut == groupLink.linkType && subNodes.Contains(groupLink.sourceNodeId))
                    res.Remove(groupLink.sourceNodeId);
            }
            return res;
        }
        /// <summary>
        /// 根据连接到的节点查找虚拟节点
        /// </summary>
        /// <param name="nodeId"></param>
        public void FindGroupLinkFromNodes(List<int> nodeIds, out List<GKToyGroupLink> inLinkNodes, out List<GKToyGroupLink> outLinkNodes)
        {
            inLinkNodes = new List<GKToyGroupLink>();
            outLinkNodes = new List<GKToyGroupLink>();
            GKToyGroupLink groupLink;
            foreach (int linkId in groupLinkNodes)
            {
                groupLink = (GKToyGroupLink)data.nodeLst[linkId];
                if (!nodeIds.Contains(groupLink.sourceNodeId))
                    continue;
                if (GroupLinkType.LinkIn == groupLink.linkType)
                    inLinkNodes.Add(groupLink);
                else
                    outLinkNodes.Add(groupLink);
            }
        }
        /// <summary>
        /// 根据源节点查找连出虚拟节点
        /// </summary>
        /// <param name="nodeId">源节点Id</param>
        /// <returns></returns>
        public GKToyGroupLink FindVirtualOutLinkFromSource(int nodeId)
        {
            GKToyGroupLink node;
            foreach (int id in groupLinkNodes)
            {
                node = (GKToyGroupLink)data.nodeLst[id];
                if (GroupLinkType.LinkOut == node.linkType && nodeId == node.sourceNodeId)
                    return node;
            }
            return null;
        }
        /// <summary>
        /// 根据源节点查找连入虚拟节点
        /// </summary>
        /// <param name="nodeId">源节点Id</param>
        /// <returns></returns>
        public GKToyGroupLink FindVirtualInLinkFromSource(int nodeId)
        {
            GKToyGroupLink node;
            foreach (int id in groupLinkNodes)
            {
                node = (GKToyGroupLink)data.nodeLst[id];
                if (GroupLinkType.LinkIn == node.linkType && nodeId == node.sourceNodeId)
                    return node;
            }
            return null;
        }
        /// <summary>
        /// 获取所有链入虚拟节点
        /// </summary>
        /// <returns></returns>
        public List<GKToyGroupLink> GetAllInNodes()
        {
            List<GKToyGroupLink> inNodes = new List<GKToyGroupLink>();
            GKToyGroupLink curNode;
            foreach (int linkId in groupLinkNodes)
            {
                curNode = (GKToyGroupLink)data.nodeLst[linkId];
                if (GroupLinkType.LinkIn == curNode.linkType)
                    inNodes.Add(curNode);
            }
            return inNodes;
        }
        /// <summary>
        /// 获取所有链出虚拟节点
        /// </summary>
        /// <returns></returns>
        public List<GKToyGroupLink> GetAllOutNodes()
        {
            List<GKToyGroupLink> outNodes = new List<GKToyGroupLink>();
            GKToyGroupLink curNode;
            foreach (int linkId in groupLinkNodes)
            {
                curNode = (GKToyGroupLink)data.nodeLst[linkId];
                if (GroupLinkType.LinkOut == curNode.linkType)
                    outNodes.Add(curNode);
            }
            return outNodes;
        }

        override public void Enter()
        {
            _subStates = new List<NodeState>();
            foreach (int subNodeId in subNodes)
            {
                _subStates.Add(((GKToyNode)data.nodeLst[subNodeId]).state);
            }
        }

        override public int Update()
        {
            for (int i = 0; i < subNodes.Count; ++i)
            {
                if (_subStates[i] != ((GKToyNode)data.nodeLst[subNodes[i]]).state)
                {
                    _subStates[i] = ((GKToyNode)data.nodeLst[subNodes[i]]).state;
                    state = _subStates[i];
                }
            }
            return 0;
        }

        override public void Exit()
        {
            _subStates.Clear();
        }
        #endregion

        #region PrivateMethod
        #endregion
    }
}
