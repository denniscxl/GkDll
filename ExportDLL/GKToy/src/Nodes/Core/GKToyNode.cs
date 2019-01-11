using System.Collections.Generic;
using UnityEngine;
using GKBase;
using System.Linq;
using GKStateMachine;
using System.Reflection;
using System;
using Newtonsoft.Json;

namespace GKToy
{
    [System.Serializable]
    public class GKToyNode : GKStateMachineStateBase<int>
    {
        #region PublicField
        public int id;
        public Texture icon;
        public NodeType nodeType = NodeType.Node;
        public Vector2 pos;
        public int width;
        public int height;
        [ExportClient]
        [ExportServer]
        public string className;
        [ExportClient]
        [ExportServer]
        public string name;
        public string comment;
        public Rect rect;
        // 输入输出区域缓存.
        public Rect inputRect;
        public Rect outputRect;
        // 参数链接区域及属性缓存.
        public Dictionary<string, NodeParm> parmRect;
        public bool isMove;
        // 属性反射信息.
        public PropertyInfo[] props;
        // 当前属性是否使用全局变量.
        public int [] propStates;
        // 输入输出标记. 1 Input 2 Output 4 Need 8 OptionSwitch.
        public int[] ioStates;
        // 属性锁. 用来控制必要参数链接触发时机. 如果存在多个必要参数链接, 通过属性锁来协同触发时机.
        public bool[] propLock;
        [ExportClient]
        [ExportServer]
        public List<Link> links = new List<Link>();
        public List<Link> otherLinks = new List<Link>();
        public GKNodeStateMachine machine;
		public NodeState state;
        // 节点输出对象.
        public object outputObject = null;
        // 双击窗口类型.
        public int doubleClickType = 0;
        #endregion

        #region PrivateField
        protected GKToyBaseOverlord _overlord;
        protected bool _bLock = true;
        #endregion

        #region PublicMethod
        public GKToyNode(int _id):base(_id)
        {
        }

        virtual public void Init(GKToyBaseOverlord ovelord)
        {
            _overlord = ovelord;

            Type t = GetType();
            props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var fs = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if(null == propStates)
            {
                propStates = new int[props.Length];
                ioStates = new int[props.Length];
                propLock = new bool[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    propStates[i] = -1;
                    ioStates[i] = 0;
                    propLock[i] = false;
                }
            }
			state = NodeState.Inactive;

            parmRect = new Dictionary<string, NodeParm>();
            parmRect.Clear();
            for (int i = 0; i < props.Length; i++)
            {
                // 引用变量赋值.
                if(-1 != propStates[i])
                {
                    var v = props[i].PropertyType;
                    var vlst = _overlord.GetVariableListByType(v);
                    if(vlst.Count > propStates[i])
                    {
                        props[i].SetValue(this, ((GKToyVariable)vlst[propStates[i]]), null);
                    }
                }
                NodeParm np = new NodeParm();
                np.propretyInfo = props[i];
                np.rect = Rect.zero;
                // 参数节点区域初始化.
                parmRect.Add(props[i].Name, np);
            }
        }

        // 通过ID查找链接.
        public Link GetLinkByID(int id)
        {
            foreach (var l in links)
            {
                if (l.id == id)
                    return l;
            }
            return null;
        }

        public void UpdateLinkWithNode(GKToyNode node, int height)
        {
            Link l = FindLinkFromNode(node.id);
            if (null != l)
                UpdateLink(l, node, height);
            if ((int)node.nodeType >= 10)
            {
                l = FindLinkFromOtherNode(node.id);
                if (null != l)
                    UpdateLink(l, node, height);
            }

        }
        /// <summary>
        /// 根据组查找连出链接
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public Link FindLinkFromOtherNode(int nodeId)
        {
            var res = otherLinks.Where(x => x.next == nodeId).FirstOrDefault();
            if (!default(KeyValuePair<int, Link>).Equals(res))
            {
                return res;
            }
            return null;
        }

        /// <summary>
        /// 更新单根连线
        /// </summary>
        /// <param name="linkId">要更新连线的Id</param>
        public void UpdateLink(Link link, GKToyNode nextNode, int height)
        {
            bool vertical = false;
            Vector2 src = new Vector2(outputRect.x + outputRect.width, outputRect.y + outputRect.height * 0.5f);
            Vector2 dest = Vector2.zero;
            // 参数链接.
            if (!string.IsNullOrEmpty(link.parmKey) && nextNode.parmRect.ContainsKey(link.parmKey))
            {
                dest = new Vector2(nextNode.parmRect[link.parmKey].rect.x, nextNode.parmRect[link.parmKey].rect.y + nextNode.parmRect[link.parmKey].rect.height * 0.5f);
            }
            // 节点链接.
            else
            {
                dest = new Vector2(nextNode.inputRect.x, nextNode.inputRect.y + nextNode.inputRect.height * 0.5f);
            }
            link.points = new List<Vector2>(GK.ClacLinePoint(src, dest, out vertical, height));
            link.isFirstVertical = vertical;
        }

        /// <summary>
        /// 添加连线
        /// </summary>
        /// <param name="linkId">连线GUID</param>
        /// <param name="nextNode">连接到的节点</param>
        public void AddLink(int linkId, GKToyNode nextNode, int height, string parmKey = "")
        {
            bool vertical = false;
            Vector2 src = new Vector2(outputRect.x + outputRect.width, outputRect.y + outputRect.height * 0.5f);
            Vector2 dest = Vector2.zero;
            // 参数链接.
            if(!string.IsNullOrEmpty(parmKey) && nextNode.parmRect.ContainsKey(parmKey))
            {
                dest = new Vector2(nextNode.parmRect[parmKey].rect.x, nextNode.parmRect[parmKey].rect.y + nextNode.parmRect[parmKey].rect.height * 0.5f);
            }
            // 节点链接.
            else
            {
                dest = new Vector2(nextNode.inputRect.x, nextNode.inputRect.y + nextNode.inputRect.height * 0.5f);
            }
            if (10 <= (int)nextNode.nodeType)
                otherLinks.Add(new Link(linkId, GK.ClacLinePoint(src, dest, out vertical, height), vertical, id, nextNode.id, parmKey));
            else
                links.Add(new Link(linkId, GK.ClacLinePoint(src, dest, out vertical, height), vertical, id, nextNode.id, parmKey));
        }

        public void RemoveLink(GKToyNode removeNode)
        {
            if (10 > (int)removeNode.nodeType)
            {
                Link link = FindLinkFromNode(removeNode.id);
                if (null != link)
                    links.Remove(link);
            }
            else
            {
                Link link = FindLinkFromOtherNode(removeNode.id);
                if (null != link)
                    otherLinks.Remove(link);
            }
        }
        /// <summary>
        /// 检测是否被连接
        /// </summary>
        /// <param name="_selectNode">连线起始节点</param>
        /// <param name="mousePos">mouse up的位置</param>
        /// <returns>0:未被连接，1:被连接，但不添加连接，2:添加链接</returns>
        virtual public int CheckLink(GKToyNode _selectNode, Vector2 mousePos, ref string paramKey)
        {
            if (id == _selectNode.id)
                return 0;
            if (inputRect.Contains(mousePos) || rect.Contains(mousePos))
            {
                if (10 > (int)nodeType && null != _selectNode.FindLinkFromNode(id))
                    return 1;
                if (10 <= (int)nodeType && null != _selectNode.FindLinkFromOtherNode(id))
                    return 1;
                return 2;
            }
            else if(null != parmRect)
            {
                // 检测是否为节点参数区域.
                var pr = parmRect.Values.ToArray();
                for (int i = 0; i < ioStates.Length; i++)
                {
                    // 当前输入节点未开放.
                    if ((ioStates[i] & 1) != 1)
                        continue;

                    // 输出输入参数相等.
                    if (null != _selectNode.outputObject && pr[i].propretyInfo.PropertyType.Name == _selectNode.outputObject.GetType().Name)
                    {
                        if (pr[i].rect.Contains(mousePos))
                        {
                            paramKey = pr[i].propretyInfo.Name;
                            return 2;
                        }
                    }
                }
            }
            return 0;
        }

        virtual public void DoubleClick()
        {
        }

        /// <summary>
        /// 返回连接到某个节点的连接
        /// </summary>
        /// <param name="nodeId">被连接的节点id</param>
        /// <returns></returns>
        public Link FindLinkFromNode(int nodeId)
        {
            var res = links.Where(x => x.next == nodeId).FirstOrDefault();
            if (!default(KeyValuePair<int, Link>).Equals(res))
            {
                return res;
            }
            return null;
        }

        // 执行所有链接结果.
        public void NextAll()
        {
            List<GKStateListMachineBase<int>.NewStateStruct> lst = new List<GKStateListMachineBase<int>.NewStateStruct>();
            foreach (var l in links)
            {
                lst.Add(new GKStateListMachineBase<int>.NewStateStruct(l.next, l.parmKey, outputObject));
            }
            machine.GoToState(id, lst);
        }

        // 执行所有链接节点.
        public void NextAll(List<int> nexts)
        {
            List<GKStateListMachineBase<int>.NewStateStruct> lst = new List<GKStateListMachineBase<int>.NewStateStruct>();
            foreach (var n in nexts)
            {
                var link = FindLinkFromNode(n);
                if(null != link)
                    lst.Add(new GKStateListMachineBase<int>.NewStateStruct(link.next, link.parmKey, outputObject));
            }
            machine.GoToState(id, lst);
        }

        // 执行链接节点.
        public void Next(int idx)
        {
            if (0 == links.Count)
                return;

            List<GKStateListMachineBase<int>.NewStateStruct> lst = new List<GKStateListMachineBase<int>.NewStateStruct>();
            if(idx < links.Count)
            {
                lst.Add(new GKStateListMachineBase<int>.NewStateStruct(links[idx].next, links[idx].parmKey, outputObject)); 
            }
            else
                lst.Add(new GKStateListMachineBase<int>.NewStateStruct(links[0].next, links[0].parmKey, outputObject)); 
            machine.GoToState(id, lst);
        }

		public override void Enter()
		{
			state = NodeState.Activated;
			//Debug.Log("Enter " + name);
		}

        public override void Trigger(string parm, object val)
        {
            _bLock = true;
            if(string.IsNullOrEmpty(parm))
            {
                _bLock = false;
            }
            else
            {
                for (int i = 0; i < propLock.Length; i++)
                {
                    if(props[i].Name.Equals(parm))
                    {
                        // 设置参数值.
                        props[i].SetValue(this, val, null);
                        propLock[i] = false;
                        break;
                    }
                }

                // 检测当前参数锁是否全开. 如果存在未赋值必要参数, 跳出.
                for (int i = 0; i < propLock.Length; i++)
                {
                    if (propLock[i])
                        return;
                }
                _ResetPropLock();
                _bLock = false;
            }
        }

		public override int Update()
		{
			return 1;
		}

		public override void Exit()
		{
			if(state == NodeState.Activated)
				state = NodeState.Success;
			//Debug.Log("Exit " + name);
		}

        public override void ResetAfterState()
        {
            state = NodeState.Inactive;
            List<int> lst = new List<int>();
            foreach (var l in links)
            {
                lst.Add(l.next);
            }
            machine.ResetState(id, lst);
        }
        #endregion

        #region PrivateMethod
        // 重置参数锁数据.
        protected void _ResetPropLock()
        {
            for (int i = 0; i < props.Length; i++)
            {
                if(((ioStates[i] & 4) == 4))
                {
                    propLock[i] = true;
                }
            }
        }
		#endregion
	}

	/// <summary>
	/// 连线类
	/// </summary>
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Link
	{
        public int id;
        public Color color = Color.black;
		public bool isFirstVertical;
		public List<Vector2> points;
        public int previous;
        [JsonProperty]
        public int next;
        /// <summary>
        /// 链接目标可为节点也可为节点参数, 如果key不为空为该节点参数.
        /// </summary>
        public string parmKey = String.Empty;
        // 链接形态类型.
        public LinkType type = LinkType.RightAngle;
        // 是否展开Option.
        public bool bOption = false;

		public Link(int _id, List<Vector2> _points, bool _isFirstVertical, int _previous, int _next, string _parmKey = "")
		{
			id = _id;
			points = new List<Vector2>(_points);
			isFirstVertical = _isFirstVertical;
            previous = _previous;
			next = _next;
            parmKey = _parmKey;
		}
	}

    // 节点参数结构.
    public class NodeParm
    {
        public Rect rect;
        public PropertyInfo propretyInfo;
    }

	public enum NodeState
	{
		Inactive = 0,
		Activated,
		Success,
		Fail
	}

    public enum LinkType
    {
        RightAngle = 0,
        StraightLine
    }
}
