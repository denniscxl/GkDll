using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GKBase;
using System;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Xml;

namespace GKToy
{
    public class GKToyMaker : EditorWindow
    {
        #region PublicField
        protected static GKToyMaker _instance;
        public static GKToyMaker Instance
        {
            get
            {
                if (null == _instance)
                    _instance = GetWindow<GKToyMaker>(_GetLocalization("Neuron"), false);
                return _instance;
            }
        }
        protected ToyMakerSettings _settings;
        public ToyMakerSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GK.LoadResource<ToyMakerSettings>("Settings/ToyMakerSettings");
                }
                return _settings;
            }
        }
        static public GKToyLocalizationData Data
        {
            get
            {
                if (null == _data)
                {
                    _data = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyLocalizationData.asset") as GKToyLocalizationData;
                    if (null == _data)
                        Debug.LogError("Load maker data fail.");
                }
                return _data;
            }
        }
        // 当前渲染数据源.
        public GKToyData CurRenderData
        {
            get
            {
                if (0 != CurDataIdx && _overlord.externalDatas.Count > CurDataIdx - 1)
                    return _overlord.externalDatas[CurDataIdx - 1].data;
                return _overlord.internalData.data;
            }
        }
        public virtual ToyMakerSettings.ToyMakerBase ToyMakerBase
        {
            get
            {
                return Settings.toyMakerBase;
            }
        }
        #endregion

        #region PrivateField
        protected float Scale
        {
            get { return _contentSacle; }
            set
            {
                if (ToyMakerBase._minScale > value)
                {
                    _contentSacle = ToyMakerBase._minScale;
                    return;
                }
                if (ToyMakerBase._maxScale < value)
                {
                    _contentSacle = ToyMakerBase._maxScale;
                    return;
                }
                _contentSacle = value;
            }
        }

        // 当前Node索引. 用于产生GUID.
        protected int CurNodeIdx
        {
            get { return CurRenderData.nodeGuid; }
            set
            {
                if (null != _overlord)
                {
                    CurRenderData.nodeGuid = value;
                }
            }
        }

        // 当前Link索引，用于产生Link的GUID.
        protected int CurLinkIdx
        {
            get { return CurRenderData.linkGuid; }
            set
            {
                if (null != _overlord)
                {
                    CurRenderData.linkGuid = value;
                }
            }
        }

        // 名称.
        protected string Name
        {
            get { return CurRenderData.name; }
            set
            {
                if (CurRenderData.name != value)
                {
                    CurRenderData.name = value;
                    _UpdateDataNames();
                }

            }
        }

        List<GKToyNode> _searchList = new List<GKToyNode>();
        // 搜索内容.
        protected string _searchContent = string.Empty;
        protected string SearchContent
        {
            get { return _searchContent; }
            set
            {
                if (_searchContent != value)
                {
                    _searchContent = value;
                    _UpdateSearchValue();
                }
            }
        }
        // 节点类型搜索内容.
        Dictionary<string, string> _typeSearchList = new Dictionary<string, string>();
        protected string _typeSearchContent = string.Empty;
        protected string TypeSearchContent
        {
            get { return _typeSearchContent; }
            set
            {
                if (_typeSearchContent != value)
                {
                    _typeSearchContent = value;
                    _UpdateTypeSearchValue();
                }
            }
        }

        private int _curDataIdx = 0;
        private int CurDataIdx
        {
            get
            {
                return _curDataIdx;
            }
            set
            {
                if (_curDataIdx != value)
                {
                    _selectNodes.Clear();
                    _selectLink = null;
                    _curDataIdx = value;
                }
            }
        }

        // 语言类型.
        static protected LanguageType _language = LanguageType.Chinese;

        static GKToyLocalizationData _data;
        // 节点种类树根节点.
        static TreeNode root = null;

        protected GKToyBaseOverlord _overlord;
        protected List<Type> _variableType = new List<Type>();
        protected string[] _variableTypeNames;
        // 节点内容缩放因子.
        protected float _contentSacle = 1;
        // 当前展开的节点组.
        protected GKToyNodeGroup _curGroup = null;
        protected string[] _virtualNodeNames;
        protected List<GKToyNode> _virtualNodeSources = new List<GKToyNode>();
        // 事件内容滚动条位置.
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        // 鼠标是否拖拽中.
        protected bool _isDrag = false;
        protected bool _isLinking = false;
        protected bool _isScale = false;
        protected bool _isAreaSelecting = false;
        // 当前信息界面类型.
        protected InformationType _infoType = InformationType.Detail;
        protected List<GKToyNode> _selectNodes = new List<GKToyNode>();
        protected string _nodeDes = String.Empty;
        // 临时节点缓存.
        protected List<GKToyNode> _tmpSelectNodes = new List<GKToyNode>();
        // 当前选中Link的Id.
        protected Link _selectLink = null;
        // 点击到的元素.
        protected ClickedElement _clickedElement = ClickedElement.NodeElement;
        // 视口区域.
        protected Rect _contentView;
        protected Rect _contentRect = new Rect();
        protected Rect _nonContentRect;
        protected Rect _responseRect;
        // 缩放视口偏移缓存.
        protected Vector2 _tmpScalePos = Vector2.zero;
        // 鼠标距节点中心的偏移量.
        protected Dictionary<GKToyNode, Vector2> _mouseOffset = new Dictionary<GKToyNode, Vector2>();
        protected Vector2 _mouseStart = Vector2.zero;
        // !!!逻辑渲染分离, 等渲染完毕后再进行逻辑处理, 规避渲染时变更渲染内容所产生的异常.
        // 链接变更列表.
        protected Dictionary<GKToyNode, List<NewNode>> _newLinkLst = new Dictionary<GKToyNode, List<NewNode>>();
        protected Dictionary<GKToyNode, List<GKToyNode>> _removeLinkLst = new Dictionary<GKToyNode, List<GKToyNode>>();
        // 节点变更列表.
        protected Dictionary<int, object> _newNodeLst = new Dictionary<int, object>();
        protected List<int> _removeNodeLst = new List<int>();
        protected Dictionary<int, GKToyGroupLink> _newGroupLinkLst = new Dictionary<int, GKToyGroupLink>();
        protected List<int> _removeGroupLinkLst = new List<int>();
        protected List<GKToyNode> _drawingNodes = new List<GKToyNode>();
        protected List<GKToyNode> _tmpAddDrawingNodes = new List<GKToyNode>();
        // GUI 数据备份.
        protected Color _lastColor;
        protected Color _lastBgColor;
        // 临时变量编辑缓存.
        protected string _newVariableName = "";
        protected int _newVariableIdx = 0;
        protected Dictionary<string, List<object>> _addVariableLst = new Dictionary<string, List<object>>();
        protected Dictionary<string, List<object>> _delVariableLst = new Dictionary<string, List<object>>();
        // 是否锁定中.
        protected bool _isLocking = false;
        // 当前数据源索引及名称.
        List<string> _dataNameList = new List<string>();
        int _curVirtualNodeIdx = 0;
        GKEditorScreenshot _screenshot;
        // 链接是否需要刷新.
        int _linkReCal = 0;

        #endregion

        #region PublicMethod
        //[MenuItem("GK/Toy Maker", false, GKEditorConfiger.MenuItemPriorityA)]
        //public static void MenuItemWindow()
        //{
        //    instance = GetWindow<GKToyMaker>(_GetLocalization("Neuron"), false);

        //}
        #endregion

        #region Base
        void OnEnable()
        {
            if (null == _instance)
            {
                _instance = GetWindow<GKToyMaker>(_GetLocalization("Neuron"), true);
                _Init();
                wantsMouseMove = true;
                minSize = new Vector2(ToyMakerBase._minWidth, ToyMakerBase._minHeight);
                maxSize = new Vector2(ToyMakerBase._minWidth, ToyMakerBase._minHeight);
            }
        }

        void OnGUI()
        {
            _EventProcess();
            _Render();
        }

        void Update()
        {
            _CheckScreenshot();
            _SelectChanged();

            if (null == _overlord)
                return;

            _Changed();
            _UpdateLinks();
        }

        void OnDestroy()
        {
            if (null != _overlord)
            {
                if (EditorUtility.DisplayDialog(_GetLocalization("Save node"),
                    _GetLocalization("Save data before proceeding?"), _GetLocalization("OK"), _GetLocalization("Cancel")))
                {
                    _ResetScaleData();
                    _overlord.Save();
                    _overlord.Backup();
                }
            }
            _instance = null;
        }

        void _Render()
        {
            if (null == _overlord || null == _overlord.internalData)
            {
                GUILayout.BeginArea(_nonContentRect);
                {
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.Label(_GetLocalization("Create a new task or select a record"));
                        if (GUILayout.Button(_GetLocalization("Create"), GUILayout.Width(200), GUILayout.Height(30)))
                        {
                            var destPath = EditorUtility.SaveFilePanelInProject("Save path", "", "Asset", "Select save path.");
                            if (!string.IsNullOrEmpty(destPath))
                            {
                                ToyMakerBase._defaultOverlordPath = Path.GetDirectoryName(destPath);
                                _CreateData(destPath);
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndArea();
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    _DrawInformation();
                    GUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));
                    {
                        _DrawToolBar();
                        _DrawContent();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            if (0 != _tmpSelectNodes.Count)
            {
                GUI.FocusControl(null);
            }
        }
        #endregion

        #region Event
        // 按键响应.
        void _EventProcess()
        {
            if (null == Event.current || null == _overlord)
                return;

            // 缓存内容坐标. 防止缩放时移动.
            _isScale = Event.current.alt;
            if (_isScale)
                _tmpScalePos = _contentScrollPos;

            // Zoom in or out.
#if UNITY_2017_1_OR_NEWER
            if (Event.current.alt && Event.current.isScrollWheel)
#else
            if (Event.current.alt)
#endif
            {
                if (Event.current.delta.y < 0)
                {
                    Scale -= 0.01f;
                }

                if (Event.current.delta.y > 0)
                {
                    Scale += 0.01f;
                }
                Repaint();
            }

            switch (Event.current.rawType)
            {
                case EventType.MouseDown:
                    _clickedElement = ClickedElement.NoElement;
                    if (0 == Event.current.button)
                    {
                        _isDrag = true;
                    }
                    _UpdateTouch();
                    break;
                case EventType.MouseUp:
                    if (0 == Event.current.button)
                    {
                        if (_isDrag)
                            _CheckGroupRelease();
                        _isDrag = false;
                        if (_isLinking)
                        {
                            _isLinking = false;
                            _CheckLink();
                        }
                        if (_isAreaSelecting)
                        {
                            _isAreaSelecting = false;
                            _EndAreaSelection();
                        }
                    }
                    if (0 != _selectNodes.Count)
                    {
                        foreach (GKToyNode node in _selectNodes)
                            node.isMove = false;
                    }
                    if (_mouseOffset.Count > 0)
                        _mouseOffset.Clear();
                    break;
                case EventType.KeyDown:
                    _CheckHotKeys();
                    break;
            }
        }

        // 触摸事件响应.
        void _UpdateTouch()
        {
            if (default(Rect) == _responseRect)
            {
                _responseRect = new Rect(_contentView.x, _contentView.y,
                    _contentView.width - GUI.skin.verticalScrollbar.fixedWidth,
                    _contentView.height - GUI.skin.horizontalScrollbar.fixedHeight);
            }
            if (!_responseRect.Contains(Event.current.mousePosition))
                return;
            if (_UpdateNodeTouch())
                return;
            if (_UpdateLinkTouch())
                return;
            // 框选.
            if (0 == Event.current.button)
            {
                _isAreaSelecting = true;
                _mouseStart = Event.current.mousePosition + _contentScrollPos;
            }
            // 点击空白清除选择.
            if (ClickedElement.NoElement == _clickedElement && !Event.current.control)
                _selectNodes.Clear();
        }

        // 更新节点点击逻辑.
        protected virtual bool _UpdateNodeTouch()
        {
            // 链接时不可点击.
            if (_isLinking)
                return false;
            if (!_contentRect.Contains(Event.current.mousePosition))
                return false;
            Vector2 mousePos = Event.current.mousePosition + _contentScrollPos;
            foreach (GKToyNode node in _drawingNodes)
            {
                if (node.inputRect.Contains(mousePos))
                {
                    _tmpSelectNodes.Clear();
                    _tmpSelectNodes.Add(node);
                    return true;
                }
                else if (node.outputRect.Contains(mousePos))
                {
                    _tmpSelectNodes.Clear();
                    _tmpSelectNodes.Add(node);
                    // 禁止从虚拟出节点连出、禁止从已有连接的虚拟入节点连出
                    if (NodeType.VirtualNode == node.nodeType)
                    {
                        GKToyGroupLink groupLink = (GKToyGroupLink)node;
                        if (GroupLinkType.LinkOut == groupLink.linkType)
                            return false;
                    }
                    _isLinking = true;
                    return true;
                }
                else if (node.rect.Contains(mousePos))
                {
                    // 双击节点组展开.
                    if (NodeType.Group == node.nodeType && 2 == Event.current.clickCount)
                    {
                        _curGroup = (GKToyNodeGroup)node;
                        _SetDrawingNodes();
                    }
                    // 双击虚拟节点返回上层.
                    else if (NodeType.VirtualNode == node.nodeType && 2 == Event.current.clickCount)
                    {
                        _curGroup = null;
                        _SetDrawingNodes();
                    }
                    else if (2 == Event.current.clickCount)
                    {
                        GKToyMakerNodeComSelector.SelectCom(node);
                        node.DoubleClick();
                    }
                    else
                        _CheckNodeClick(node, mousePos);
                    return true;
                }
            }
            return false;
        }
        // 判断是否在组节点上松开鼠标.
        void _CheckGroupRelease()
        {
            // 链接时不可点击.
            if (_isLinking || 0 == _selectNodes.Count || _curGroup != null)
                return;
            if (!_contentRect.Contains(Event.current.mousePosition))
                return;
            foreach (GKToyNode node in _selectNodes)
            {
                if (NodeType.Group == node.nodeType)
                    return;
            }
            Vector2 mousePos = Event.current.mousePosition + _contentScrollPos;
            GKToyNodeGroup group = null;
            foreach (GKToyNode node in _drawingNodes)
            {
                if (NodeType.Group != node.nodeType)
                    continue;
                if (node.rect.Contains(mousePos))
                {
                    if (EditorUtility.DisplayDialog(_GetLocalization("Confirmation"), _GetLocalization("Are you sure to move selected node into group?"), _GetLocalization("OK"), _GetLocalization("Cancel")))
                        group = (GKToyNodeGroup)node;
                    else
                        return;
                }
            }
            if (null != group)
                _MoveInGroup(group, _selectNodes);
            return;
        }
        // 处理节点被点选事件.
        protected void _CheckNodeClick(GKToyNode node, Vector2 mousePos)
        {
            _clickedElement = ClickedElement.NodeElement;
            if (!(_selectNodes.Contains(node) || _tmpSelectNodes.Contains(node)))
            {
                _tmpSelectNodes.Clear();
                if (Event.current.control)
                    _tmpSelectNodes.AddRange(_selectNodes);
                _tmpSelectNodes.Add(node);
                if (0 == Event.current.button)
                {
                    _tmpSelectNodes[0].isMove = true;
                }
            }
            if (0 == Event.current.button)
            {
                _selectNodes.ForEach(x => x.isMove = true);
            }
            _selectLink = null;
            var selectNodes = 0 == _tmpSelectNodes.Count ? _selectNodes : _tmpSelectNodes;
            foreach (GKToyNode selectNode in selectNodes)
            {
                _mouseOffset.Add(selectNode, mousePos - selectNode.rect.position);
            }
        }

        // 链接线段点选检测.
        bool _UpdateLinkTouch()
        {
            if (0 == _drawingNodes.Count)
                return false;
            foreach (GKToyNode node in _drawingNodes)
            {
                var links = node.links;
                if (0 != links.Count)
                {
                    foreach (Link link in links)
                    {
                        if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[link.next]) && _CheckLinkClick(link, node, 10 <= (int)node.nodeType))
                            return true;
                    }
                }
                links = node.otherLinks;
                if (0 != links.Count)
                {
                    foreach (Link link in links)
                        if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[link.next]) && _CheckLinkClick(link, node, true))
                            return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 绘制框选区域
        /// </summary>
        void _UpdateAreaSelection()
        {
            Vector2 curPos = Event.current.mousePosition;
            Vector3 point1 = new Vector3(curPos.x, _mouseStart.y);
            Vector3 point2 = new Vector3(_mouseStart.x, curPos.y);
            Handles.DrawLines(new Vector3[] { _mouseStart, point1, point1, curPos, curPos, point2, point2, _mouseStart });
        }
        /// <summary>
        /// 框选结束，检测选择的节点
        /// </summary>
        void _EndAreaSelection()
        {
            Vector2 curPos = Event.current.mousePosition + _contentScrollPos;
            Rect selRect = new Rect(Mathf.Min(curPos.x, _mouseStart.x), Mathf.Min(curPos.y, _mouseStart.y),
                                Mathf.Abs(curPos.x - _mouseStart.x), Mathf.Abs(curPos.y - _mouseStart.y));
            foreach (GKToyNode node in _drawingNodes)
            {
                if (selRect.Contains(node.rect.center))
                    _tmpSelectNodes.Add(node);
            }
        }
        // 检测是否链接.
        void _CheckLink()
        {
            if (null == Event.current && 1 != _selectNodes.Count)
                return;
            GKToyNode _selectNode = _selectNodes[0];
            Vector2 mousePos = Event.current.mousePosition + _contentScrollPos;
            GKToyNode nextNode = null;
            int res;
            string paramKey = string.Empty;
            // 添加链接.
            foreach (GKToyNode node in _drawingNodes)
            {
                paramKey = string.Empty;
                res = node.CheckLink(_selectNode, mousePos, ref paramKey);
                if (0 < res)
                {
                    nextNode = node;
                    if (2 == res)
                        _AddLink(_selectNode, node, paramKey);
                    break;
                }
            }
            // 更新虚拟节点.
            if (null == nextNode)
                return;
            // 虚拟节点到其他.
            if (NodeType.VirtualNode == _selectNode.nodeType)
            {
                GKToyNode sourceNode = (GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)_selectNode).sourceNodeId];
                _AddLink(sourceNode, nextNode, paramKey);
                int otherGroupId = ((GKToyGroupLink)_selectNode).otherGroupId;
                if (-1 != otherGroupId)
                {
                    GKToyNodeGroup sourceGroup = (GKToyNodeGroup)CurRenderData.nodeLst[otherGroupId];
                    GKToyNode virtualNode = sourceGroup.FindVirtualOutLinkFromSource(nextNode.id);
                    if (null == virtualNode)
                        virtualNode = _CreateGroupOutLinkNode(_GenerateGUID(CurNodeIdx++), sourceGroup, nextNode, _curGroup.id);
                    _AddLink(sourceNode, virtualNode, paramKey);
                    if (null == sourceGroup.FindLinkFromOtherNode(_curGroup.id))
                        _AddLink(sourceGroup, _curGroup, string.Empty);
                }
                return;
            }
            // 其他到虚拟节点.
            if (NodeType.VirtualNode == nextNode.nodeType)
            {
                GKToyNode sourceNode = (GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)nextNode).sourceNodeId];
                _AddLink(_selectNode, sourceNode, paramKey);
                int otherGroupId = ((GKToyGroupLink)nextNode).otherGroupId;
                if (-1 != otherGroupId)
                {
                    GKToyNodeGroup sourceGroup = (GKToyNodeGroup)CurRenderData.nodeLst[otherGroupId];
                    GKToyNode virtualNode = sourceGroup.FindVirtualInLinkFromSource(_selectNode.id);
                    if (null == virtualNode)
                        virtualNode = _CreateGroupInLinkNode(_GenerateGUID(CurNodeIdx++), sourceGroup, _selectNode, _curGroup.id);
                    _AddLink(virtualNode, sourceNode, paramKey);
                    if (null == _curGroup.FindLinkFromOtherNode(sourceGroup.id))
                        _AddLink(_curGroup, sourceGroup, string.Empty);
                }
                return;
            }
            // 组到其他.
            if (NodeType.Group == _selectNode.nodeType)
            {
                if (NodeType.Group == nextNode.nodeType)
                {
                    _CreateGroupOutLinkNode(_GenerateGUID(CurNodeIdx++), (GKToyNodeGroup)_selectNode, nextNode, nextNode.id);
                    _CreateGroupInLinkNode(_GenerateGUID(CurNodeIdx++), (GKToyNodeGroup)nextNode, _selectNode, _selectNode.id);
                }
                else if (null == ((GKToyNodeGroup)_selectNode).FindVirtualOutLinkFromSource(nextNode.id))
                    _CreateGroupOutLinkNode(_GenerateGUID(CurNodeIdx++), (GKToyNodeGroup)_selectNode, nextNode, -1);
                return;
            }
            // 其他到组.
            if (NodeType.Group == nextNode.nodeType && null == ((GKToyNodeGroup)nextNode).FindVirtualInLinkFromSource(_selectNode.id))
                _CreateGroupInLinkNode(_GenerateGUID(CurNodeIdx++), (GKToyNodeGroup)nextNode, _selectNode, -1);
        }
        /// <summary>
        /// 检测链接是否被点击
        /// </summary>
        /// <param name="link">链接</param>
        /// <param name="node">链接头节点</param>
        /// <returns></returns>
        bool _CheckLinkClick(Link link, GKToyNode node, bool isVirtualLink)
        {
            switch (link.type)
            {
                case LinkType.RightAngle:
                    for (int i = 0; i < link.points.Count - 1; ++i)
                    {
                        bool isRightAngleVertical = 0 == (i & 1) ^ link.isFirstVertical;
                        Rect lineRect;
                        float x = Mathf.Min(link.points[i].x, link.points[i + 1].x) - ToyMakerBase._linkClickOffset;
                        float y = Mathf.Min(link.points[i].y, link.points[i + 1].y) - ToyMakerBase._linkClickOffset;
                        if (isRightAngleVertical)
                        {
                            lineRect = new Rect(x, y, ToyMakerBase._linkClickOffset * 2, Mathf.Abs(link.points[i].y - link.points[i + 1].y));
                        }
                        else
                        {
                            lineRect = new Rect(x, y, Mathf.Abs(link.points[i].x - link.points[i + 1].x), ToyMakerBase._linkClickOffset * 2);
                        }
                        if (lineRect.Contains(Event.current.mousePosition + _contentScrollPos))
                        {
                            _selectLink = link;
                            _tmpSelectNodes.Clear();
                            _tmpSelectNodes.Add(node);
                            if (isVirtualLink)
                                _clickedElement = ClickedElement.VirtualLinkElement;
                            else
                                _clickedElement = ClickedElement.LinkElement;
                            return true;
                        }
                    }
                    break;
                case LinkType.StraightLine:
                    //  ________
                    // |   ||||||
                    // |   ||||||
                    // ——————————
                    // |||||    |
                    // |||||____|
                    // 直线触摸区域二分法实现, 分别取左右宽高为举证拼接而成.
                    Rect lineRectA;
                    Rect lineRectB;
                    Vector2 PosA = (link.points[0].x <= link.points[link.points.Count - 1].x) ? link.points[0] : link.points[link.points.Count - 1];
                    Vector2 PosB = (link.points[0].x > link.points[link.points.Count - 1].x) ? link.points[0] : link.points[link.points.Count - 1];
                    float width = Mathf.Abs(PosA.x - PosB.x) * 0.5f;
                    float height = Mathf.Abs(PosA.y - PosB.y) * 0.5f;
                    lineRectA = new Rect(PosA.x, PosA.y, width, height);
                    if (PosA.y <= PosB.y)
                    {
                        lineRectB = new Rect(PosA.x + width, PosA.y + height, width, height);
                    }
                    else
                    {
                        PosA.y -= height;
                        lineRectA.y = PosA.y;
                        lineRectB = new Rect(PosA.x + width, PosA.y - height, width, height);
                    }
                    if (lineRectA.Contains(Event.current.mousePosition + _contentScrollPos)
                        || lineRectB.Contains(Event.current.mousePosition + _contentScrollPos))
                    {
                        _selectLink = link;
                        _tmpSelectNodes.Clear();
                        _tmpSelectNodes.Add(node);
                        if (isVirtualLink)
                            _clickedElement = ClickedElement.VirtualLinkElement;
                        else
                            _clickedElement = ClickedElement.LinkElement;
                        return true;
                    }
                    break;
            }
            return false;
        }
        #endregion

        #region Information
        // 绘制信息界面.
        void _DrawInformation()
        {
            GUILayout.BeginVertical("Box", GUILayout.Width(ToyMakerBase._informationWidth), GUILayout.ExpandHeight(true));
            {
                // 绘制Tab.
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Toggle(InformationType.Detail == _infoType, _GetLocalization("Detaile"), EditorStyles.toolbarButton, GUILayout.Height(ToyMakerBase._lineHeight)))
                    {
                        _infoType = InformationType.Detail;
                    }
                    if (GUILayout.Toggle(InformationType.Actions == _infoType, _GetLocalization("Node"), EditorStyles.toolbarButton, GUILayout.Height(ToyMakerBase._lineHeight)))
                    {
                        _infoType = InformationType.Actions;
                    }
                    if (GUILayout.Toggle(InformationType.Variables == _infoType, _GetLocalization("Variable"), EditorStyles.toolbarButton, GUILayout.Height(ToyMakerBase._lineHeight)))
                    {
                        _infoType = InformationType.Variables;
                    }
                    if (GUILayout.Toggle(InformationType.Search == _infoType, _GetLocalization("Search"), EditorStyles.toolbarButton, GUILayout.Height(ToyMakerBase._lineHeight)))
                    {
                        _infoType = InformationType.Search;
                    }
                    if (GUILayout.Toggle(InformationType.Inspector == _infoType, _GetLocalization("Inspector"), EditorStyles.toolbarButton, GUILayout.Height(ToyMakerBase._lineHeight)))
                    {
                        _infoType = InformationType.Inspector;
                    }
                }
                GUILayout.EndHorizontal();
                // 绘制内容.
                _DrawInformationContent();
            }
            GUILayout.EndVertical();
        }
        Vector2 _inspectorScrollPos = Vector2.zero;
        // 绘制信息界面.
        void _DrawInformationContent()
        {
            _inspectorScrollPos = GUILayout.BeginScrollView(_inspectorScrollPos, false, false);
            switch (_infoType)
            {
                case InformationType.Detail:
                    _DrawDetail();
                    break;
                case InformationType.Actions:
                    _DrawTasks();
                    break;
                case InformationType.Variables:
                    _DrawVariables();
                    break;
                case InformationType.Search:
                    _DrawSearch();
                    break;
                case InformationType.Inspector:
                    _DrawInspector();
                    break;
            }
            GUILayout.EndScrollView();
        }
        #endregion

        #region Content
        int _tmpDataIdx;
        int _tmpVirtualNodeIdx;
        // 绘制工具栏.
        void _DrawToolBar()
        {
            GUILayout.BeginHorizontal();
            {
                // 切换数据源.
                GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(ToyMakerBase._minWidth - ToyMakerBase._informationWidth - 640 - 16), GUILayout.Height(ToyMakerBase._lineHeight));
                _tmpDataIdx = EditorGUI.Popup(new Rect(ToyMakerBase._informationWidth + ToyMakerBase._layoutSpace * 3 - 2, ToyMakerBase._layoutSpace * 2 - 1, ToyMakerBase._minWidth - ToyMakerBase._informationWidth - 640 - 16, ToyMakerBase._lineHeight),
                                             CurDataIdx, _dataNameList.ToArray());
                if (_tmpDataIdx != CurDataIdx)
                {
                    CurDataIdx = _tmpDataIdx;
                    _curGroup = null;
                    _SetDrawingNodes();
                    foreach (GKToyNode node in CurRenderData.nodeLst.Values)
                    {
                        _UpdateNodeIcon(node);
                        if (NodeType.VirtualNode == node.nodeType)
                            _UpdateVirtualParam((GKToyGroupLink)node, (GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)node).sourceNodeId]);
                    }
                }
                // 返回中心.
                if (GUILayout.Button(_GetLocalization("Back to center"), EditorStyles.toolbarButton, GUILayout.Width(120), GUILayout.Height(ToyMakerBase._lineHeight + 4)))
                {
                    _BackToCenter();
                }
                // 锁定.
                _isLocking = GUILayout.Toggle(_isLocking, _GetLocalization("Lock"), EditorStyles.toolbarButton, GUILayout.Width(120), GUILayout.Height(ToyMakerBase._lineHeight));
                // 导出数据源.
                if (GUILayout.Button(_GetLocalization("Export"), EditorStyles.toolbarButton, GUILayout.Width(120), GUILayout.Height(ToyMakerBase._lineHeight)))
                {
                    var destPath = EditorUtility.SaveFilePanelInProject("Save path", "", "Asset", "Select save path.");
                    if (string.IsNullOrEmpty(destPath))
                        return;

                    // 存储数据源节点.
                    _ResetScaleData();
                    _overlord.Save();

                    var obj = ScriptableObject.CreateInstance<GKToyExternalData>();
                    obj.data = CurRenderData.Clone() as GKToyData;
                    GKEditor.CreateAsset(obj, destPath);
                    Selection.activeObject = obj;
                }
                // 截屏.
                if (GUILayout.Button(_GetLocalization("Take screenshot"), EditorStyles.toolbarButton, GUILayout.Width(160), GUILayout.Height(ToyMakerBase._lineHeight)))
                {
                    // 计算截屏的Scale、ScrollPos，设置截屏参数. 
                    float minX = (_drawingNodes.Min(x => x.inputRect.xMin) - _contentRect.x) / Scale - ToyMakerBase._lineHeight;
                    float maxX = (_drawingNodes.Max(x => x.outputRect.xMax) - _contentRect.x) / Scale + ToyMakerBase._lineHeight;
                    float minY = (_drawingNodes.Min(x => x.rect.yMin) - _contentRect.y) / Scale - ToyMakerBase._lineHeight * 2;
                    float maxY = (_drawingNodes.Max(x => x.rect.yMax) - _contentRect.y) / Scale + ToyMakerBase._lineHeight;
                    int shotWidth = (int)(_contentView.width - GUI.skin.verticalScrollbar.fixedWidth);
                    int shotHeight = (int)(_contentView.height - GUI.skin.horizontalScrollbar.fixedHeight);
                    float shotX = _contentView.x;
                    float shotY = GUI.skin.horizontalScrollbar.fixedHeight + ToyMakerBase._layoutSpace * 4;
                    float scaleX = shotWidth / (maxX - minX);
                    float scaleY = shotHeight / (maxY - minY);
                    float shotScale = Mathf.Min(scaleX, scaleY, 1);

                    Debug.Log(string.Format("minX:{0}|minY:{1}|maxX:{2}|maxY:{3}|shotWidth:{4}|shotHeight:{5}|shotX:{6}|shotY:{7}|scaleX:{8}|scaleY:{9}|shotScale:{10}", minX, minY, maxX, maxY, shotWidth, shotHeight, shotX, shotY, scaleX, scaleY, shotScale));

                    if (shotScale == 1)
                    {
                        shotY += shotHeight - (int)(maxY - minY);
                        shotWidth = (int)(maxX - minX);
                        shotHeight = (int)(maxY - minY);
                    }
                    if (null == _screenshot)
                        _screenshot = new GKEditorScreenshot();
                    _screenshot.SaveParams(shotWidth, shotHeight, new Rect(shotX, shotY, shotWidth, shotHeight), _contentSacle, _contentScrollPos);
                    _contentSacle = shotScale;
                    _contentScrollPos = new Vector2(minX * Scale, minY * Scale + ToyMakerBase._lineHeight);
                }
                // 设置语言.
                if (GUILayout.Button(string.Format(_GetLocalization("Language") + " / {0}", _language.ToString()), EditorStyles.toolbarButton, GUILayout.Width(120), GUILayout.Height(ToyMakerBase._lineHeight)))
                {
                    if (LanguageType.Chinese == _language)
                        _language = LanguageType.English;
                    else
                        _language = LanguageType.Chinese;
                    _UpdateNodeDecs();
                    root = TreeNode.Get().GenerateFileTree(GKToyMakerTypeManager.Instance().typeAttributeDict, _language.ToString(), GKToyMakerTypeManager.Instance().hideNodes);
                }

            }
            GUILayout.EndHorizontal();
            // 层次按钮.
            _tmpRect.x = ToyMakerBase._informationWidth + ToyMakerBase._layoutSpace * 4;
            _tmpRect.y = ToyMakerBase._lineHeight + ToyMakerBase._layoutSpace;
            _tmpRect.width = 120;
            _tmpRect.height = ToyMakerBase._lineHeight;
            if (GUI.Button(_tmpRect, CurRenderData.name, "GUIEditor.BreadcrumbLeft"))
            {
                _curGroup = null;
                _SetDrawingNodes();
            }
            if (null != _curGroup)
            {
                _tmpRect.x += _tmpRect.width;
                GUI.Button(_tmpRect, _curGroup.name, "GUIEditor.BreadcrumbMid");
            }
            _tmpRect.x = ToyMakerBase._minWidth - ToyMakerBase._layoutSpace * 1.5f - _tmpRect.width;
            // 绘制数据还原按钮.
            if (GUI.Button(_tmpRect, _GetLocalization("Restore data"), EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog(_GetLocalization("Confirmation"), _GetLocalization("Are you sure to restore data to last save?"), _GetLocalization("OK"), _GetLocalization("Cancel")))
                {
                    bool res = _overlord.Restore();
                    if (!res)
                    {
                        ShowNotification(new GUIContent(_GetLocalization("Restore fail. wrong backup files")));
                    }
                    else
                    {
                        _ResetSelected(_overlord);
                        ShowNotification(new GUIContent(_GetLocalization("Restore success")));
                    }
                }
            }
            // 绘制游戏数据导出按钮.
            _tmpRect.width = 160;
            _tmpRect.x -= _tmpRect.width;
            if (GUI.Button(_tmpRect, _GetLocalization("Export Game Data"), EditorStyles.toolbarButton))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(_GetLocalization("Client")), false, _ExportGameData, (object)(string.Format("1|{0}", _overlord.internalData.data.name)));
                menu.AddItem(new GUIContent(_GetLocalization("Server")), false, _ExportGameData, (object)(string.Format("2|{0}", _overlord.internalData.data.name)));
                menu.ShowAsContext();
            }
            // 绘制快捷键按钮.
            _tmpRect.width = 120;
            _tmpRect.x -= _tmpRect.width;
            if (GUI.Button(_tmpRect, _GetLocalization("Shortcut keys"), EditorStyles.toolbarButton))
            {
                GetWindow<GKToyMakerShortcuts>(true, _GetLocalization("Shortcut keys"), true);
            }
            // 绘制虚拟节点选择按钮.
            if (1 == _selectNodes.Count && NodeType.VirtualNode == _selectNodes[0].nodeType && -1 != ((GKToyGroupLink)_selectNodes[0]).otherGroupId && 0 != _virtualNodeSources.Count)
            {
                _tmpRect.width = 200;
                _tmpRect.x -= _tmpRect.width;
                _tmpVirtualNodeIdx = EditorGUI.Popup(_tmpRect, _curVirtualNodeIdx, _virtualNodeNames);
                if (_tmpVirtualNodeIdx != _curVirtualNodeIdx)
                {
                    _ChangeVirtualSourceNode((GKToyGroupLink)_selectNodes[0], _virtualNodeSources[_tmpVirtualNodeIdx]);
                    _curVirtualNodeIdx = _tmpVirtualNodeIdx;
                }
            }
        }

        void _BackToCenter()
        {
            if (0 != _drawingNodes.Count)
            {
                float minX = (_drawingNodes.Min(x => x.inputRect.xMin) - ToyMakerBase._informationWidth) / Scale - ToyMakerBase._lineHeight;
                float maxX = (_drawingNodes.Max(x => x.outputRect.xMax) - ToyMakerBase._informationWidth) / Scale + ToyMakerBase._lineHeight;
                float minY = (_drawingNodes.Min(x => x.rect.yMin) - ToyMakerBase._lineHeight * 2) / Scale - ToyMakerBase._lineHeight * 2;
                float maxY = (_drawingNodes.Max(x => x.rect.yMax) - ToyMakerBase._lineHeight * 2) / Scale + ToyMakerBase._lineHeight;
                _contentScrollPos = new Vector2((minX + maxX) / 2 * Scale - _contentView.width / 2, (minY + maxY) / 2 * Scale - _contentView.height / 2 + ToyMakerBase._lineHeight);
            }
        }

        // 绘制事件内容.
        void _DrawContent()
        {
            _contentRect.x = ToyMakerBase._informationWidth + ToyMakerBase._layoutSpace * 3;
            _contentRect.y = ToyMakerBase._lineHeight * 2 + ToyMakerBase._layoutSpace;
            _contentRect.width = (ToyMakerBase._maxWidth - ToyMakerBase._informationWidth - ToyMakerBase._layoutSpace * 4) * Scale;
            _contentRect.height = (ToyMakerBase._maxHeight - ToyMakerBase._lineHeight * 2 - ToyMakerBase._layoutSpace * 3) * Scale;
            _contentScrollPos = GUI.BeginScrollView(_contentView, _contentScrollPos, _contentRect);
            {

                if (_isScale)
                {
                    _contentScrollPos = _tmpScalePos;
                }

                _DrawBlackGroundGrid();
                _DrawLinks();
                // 绘制行为节点.
                foreach (GKToyNode node in _drawingNodes)
                {
                    _DrawNode(node);
                    if (_isDrag)
                        Repaint();
                }

                if (_isAreaSelecting)
                    _UpdateAreaSelection();

                _DrawMenu(_contentRect);

            }
            GUI.EndScrollView();
            // 截屏时隐藏Content上的信息.
            if (null == _screenshot || _screenshot.frameCount == 0)
            {
                _DrawContentInformation();
            }
        }

        // 绘制内容信息.
        void _DrawContentInformation()
        {
            // 标题绘制.
            GUI.Label(new Rect(ToyMakerBase._informationWidth + ToyMakerBase._layoutSpace * 3 + 10,
                                   ToyMakerBase._lineHeight * 2 + ToyMakerBase._layoutSpace, 400, 100),
                      string.Format("{0}", CurRenderData.name), ToyMakerBase._titleStyle);
            // 缩放比列尺绘制.
            GUI.Label(new Rect(ToyMakerBase._minWidth - 140, ToyMakerBase._lineHeight * 2 + ToyMakerBase._layoutSpace + 10, 100, 100),
                      string.Format("X {0:N1} ", Scale), ToyMakerBase._titleStyle);

            if (!string.IsNullOrEmpty(_nodeDes))
            {
                float desHeight = ToyMakerBase._descriptionStyle.fontSize * (_nodeDes.Count(x => x == '\n') + 1) + ToyMakerBase._descriptionStyle.padding.vertical;
                GUI.Label(new Rect(_contentView.x + ToyMakerBase._lineHeight, _contentView.y + _contentView.height - ToyMakerBase._lineHeight - GUI.skin.horizontalScrollbar.fixedHeight - desHeight, _contentView.width / 3, desHeight), _nodeDes, ToyMakerBase._descriptionStyle);
            }
        }

        // 绘制背景网格.
        void _DrawBlackGroundGrid()
        {
            GUI.backgroundColor = ToyMakerBase._bgColor;
            GUI.Box(new Rect(0, 0, ToyMakerBase._maxWidth * 2 * Scale, ToyMakerBase._maxHeight * 2 * Scale), "", ToyMakerBase._contentStyle);
            GUI.backgroundColor = _lastBgColor;
            Handles.color = ToyMakerBase._fgColor;
            int x1 = ToyMakerBase._informationWidth + ToyMakerBase._layoutSpace * 3;
            int x2 = (int)((ToyMakerBase._maxWidth * 2 - ToyMakerBase._layoutSpace) * Scale);
            int y1 = ToyMakerBase._layoutSpace;
            int space = 0;
            for (int i = 0; y1 + i * 25 * Scale < ToyMakerBase._maxHeight * 2 * Scale - ToyMakerBase._layoutSpace; i++)
            {
                space = (int)(y1 + i * 25 * Scale);
                //每3根加粗.
                if (i % 3 == 0)
                {
                    Handles.DrawLine(new Vector2(x1, y1 + space - 2), new Vector3(x2, y1 + space - 2));
                }
                Handles.DrawLine(new Vector2(x1, y1 + space), new Vector3(x2, y1 + space));
            }

            y1 = ToyMakerBase._layoutSpace;
            int y2 = ToyMakerBase._maxHeight * 2 - ToyMakerBase._layoutSpace;
            x1 = ToyMakerBase._layoutSpace * 3;
            for (int i = 0; x1 + i * 25 * Scale < ToyMakerBase._maxWidth * 2 * Scale - ToyMakerBase._layoutSpace; i++)
            {
                space = (int)(x1 + i * 25 * Scale);

                //每3根加粗.
                if (i % 3 == 0)
                {
                    Handles.DrawLine(new Vector2(x1 + space - 2, y1), new Vector3(x1 + space - 2, y2));
                }
                Handles.DrawLine(new Vector2(x1 + space, y1), new Vector3(x1 + space, y2));
            }
        }
        /// <summary>
        /// 导出游戏数据
        /// </summary>
        /// <param name="parameter">数据类型|导出路径（类型：1-客户端，2-服务器）</param>
        protected virtual void _ExportGameData(object parameter)
        {
            string[] info = ((string)parameter).Split('|');
            string dataType = info[0];
            string fileName = info[1];
            // 存储数据源节点.
            _ResetScaleData();
            _overlord.Save();
            string destPath;
            // 导出路径.
            if (1 == int.Parse(dataType))
            {
                // 客户端.
                destPath = ToyMakerBase._defaultClientPath;
                if (!Directory.Exists(destPath))
                {
                    do
                    {
                        destPath = EditorUtility.OpenFolderPanel(_GetLocalization("Save path"), Application.dataPath, _GetLocalization("Select save path."));
                    } while (!Directory.Exists(destPath));
                    ToyMakerBase._defaultClientPath = destPath;
                }
                destPath = string.Format("{0}/{1}.lua", ToyMakerBase._defaultClientPath, fileName);
                GKToyMakerDataExporter.ExportClientData(CurRenderData, destPath);
            }
            else
            {
                // 服务器.
                destPath = ToyMakerBase._defaultServerPath;
                if (!Directory.Exists(destPath))
                {
                    do
                    {
                        destPath = EditorUtility.OpenFolderPanel(_GetLocalization("Save path"), Application.dataPath, _GetLocalization("Select save path."));
                    } while (!Directory.Exists(destPath));
                    ToyMakerBase._defaultServerPath = destPath;
                }
                destPath = string.Format("{0}/{1}.xml", ToyMakerBase._defaultServerPath, fileName);
                GKToyMakerDataExporter.ExportServerData(CurRenderData, destPath);
            }
        }
        #endregion

        #region Update
        // 缓存数据赋值..
        protected void _Changed()
        {
            // 变量数据更新.
            if (0 != _addVariableLst.Count)
            {
                CurRenderData.variableChanged = true;
                foreach (var v in _addVariableLst)
                {
                    foreach (var obj in v.Value)
                    {
                        if (CurRenderData.variableLst.ContainsKey(v.Key))
                        {
                            CurRenderData.variableLst[v.Key].Add(obj);
                        }
                        else
                        {
                            List<object> lst = new List<object>();
                            lst.Add(obj);
                            CurRenderData.variableLst.Add(v.Key, lst);
                        }
                    }
                }
                _addVariableLst.Clear();
            }

            if (0 != _delVariableLst.Count)
            {
                CurRenderData.variableChanged = true;
                foreach (var v in _delVariableLst)
                {
                    foreach (var obj in v.Value)
                    {
                        CurRenderData.RemoveVariable(v.Key, obj);
                    }
                }
                _delVariableLst.Clear();
            }

            // 节点数据更新.
            if (null != _newNodeLst)
            {
                foreach (var node in _newNodeLst)
                {
                    CurRenderData.nodeLst.Add(node.Key, node.Value);
                }
                _newNodeLst.Clear();
            }
            if (0 != _removeNodeLst.Count)
            {
                foreach (var id in _removeNodeLst)
                {
                    if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[id]))
                        _drawingNodes.Remove((GKToyNode)CurRenderData.nodeLst[id]);
                    CurRenderData.nodeLst.Remove(id);
                }
                _removeNodeLst.Clear();
            }
            if (0 != _newGroupLinkLst.Count)
            {
                foreach (var node in _newGroupLinkLst)
                {
                    CurRenderData.nodeLst.Add(node.Key, node.Value);
                    ((GKToyNodeGroup)CurRenderData.nodeLst[node.Value.groupId]).groupLinkNodes.Add(node.Value.id);
                }
                _newGroupLinkLst.Clear();
            }
            if (0 != _removeGroupLinkLst.Count)
            {
                foreach (var id in _removeGroupLinkLst)
                {
                    if (CurRenderData.nodeLst.ContainsKey(((GKToyGroupLink)CurRenderData.nodeLst[id]).groupId))
                        ((GKToyNodeGroup)CurRenderData.nodeLst[((GKToyGroupLink)CurRenderData.nodeLst[id]).groupId]).groupLinkNodes.Remove(id);
                    if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[id]))
                        _drawingNodes.Remove((GKToyNode)CurRenderData.nodeLst[id]);
                    CurRenderData.nodeLst.Remove(id);
                }
                _removeGroupLinkLst.Clear();
            }
            // 链接数据更新.
            if (0 != _newLinkLst.Count)
            {
                foreach (var link in _newLinkLst)
                {
                    foreach (var l in link.Value)
                    {
                        // 计算节点高度.
                        int height = (int)(ToyMakerBase._nodeMinHeight * Scale);
                        if (!string.IsNullOrEmpty(l.parmKey))
                            height = (int)(ToyMakerBase._lineHeight * Scale);
                        if (CurRenderData.nodeLst.ContainsKey(l.node.id))
                        {
                            if (NodeType.VirtualNode == link.Key.nodeType)
                                ((GKToyGroupLink)link.Key).linkNodeIds.Add(l.node.id);
                            else if (NodeType.VirtualNode == l.node.nodeType)
                                ((GKToyGroupLink)l.node).linkNodeIds.Add(link.Key.id);
                            link.Key.AddLink(_GenerateGUID(CurLinkIdx++), l.node, height, l.parmKey);
                        }
                    }
                }
                _newLinkLst.Clear();
            }
            if (0 != _removeLinkLst.Count)
            {
                foreach (var nodeLst in _removeLinkLst)
                {
                    foreach (var node in nodeLst.Value)
                    {
                        if (NodeType.VirtualNode == nodeLst.Key.nodeType)
                            ((GKToyGroupLink)nodeLst.Key).linkNodeIds.Remove(node.id);
                        else if (NodeType.VirtualNode == node.nodeType)
                            ((GKToyGroupLink)node).linkNodeIds.Remove(nodeLst.Key.id);
                        nodeLst.Key.RemoveLink(node);
                    }
                }
                _removeLinkLst.Clear();
            }
            if (0 != _tmpSelectNodes.Count)
            {
                _selectNodes.Clear();
                _selectNodes.AddRange(_tmpSelectNodes);
                _tmpSelectNodes.Clear();
                // 更新选中虚拟节点的可选源节点.
                if (1 == _selectNodes.Count && NodeType.VirtualNode == _selectNodes[0].nodeType && -1 != ((GKToyGroupLink)_selectNodes[0]).otherGroupId)
                {
                    List<int> ids;
                    if (GroupLinkType.LinkIn == ((GKToyGroupLink)_selectNodes[0]).linkType)
                        ids = ((GKToyNodeGroup)CurRenderData.nodeLst[((GKToyGroupLink)_selectNodes[0]).otherGroupId]).GetUnlinkedInNodes(_curGroup);
                    else
                        ids = ((GKToyNodeGroup)CurRenderData.nodeLst[((GKToyGroupLink)_selectNodes[0]).otherGroupId]).GetUnlinkedOutNodes(_curGroup);
                    _virtualNodeSources.Clear();
                    if (0 != ids.Count)
                    {
                        _virtualNodeNames = new string[ids.Count + 1];
                        _virtualNodeSources.Add((GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)_selectNodes[0]).sourceNodeId]);
                        _virtualNodeNames[0] = _virtualNodeSources[0].name;
                        for (int i = 0; i < ids.Count; ++i)
                        {
                            _virtualNodeSources.Add((GKToyNode)CurRenderData.nodeLst[ids[i]]);
                            _virtualNodeNames[i + 1] = _virtualNodeSources[i + 1].name;
                        }
                        _curVirtualNodeIdx = 0;
                        _tmpVirtualNodeIdx = 0;
                    }
                }
                _UpdateNodeDecs();
            }
            if (0 != _tmpAddDrawingNodes.Count)
            {
                _drawingNodes.AddRange(_tmpAddDrawingNodes);
                _tmpAddDrawingNodes.Clear();
            }
        }
        /// <summary>
        /// 更新选择节点时，更新节点描述.
        /// </summary>
        void _UpdateNodeDecs()
        {
            if (1 == _selectNodes.Count && GKToyMakerTypeManager.Instance().desAttributeDict.ContainsKey(_selectNodes[0].className))
            {
                NodeDescriptionAttribute attr = GKToyMakerTypeManager.Instance().desAttributeDict[_selectNodes[0].className][_language.ToString()];
                if (null == attr)
                    _nodeDes = string.Empty;
                else
                    GK.AutoLineFeed(attr.desc, out _nodeDes,
                        (int)((_contentView.width / 3 - ToyMakerBase._descriptionStyle.padding.horizontal) / ToyMakerBase._descriptionStyle.fontSize));
            }
            else
                _nodeDes = string.Empty;
        }

        // 拖拽、缩放过程中更新链接线段.
        void _UpdateLinks()
        {
            if (_isDrag && !_isLinking && 0 != _selectNodes.Count)
            {
                foreach (GKToyNode _selectNode in _selectNodes)
                {
                    _UpdateAllLinks(_selectNode, (int)(ToyMakerBase._nodeMinHeight * Scale));
                    foreach (GKToyNode node in _drawingNodes)
                    {
                        node.UpdateLinkWithNode(_selectNode, (int)(ToyMakerBase._nodeMinHeight * Scale));
                    }
                }
            }
            else if (3 == _linkReCal || _isScale)
            {
                _UpdateAllLinks();
                _linkReCal = 0;
            }
            if (1 <= _linkReCal)
                ++_linkReCal;
        }
        // 更新所有链接.
        void _UpdateAllLinks()
        {
            foreach (GKToyNode node in _drawingNodes)
            {
                _UpdateAllLinks(node, (int)(ToyMakerBase._nodeMinHeight * Scale));
            }
        }
        void _UpdateAllLinks(GKToyNode node, int height)
        {
            if (0 != node.links.Count)
            {
                foreach (Link l in node.links)
                {
                    node.UpdateLink(l, (GKToyNode)CurRenderData.nodeLst[l.next], height);
                }
            }
            if (0 != node.otherLinks.Count)
            {
                foreach (Link l in node.otherLinks)
                    node.UpdateLink(l, (GKToyNode)CurRenderData.nodeLst[l.next], height);
            }
        }
        // 检查截屏状态.
        void _CheckScreenshot()
        {
            if (null != _screenshot && _screenshot.frameCount > 0)
            {
                _screenshot.frameCount++;
                switch (_screenshot.frameCount)
                {
                    case 2:
                        // 第1帧：更新链接.
                        _UpdateAllLinks();
                        break;
                    case 3:
                        // 第2帧：截屏、还原Scale和ScrollPos.
                        object[] paras = _screenshot.TakeScreenshot();
                        Scale = (float)paras[0];
                        _contentScrollPos = (Vector2)paras[1];
                        break;
                    case 4:
                        // 第3帧：更新链接.
                        _screenshot.RestoreState();
                        ShowNotification(new GUIContent(_GetLocalization("Screenshot Saved")));
                        _UpdateAllLinks();
                        break;
                }
            }
        }
        #endregion

        #region Detail
        // 绘制简介.
        virtual protected void _DrawDetail()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(_GetLocalization("Name") + " ", GUILayout.Height(ToyMakerBase._lineHeight));
                Name = EditorGUILayout.TextField(Name, GUILayout.Height(ToyMakerBase._lineHeight));
            }
            GUILayout.EndHorizontal();
            CurRenderData.comment = EditorGUILayout.TextArea(CurRenderData.comment, GUILayout.Height(ToyMakerBase._lineHeight * 5));

            GKEditor.DrawInspectorSeperator();

            // 绘制当前内部数据源;
            GUILayout.Label(_GetLocalization("Internal Data"));
            var id = _overlord.internalData;
            _overlord.internalData = EditorGUILayout.ObjectField(_overlord.internalData, typeof(GKToyExternalData), false) as GKToyExternalData;
            if (id != _overlord.internalData && null != _overlord.internalData)
            {
                _overlord.internalData.data.Init(_overlord);
                _UpdateDataNames();
            }
            EditorGUILayout.Space();

            // 绘制外部数据.
            _DrawExternalDataContent();
        }

        // 绘制外部数据内容.
        protected void _DrawExternalDataContent()
        {
            GUILayout.Label(_GetLocalization("External Data"));

            for (int i = 0; i < _overlord.externalDatas.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    var ed = _overlord.externalDatas[i];
                    _overlord.externalDatas[i] = EditorGUILayout.ObjectField(_overlord.externalDatas[i], typeof(GKToyExternalData), false) as GKToyExternalData;
                    // 如果新增或变更外部对象, 更新数据列表.
                    if (ed != _overlord.externalDatas[i] && null != _overlord.externalDatas[i])
                    {
                        _overlord.externalDatas[i].data.Init(_overlord);
                        _UpdateDataNames();
                    }
                    GUI.backgroundColor = ToyMakerBase._removeBgColor;
                    if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(14)))
                    {
                        _overlord.externalDatas.Remove(_overlord.externalDatas[i]);
                        _UpdateDataNames();
                        break;
                    }
                    GUI.backgroundColor = _lastBgColor;
                }
                GUILayout.EndHorizontal();
            }
            GUI.backgroundColor = ToyMakerBase._addBgColor;
            if (GUILayout.Button("+"))
            {
                _overlord.externalDatas.Add(null);
            }
            GUI.backgroundColor = _lastBgColor;
        }
        #endregion

        #region Task
        // 绘制节点列表.
        virtual protected void _DrawTasks()
        {
            if (root != null)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                {
                    TypeSearchContent = EditorGUILayout.TextField(TypeSearchContent, new GUIStyle("SearchTextField"), GUILayout.Height(ToyMakerBase._lineHeight));
                    if (GUILayout.Button("", "SearchCancelButton", GUILayout.Height(ToyMakerBase._lineHeight)))
                    {
                        TypeSearchContent = string.Empty;
                    }
                }
                GUILayout.EndHorizontal();

                // 绘制内容.
                if (string.IsNullOrEmpty(TypeSearchContent))
                {
                    _DrawTypeTree(root, 0, 0);
                }
                else
                {
                    foreach (var n in _typeSearchList)
                    {
                        _DrawTypeSearchContent(n.Key, n.Value);
                    }
                }
                GUILayout.EndVertical();
            }
        }
        // 节点类型搜索信息变更响应.
        protected void _UpdateTypeSearchValue()
        {
            _typeSearchList.Clear();
            if (string.IsNullOrEmpty(TypeSearchContent))
                return;
            foreach (var path in TreeNode.Get().pathData)
            {
                if (path.Key.Contains(TypeSearchContent))
                {
                    _typeSearchList.Add(path.Key.Substring(path.Key.LastIndexOf('/') + 1), path.Value);
                }
            }
        }
        protected void _DrawTypeSearchContent(string name, string type)
        {
            if (GUILayout.Button(name))
            {
                _tmpSelectNodes.Clear();
                Assembly assem = GKToyMakerTypeManager.Instance().typeAssemblyDict[type];
                GKToyNode newNode = assem.CreateInstance(type, true, System.Reflection.BindingFlags.Default, null, new object[] { _GenerateGUID(CurDataIdx++) }, null, null) as GKToyNode;
                newNode.className = type;
                newNode.pos.x = (_contentScrollPos.x + ToyMakerBase._minWidth * 0.5f) / Scale;
                newNode.pos.y = (_contentScrollPos.y + ToyMakerBase._minHeight * 0.5f) / Scale;
                _CreateNode(newNode);
            }
        }
        bool isEmptyTree;
        void _DrawTypeTree(TreeNode node, int level, int treeIndex)
        {
            if (node == null)
            {
                return;
            }
            if (level != 0)
            {
                treeIndex++;
                if (node.nodeType == TreeNode.TreeNodeType.Switch)
                {
                    if (0 == node.children.Count)
                        return;
                    isEmptyTree = true;
                    foreach (TreeNode subNode in node.children)
                    {
                        if (subNode.nodeType == TreeNode.TreeNodeType.Switch || (subNode.space == _GetNamespace() || subNode.space == _GetParentNamespace()))
                            isEmptyTree = false;
                    }
                    if (isEmptyTree)
                        return;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10 * (level - 1));
#if UNITY_2017_1_OR_NEWER
                    node.isOpen = EditorGUILayout.Foldout(node.isOpen, node.name, true);
#else
                    node.isOpen = EditorGUILayout.Foldout(node.isOpen, node.name);
#endif
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if (node.space == _GetNamespace() || node.space == _GetParentNamespace())
                    {
                        if (GUILayout.Button(node.name))
                        {
                            _tmpSelectNodes.Clear();
                            Assembly assem = GKToyMakerTypeManager.Instance().typeAssemblyDict[node.key];
                            GKToyNode newNode = assem.CreateInstance(node.key, true, System.Reflection.BindingFlags.Default, null, new object[] { _GenerateGUID(CurNodeIdx++) }, null, null) as GKToyNode;
                            newNode.className = node.key;
                            newNode.pos.x = (_contentScrollPos.x + ToyMakerBase._minWidth * 0.5f) / Scale;
                            newNode.pos.y = (_contentScrollPos.y + ToyMakerBase._minHeight * 0.5f) / Scale;
                            _CreateNode(newNode);
                        }
                    }
                }
            }
            if (node == null || !node.isOpen || node.children == null)
            {
                return;
            }
            for (int i = 0; i < node.children.Count; i++)
            {
                _DrawTypeTree(node.children[i], level + 1, treeIndex);
            }
        }
        #endregion

        #region Localization
        // 获取本地化文本.
        static public string _GetLocalization(int id)
        {
            if (id < 0 || id >= Data._localizationData.Length)
            {
                Debug.LogError(string.Format("Get localization data faile. id: {0}", id));
                return string.Empty;
            }
            switch (_language)
            {
                case LanguageType.Chinese:
                    return Data._localizationData[id].chinese;
                case LanguageType.English:
                    return Data._localizationData[id].english;
                default:
                    return string.Empty;
            }
        }

        static public string _GetLocalization(string key)
        {
            GKToyLocalizationData.LocalizationData ld = Data.GetLocalizationData(key);

            if (null == ld)
                return string.Empty;

            switch (_language)
            {
                case LanguageType.Chinese:
                    return ld.chinese;
                case LanguageType.English:
                    return ld.english;
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Variables
        // 绘制节点列表.
        virtual protected void _DrawVariables()
        {
            if (null == _variableTypeNames)
                return;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(_GetLocalization("Name"), GUILayout.Width(40));
                _newVariableName = EditorGUILayout.TextField(_newVariableName);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(_GetLocalization("Type"), GUILayout.Width(40));
                _newVariableIdx = EditorGUILayout.Popup(_newVariableIdx, _variableTypeNames);
                if (GUILayout.Button(_GetLocalization("Add"), GUILayout.Width(40), GUILayout.Height(14)))
                {
                    if (string.IsNullOrEmpty(_newVariableName))
                    {
                        EditorUtility.DisplayDialog(_GetLocalization("Tip"), _GetLocalization("Variable can't be empty."), _GetLocalization("OK"));
                    }
                    else if (!_IsExistVariable(_newVariableName))
                    {
                        string strType = string.Format("{0}.{1}", _GetNamespace(), _variableTypeNames[_newVariableIdx]);
                        if (null == Type.GetType(strType))
                        {
                            strType = string.Format("{0}.{1}", _GetParentNamespace(), _variableTypeNames[_newVariableIdx]);
                            if (null == Type.GetType(strType))
                                return;
                        }
                        Type t = typeof(GKToyVariable);
                        GKToyVariable v = (GKToyVariable)t.Assembly.CreateInstance(strType);
                        v.Name = _newVariableName;
                        v.InitializePropertyMapping(_overlord, CurRenderData);
                        if (_addVariableLst.ContainsKey(v.PropertyMapping))
                        {
                            _addVariableLst[v.PropertyMapping].Add(v);
                        }
                        else
                        {
                            List<object> lst = new List<object>();
                            lst.Add(v);
                            _addVariableLst.Add(v.PropertyMapping, lst);
                        }
                        _newVariableName = "";
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(_GetLocalization("Tip"), _GetLocalization("Rename the variable"), _GetLocalization("OK"));
                    }
                }
            }
            GUILayout.EndHorizontal();

            bool bDrawLine = false;
            if (0 != CurRenderData.variableLst.Count)
            {
                foreach (var vl in CurRenderData.variableLst)
                {
                    for (int i = 0; i < vl.Value.Count; i++)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            if (bDrawLine)
                            {
                                GKEditor.DrawMiniInspectorSeperator();
                                bDrawLine = true;
                            }
                            GKToyVariable v = (GKToyVariable)vl.Value[i];

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(string.Format("{0} : {1}", v.Name, v.GetType().Name));
                                GUI.backgroundColor = ToyMakerBase._removeBgColor;
                                if (GUILayout.Button("X", GUILayout.Width(20)))
                                {
                                    if (_delVariableLst.ContainsKey(vl.Key))
                                    {
                                        _delVariableLst[vl.Key].Add(vl.Value[i]);
                                    }
                                    else
                                    {
                                        List<object> lst = new List<object>();
                                        lst.Add(vl.Value[i]);
                                        _delVariableLst.Add(vl.Key, lst);
                                    }
                                }
                                GUI.backgroundColor = _lastBgColor;
                            }
                            GUILayout.EndHorizontal();

                            if (v.IsLst)
                            {
                                int tCount = v.GetValueCount();
                                for (int j = 0; j < tCount; j++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GKEditor.DrawBaseControl(true, v.GetValue(j), (obj) => { v.SetValue(j, obj); });
                                        GUI.backgroundColor = ToyMakerBase._removeBgColor;
                                        if (GUILayout.Button("X", GUILayout.Width(20)))
                                        {
                                            v.RemoveAt(j);
                                        }
                                        GUI.backgroundColor = _lastBgColor;
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUI.backgroundColor = ToyMakerBase._addBgColor;
                                if (GUILayout.Button(_GetLocalization("New element")))
                                {
                                    v.AddCapacity();
                                }
                                GUI.backgroundColor = _lastBgColor;
                            }
                            else
                            {
                                GKEditor.DrawBaseControl(true, v.GetValue(), (obj) => { v.SetValue(obj); });
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
        }

        // 检测变量是否重名.
        protected bool _IsExistVariable(string key)
        {
            if (null == _overlord || 0 == CurRenderData.variableLst.Count)
                return false;

            foreach (var v in CurRenderData.variableLst.Values)
            {
                foreach (var n in v)
                {
                    if (((GKToyVariable)n).Name.Equals(key))
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region Search
        // 绘制搜索信息.
        protected void _DrawSearch()
        {
            GUILayout.BeginHorizontal();
            {
                SearchContent = EditorGUILayout.TextField(SearchContent, new GUIStyle("SearchTextField"), GUILayout.Height(ToyMakerBase._lineHeight));
                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Height(ToyMakerBase._lineHeight)))
                {
                    SearchContent = string.Empty;
                }
            }
            GUILayout.EndHorizontal();

            // 绘制内容.
            foreach (var n in _searchList)
            {
                _DrawSearchContent(n);
            }
        }

        // 搜索信息变更响应.
        protected void _UpdateSearchValue()
        {
            _searchList.Clear();
            foreach (GKToyNode n in _overlord.internalData.data.nodeLst.Values)
            {
                string strName = string.Format("{0}{1}", n.name, n.LiteralId);
                if (strName.Contains(SearchContent))
                {
                    _searchList.Add((GKToyNode)n);
                }
            }
        }

        // 搜索内容绘制.
        protected void _DrawSearchContent(GKToyNode node)
        {
            if (GUILayout.Button(string.Format("{0}[{1}]", node.name, node.LiteralId), GUILayout.Height(ToyMakerBase._lineHeight)))
            {
                // 时间有限, 暂时先做缩放归1后跳转节点处理.
                if (!_drawingNodes.Contains(node))
                {
                    GKToyNodeGroup group = null;
                    GKToyNodeGroup tmpGroup = null;
                    foreach (GKToyNode groupNode in CurRenderData.nodeLst.Values)
                    {
                        if (NodeType.Group != groupNode.nodeType)
                            continue;
                        tmpGroup = (GKToyNodeGroup)groupNode;
                        if (tmpGroup.subNodes.Contains(node.id) || tmpGroup.groupLinkNodes.Contains(node.id))
                            group = tmpGroup;
                    }
                    _curGroup = group;
                    _SetDrawingNodes();
                }
                Scale = 1;
                float x = node.pos.x - _contentView.x - (_contentView.width - ToyMakerBase._nodeMinWidth) * 0.5f;
                float y = node.pos.y - _contentView.y - (_contentView.height - ToyMakerBase._nodeMinWidth) * 0.5f;
                _contentScrollPos = new Vector2(x, y);
                _tmpSelectNodes.Clear();
                _tmpSelectNodes.Add(node);
            }
        }
        #endregion

        #region Content
        //------------------------------ Link ------------------------------
        // 渲染链接线段.
        void _DrawLinks()
        {
            if (null == Event.current)
                return;

            // Draw current link line.
            if (_isLinking && 1 == _selectNodes.Count)
            {
                _DrawCurrentLink(_selectNodes[0]);
                Repaint();
            }

            // Draw links.
            foreach (GKToyNode node in _drawingNodes)
            {
                _DrawLinks(node, _selectLink);
            }

            // 绘制高亮连接.
            if (null != _selectLink)
            {
                for (int i = 0; i < _selectLink.points.Count - 1; ++i)
                {
                    _DrawLine(_selectLink, _selectLink);
                    //DrawLine(_selectLink.points[i], _selectLink.points[i + 1], 0 == (i & 1) ^ _selectLink.isFirstVertical, ToyMakerBase._selectionColor);
                }
            }

            Handles.color = Color.black;
        }

        // 绘制当前拖拽链接线.
        void _DrawCurrentLink(GKToyNode node)
        {
            float x = node.outputRect.x + node.outputRect.width;
            float y = node.outputRect.y + node.outputRect.height * 0.5f;
            _DrawLine(new Vector2(x, y), Event.current.mousePosition, Color.black, true);
        }

        // 绘制所有链接.
        void _DrawLinks(GKToyNode node, Link selectLink)
        {
            if (0 != node.links.Count)
            {
                foreach (Link link in node.links)
                {
                    // 高连线段最后绘制.
                    if (null != selectLink && link.id == selectLink.id)
                        continue;
                    if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[link.next]))
                        _DrawLine(link, selectLink);
                }
            }
            if (0 != node.otherLinks.Count)
            {
                foreach (Link link in node.otherLinks)
                {
                    // 高连线段最后绘制.
                    if (null != selectLink && link.id == selectLink.id)
                        continue;
                    if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[link.next]))
                        _DrawLine(link, selectLink);
                }
            }
        }

        // 绘制链接线段.
        protected void _DrawLine(Link link, Link selectLink)
        {
            Color c = link.color;
            if (null != selectLink && link.id == selectLink.id)
                c = ToyMakerBase._selectionColor;

            switch (link.type)
            {
                case LinkType.RightAngle:
                    for (int i = 0; i < link.points.Count - 1; ++i)
                    {
                        bool isRightAngleVertical = 0 == (i & 1) ^ link.isFirstVertical;
                        _DrawLine(link.points[i], link.points[i + 1], isRightAngleVertical, c);
                    }
                    break;
                case LinkType.StraightLine:
                    float x = Mathf.Abs(link.points[0].x - link.points[link.points.Count - 1].x);
                    float y = Mathf.Abs(link.points[0].y - link.points[link.points.Count - 1].y);
                    bool isStraightLineVertical = x < y;
                    _DrawLine(link.points[0], link.points[link.points.Count - 1], isStraightLineVertical, c);
                    break;
            }
        }

        protected void _DrawLine(Vector2 src, Vector2 dest, Color color, bool bSelect = false)
        {
            bool vertical = false;
            var lst = GK.ClacLinePoint(src, dest, out vertical, ToyMakerBase._nodeMinHeight, bSelect);
            switch (lst.Count)
            {
                case 2:
                    _DrawLine(lst[0], lst[1], vertical, color);
                    break;
                case 4:
                    _DrawLine(lst[0], lst[1], false, color);
                    _DrawLine(lst[1], lst[2], true, color);
                    _DrawLine(lst[2], lst[3], false, color);
                    break;
                case 6:
                    _DrawLine(lst[0], lst[1], false, color);
                    _DrawLine(lst[1], lst[2], true, color);
                    _DrawLine(lst[2], lst[3], false, color);
                    _DrawLine(lst[3], lst[4], true, color);
                    _DrawLine(lst[4], lst[5], false, color);
                    break;
                default:
                    Debug.LogError(string.Format("DrawCurrentLink faile. point Count: {0}", lst.Count));
                    break;
            }
        }

        // 绘制链接线段.
        protected void _DrawLine(Vector2 src, Vector2 dest, bool vertical, Color color)
        {
            float val = 1;
            Handles.color = color;
            for (int i = 0; i < 5; i++)
            {
                if (vertical)
                {
                    Handles.DrawLine(new Vector2(src.x + val, src.y), new Vector3(dest.x + val, dest.y));
                }
                else
                {
                    Handles.DrawLine(new Vector2(src.x, src.y + val), new Vector3(dest.x, dest.y + val));
                }
                val -= 0.5f;
            }
            Handles.color = Color.black;
        }

        //------------------------------ Node ------------------------------

        // 绘制Node.
        protected Rect _tmpRect;
        protected float _contentOffsetX;
        protected float _contentOffsetY;
        protected GKToyNode _tmpIONode;
        virtual protected void _DrawNode(GKToyNode node)
        {
            if (null == Event.current)
                return;

            var defaultColor = Color.white;
            switch (node.nodeType)
            {
                case NodeType.Action:
                    defaultColor = ToyMakerBase._actionColor;
                    break;
                case NodeType.Condition:
                    defaultColor = ToyMakerBase._conditionColor;
                    break;
                case NodeType.Decoration:
                    defaultColor = ToyMakerBase._decorationColor;
                    break;
                case NodeType.Group:
                    defaultColor = ToyMakerBase._groupColor;
                    break;
                case NodeType.VirtualNode:
                    defaultColor = ToyMakerBase._virtualColor;
                    break;
                default:
                    defaultColor = ToyMakerBase._removeBgColor;
                    break;
            }
            GUI.backgroundColor = defaultColor;

            _DrawNodeBg(node);

            _contentOffsetX = (1 - Scale) * _contentRect.x;
            _contentOffsetY = (1 - Scale) * _contentRect.y;
            // 如果当前为拖拽状态. 更新对象坐标.
            if (node.isMove && _isDrag && _mouseOffset.ContainsKey(node))
            {
                node.pos.x = (Event.current.mousePosition.x - _mouseOffset[node].x) / Scale + (1 - 1 / Scale) * _contentRect.x;
                node.pos.y = (Event.current.mousePosition.y - _mouseOffset[node].y) / Scale + (1 - 1 / Scale) * _contentRect.y;
            }

            // 计算Node宽高.
            int w = name.Length * ToyMakerBase._charWidth + 4;
            if (w <= ToyMakerBase._nodeMinWidth)
                w = ToyMakerBase._nodeMinWidth;
            node.width = w;
            node.height = ToyMakerBase._nodeMinHeight;

            // Right.
            _tmpRect.width = 10 * Scale;
            _tmpRect.height = (node.height - 24) * Scale;
            _tmpRect.x = (node.width + node.pos.x - 6) * Scale + _contentOffsetX;
            _tmpRect.y = (node.height * 0.5f + node.pos.y) * Scale - _tmpRect.height * 0.5f + _contentOffsetY;
            node.outputRect = _tmpRect;
            GUI.Button(_tmpRect, "");

            // Left.
            _tmpRect.x = (node.pos.x - 6) * Scale + _contentOffsetX;
            node.inputRect = _tmpRect;
            GUI.Button(_tmpRect, "");

            // Bg.
            _tmpRect.width = node.width * Scale;
            _tmpRect.height = node.height * Scale;
            _tmpRect.x = node.pos.x * Scale + _contentOffsetX;
            _tmpRect.y = node.pos.y * Scale + _contentOffsetY;
            node.rect = _tmpRect;
            //判断是正在否移动对象.
            ToyMakerBase._nodeStyle.fontSize = (int)(10 * Scale);
            ToyMakerBase._groupNodeStyle.fontSize = (int)(10 * Scale);
            ToyMakerBase._groupLinkStyle.fontSize = (int)(10 * Scale);
            ToyMakerBase._parmStyle.fontSize = (int)(10 * Scale);
            string strTypeName = null == node.outputObject ? "Null" : node.outputObject.GetType().Name;
            _tmpIONode = node;
            switch (node.nodeType)
            {
                case NodeType.Group:
                    GUI.Label(_tmpRect, string.Format("{0}\n[{1}]", node.name, node.LiteralId), ToyMakerBase._groupNodeStyle);
                    break;
                case NodeType.VirtualNode:
                    GUI.Label(_tmpRect, string.Format("{0}\n[{1}]", node.name, node.LiteralId), ToyMakerBase._groupLinkStyle);
                    if (GroupLinkType.LinkOut == ((GKToyGroupLink)node).linkType)
                        _tmpIONode = (GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)node).sourceNodeId];
                    break;
                default:
                    GUI.Label(_tmpRect, string.Format("{0}\n[{1}]\n{2}", node.name, node.LiteralId, strTypeName), ToyMakerBase._nodeStyle);
                    break;
            }
            _tmpRect.y += _tmpRect.height;
            // 绘制参数输入入口.
            if (null != node.ioStates)
            {
                int inputParmCount = 0;
                for (int i = 0; i < node.ioStates.Length; i++)
                {
                    if ((node.ioStates[i] & 1) == 1)
                    {
                        // 判断参数节点与链接输出类型是否一致, 一致变更颜色.
                        if (_isLinking && 1 == _selectNodes.Count && null != _selectNodes[0].outputObject && _selectNodes[0] != node
                           && _selectNodes[0].outputObject.GetType() == node.props[i].GetValue(_tmpIONode, null).GetType())
                        {
                            GUI.backgroundColor = ToyMakerBase._parmLinkBgColor;
                        }
                        else
                        {
                            GUI.backgroundColor = defaultColor;
                        }

                        _tmpRect.height = ToyMakerBase._lineHeight * Scale;
                        GUI.Label(_tmpRect, string.Format("{0} : {1}", node.props[i].Name, node.props[i].PropertyType.Name), ToyMakerBase._parmStyle);
                        node.parmRect[node.props[i].Name].rect = _tmpRect;
                        _tmpRect.y += ToyMakerBase._lineHeight * Scale;
                        inputParmCount++;
                    }
                }
            }
            GUI.backgroundColor = Color.white;
            // 批注.
            if (!string.IsNullOrEmpty(node.comment))
            {
                ToyMakerBase._commentStyle.fontSize = (int)(10 * Scale);
                _tmpRect.x += ToyMakerBase._commentIndent;
                _tmpRect.width -= ToyMakerBase._commentIndent * 2;
                string commentWord;
                int lines = GK.AutoLineFeed(node.comment, out commentWord, (int)((_tmpRect.width - ToyMakerBase._commentContentMargin) / ToyMakerBase._commentStyle.fontSize));
                _tmpRect.height = (lines + 1) * ToyMakerBase._commentStyle.lineHeight + 6;
                GUI.Box(_tmpRect, commentWord, ToyMakerBase._commentStyle);
            }
            // 绘制图标.
            if (node.icon != null)
            {
                float tmpSize = node.height * Scale - ToyMakerBase._layoutSpace * 4;
                _tmpRect.x = node.pos.x * Scale + _contentOffsetX + ToyMakerBase._layoutSpace * 2;
                _tmpRect.y = node.pos.y * Scale + _contentOffsetY + ToyMakerBase._layoutSpace * 2;
                _tmpRect.width = tmpSize;
                _tmpRect.height = tmpSize;
                GUI.DrawTexture(_tmpRect, node.icon);
            }
            // 绘制状态标志.
            if (node.state != NodeState.Inactive)
            {
                float tmpSize2 = node.height * Scale * 0.35f;
                _tmpRect.x = node.pos.x * Scale + _contentOffsetX + node.width * Scale - tmpSize2 * 0.7f;
                _tmpRect.y = node.pos.y * Scale + _contentOffsetY - tmpSize2 * 0.3f;
                _tmpRect.width = tmpSize2;
                _tmpRect.height = tmpSize2;
                switch (node.state)
                {
                    case NodeState.Activated:
                        GUI.DrawTexture(_tmpRect, ToyMakerBase._activatedMark);
                        break;
                    case NodeState.Success:
                        GUI.DrawTexture(_tmpRect, ToyMakerBase._successMark);
                        break;
                    case NodeState.Fail:
                        GUI.DrawTexture(_tmpRect, ToyMakerBase._failMark);
                        break;
                }
            }
        }

        // 绘制节点背景色.
        protected void _DrawNodeBg(GKToyNode node)
        {
            if (0 != _selectNodes.Count)
            {
                // 当前点选对象背景色.
                if (_selectNodes.Contains((GKToyNode)CurRenderData.nodeLst[node.id]))
                {
                    GUI.backgroundColor = ToyMakerBase._selectionColor;
                    return;
                }
                if (1 == _selectNodes.Count)
                {
                    // 当前点选输出节点背景色.
                    foreach (var l in _selectNodes[0].links)
                    {
                        if (l.next == node.id)
                        {
                            GUI.backgroundColor = ToyMakerBase._selectionOutputColor;
                            return;
                        }
                    }
                    foreach (var l in _selectNodes[0].otherLinks)
                    {
                        if (l.next == node.id)
                        {
                            GUI.backgroundColor = ToyMakerBase._selectionOutputColor;
                            return;
                        }
                    }

                    // 当前点选输入节点背景色.
                    foreach (var l in node.links)
                    {
                        if (l.next == _selectNodes[0].id)
                        {
                            GUI.backgroundColor = ToyMakerBase._selectionInputColor;
                            return;
                        }
                    }
                    foreach (var l in node.otherLinks)
                    {
                        if (l.next == _selectNodes[0].id)
                        {
                            GUI.backgroundColor = ToyMakerBase._selectionInputColor;
                            return;
                        }
                    }
                }
            }
        }

        //------------------------------ Inspector ------------------------------

        // 绘制Inspector.
        virtual protected void _DrawInspector()
        {
            if (1 != _selectNodes.Count)
                return;

            _DrawInspector(_selectNodes[0], ref _selectLink);
        }

        // 绘制详情.
        virtual public void _DrawInspector(GKToyNode node, ref Link selected)
        {
            if (NodeType.VirtualNode == node.nodeType)
                _DrawProperty((GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)node).sourceNodeId]);
            else
                _DrawProperty(node);
            bool isFirst;
            // 绘制节点连接.
            if (10 > (int)node.nodeType)
            {
                if (0 != node.links.Count)
                {
                    GKEditor.DrawInspectorSeperator();

                    GUILayout.Label(_GetLocalization("Links"));
                    GUILayout.BeginVertical("Box");
                    {
                        isFirst = true;
                        foreach (var l in node.links)
                        {
                            _DrawNextDetail(node, ref isFirst, l, ref selected);
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            else if (NodeType.Group == node.nodeType)
            {
                List<GKToyGroupLink> inNodes = ((GKToyNodeGroup)node).GetAllInNodes();
                List<GKToyGroupLink> outNodes = ((GKToyNodeGroup)node).GetAllOutNodes();
                if (0 != inNodes.Count || 0 != outNodes.Count)
                {
                    GKEditor.DrawInspectorSeperator();
                    GUILayout.Label(_GetLocalization("Links"));
                    if (0 != inNodes.Count)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            isFirst = true;
                            foreach (var n in inNodes)
                            {
                                _DrawInLinkDetail(node, ref isFirst, n, ref selected);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    if (0 != outNodes.Count)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            isFirst = true;
                            foreach (var n in outNodes)
                            {
                                _DrawOutLinkDetail(node, ref isFirst, n, ref selected);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
            else if (NodeType.VirtualNode == node.nodeType && GroupLinkType.LinkIn == ((GKToyGroupLink)node).linkType && 0 != ((GKToyGroupLink)node).linkNodeIds.Count)
            {
                GKEditor.DrawInspectorSeperator();

                GUILayout.Label(_GetLocalization("Links"));
                GUILayout.BeginVertical("Box");
                {
                    isFirst = true;
                    foreach (var l in node.links)
                    {
                        _DrawNextDetail(node, ref isFirst, l, ref selected);
                    }
                }
                GUILayout.EndVertical();
            }
        }

        virtual protected void _DrawProperty(GKToyNode node)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(_GetLocalization("Name") + " ", GUILayout.Height(ToyMakerBase._lineHeight));
                node.name = EditorGUILayout.TextField(node.name, GUILayout.Height(ToyMakerBase._lineHeight));
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(_GetLocalization("Comment"), GUILayout.Height(ToyMakerBase._lineHeight));
            node.comment = EditorGUILayout.TextArea(node.comment, GUILayout.Height(ToyMakerBase._lineHeight * 5));
            // 绘制节点属性.
            if (null != node.props && 0 != node.props.Length)
            {
                GKEditor.DrawInspectorSeperator();
                GUILayout.Label(_GetLocalization("Action"));
                GUILayout.BeginVertical("Box");
                {
                    for (int i = 0; i < node.props.Length; i++)
                    {
                        // 绘制分割线.
                        if (0 != i)
                            GKEditor.DrawMiniInspectorSeperator();

                        var prop = node.props[i];
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(prop.Name);
                            var v = prop.GetValue(node, null);
                            if (-1 != node.propStates[i])
                            {
                                var lst = _overlord.GetVariableNameListByType(v.GetType());
                                node.propStates[i] = EditorGUILayout.Popup(node.propStates[i], lst.ToArray());
                                GUI.backgroundColor = Color.gray;
                            }
                            else
                            {
                                // 处理自定义变量.
                                if (v is GKToyVariable)
                                {
                                    if (((GKToyVariable)v).IsLst)
                                    {
                                        GUILayout.BeginVertical();
                                        {
                                            int tCount = ((GKToyVariable)v).GetValueCount();
                                            for (int j = 0; j < tCount; j++)
                                            {
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GKEditor.DrawBaseControl(true, ((GKToyVariable)v).GetValue(j), (obj) => { ((GKToyVariable)v).SetValue(j, obj); });
                                                    GUI.backgroundColor = ToyMakerBase._removeBgColor;
                                                    if (GUILayout.Button("X", GUILayout.Width(20)))
                                                    {
                                                        ((GKToyVariable)v).RemoveAt(j);
                                                    }
                                                    GUI.backgroundColor = _lastBgColor;
                                                }
                                                GUILayout.EndVertical();
                                            }
                                            GUI.backgroundColor = ToyMakerBase._addBgColor;
                                            if (GUILayout.Button(_GetLocalization("New element")))
                                            {
                                                ((GKToyVariable)v).AddCapacity();
                                            }
                                            GUI.backgroundColor = _lastBgColor;
                                        }
                                        GUILayout.EndVertical();
                                    }
                                    else
                                    {
                                        GKEditor.DrawBaseControl(true, ((GKToyVariable)v).GetValue(), (obj) => { ((GKToyVariable)v).SetValue(obj); });
                                    }
                                }
                                else
                                {
                                    GKEditor.DrawBaseControl(true, v, (obj) => { prop.SetValue(node, obj, null); });
                                }
                            }
                            if (GUILayout.Button("●", GUILayout.Width(18), GUILayout.Height(16)))
                            {
                                node.propStates[i] = ((-1 == node.propStates[i]) ? 0 : -1);
                                // 清除参数可视化信息.
                                if (((node.ioStates[i] & 1) == 1))
                                {
                                    node.ioStates[i] = (node.ioStates[i] & 10);
                                    node.propLock[i] = false;
                                    _RemoveNodeLinks(node, node.props[i].Name);
                                }
                            }

                            // 展开按钮.
                            GUI.backgroundColor = ToyMakerBase._optBgColor;
                            string strOpt = ((node.ioStates[i] & 8) == 8) ? "-" : "+";
                            if (GUILayout.Button(strOpt, GUILayout.Width(ToyMakerBase._lineHeight)))
                            {
                                node.ioStates[i] = ((node.ioStates[i] & 8) == 8) ? (node.ioStates[i] & 7) : (node.ioStates[i] | 8);
                            }
                            GUI.backgroundColor = _lastBgColor;
                        }
                        GUILayout.EndHorizontal();

                        if ((node.ioStates[i] & 8) == 8)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                bool v1 = ((node.ioStates[i] & 1) == 1);
                                bool v2 = GUILayout.Toggle(v1, _GetLocalization("Input"));

                                // 如果删除参数节点, 需要删除所有相关链接.
                                // 每次输入节点变更时, 需要重置参数锁标志位状态.
                                if (v1 != v2)
                                {
                                    if (-1 == node.propStates[i])
                                    {
                                        node.ioStates[i] = v2 ? (node.ioStates[i] | 1) : (node.ioStates[i] & 14);
                                        if (!v2)
                                        {
                                            node.propLock[i] = false;
                                            node.ioStates[i] = node.ioStates[i] & 11;
                                            _RemoveNodeLinks(node, node.props[i].Name);
                                        }
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayDialog(_GetLocalization("Tip"), _GetLocalization("Unable to visualize reference variables"), _GetLocalization("OK"));
                                    }
                                }
                                //v = ((node.ioStates[i] & 2) == 2);
                                //v = GUILayout.Toggle(v, _GetLocalization(32));
                                //node.ioStates[i] = v ? (node.ioStates[i] | 2) : (node.ioStates[i] & 13);
                                // 仅当参数可视化时必要参数才有效.
                                if (v2)
                                {
                                    v1 = ((node.ioStates[i] & 4) == 4);
                                    v2 = GUILayout.Toggle(v1, _GetLocalization("Need"));
                                    // 每次输入必要节点变更时, 需要重置参数锁标志位状态.
                                    if (v1 != v2)
                                    {
                                        if (-1 == node.propStates[i])
                                        {
                                            node.ioStates[i] = v2 ? (node.ioStates[i] | 4) : (node.ioStates[i] & 11);
                                            if (!v2)
                                            {
                                                node.propLock[i] = false;
                                            }
                                            else
                                            {
                                                node.propLock[i] = true;
                                            }
                                        }
                                        else
                                        {
                                            EditorUtility.DisplayDialog(_GetLocalization("Tip"), _GetLocalization("Unable to visualize reference variables"), _GetLocalization("OK"));
                                        }
                                    }
                                }

                            }
                            GUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }

        // 绘制连接点详情.
        virtual protected void _DrawNextDetail(GKToyNode node, ref bool isFirst, Link l, ref Link selected)
        {
            if (!isFirst)
                GKEditor.DrawMiniInspectorSeperator();
            else
                isFirst = false;

            GUILayout.BeginHorizontal();
            {
                Link relateLink = null;
                if (_drawingNodes.Contains((GKToyNode)CurRenderData.nodeLst[l.next]))
                {
                    relateLink = l;
                }
                else
                {
                    if (null == _curGroup)
                    {
                        foreach (Link link in node.otherLinks)
                        {
                            if (((GKToyNodeGroup)CurRenderData.nodeLst[link.next]).subNodes.Contains(l.next))
                            {
                                relateLink = link;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Link link in node.otherLinks)
                        {
                            if (((GKToyGroupLink)CurRenderData.nodeLst[link.next]).sourceNodeId == l.next)
                            {
                                relateLink = link;
                                break;
                            }
                        }
                    }
                }
                if (null != selected && selected == relateLink)
                {
                    GUI.backgroundColor = ToyMakerBase._selectionColor;
                }
                if (GUILayout.Button(((GKToyNode)CurRenderData.nodeLst[l.next]).name))
                {
                    selected = relateLink;
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(ToyMakerBase._lineHeight)))
                {
                    _RemoveLinkAndVirtualLink(relateLink);
                }
                GUI.backgroundColor = ToyMakerBase._optBgColor;
                string strOpt = l.bOption ? "-" : "+";
                if (GUILayout.Button(strOpt, GUILayout.Width(ToyMakerBase._lineHeight)))
                {
                    l.bOption = !l.bOption;
                }
                GUI.backgroundColor = _lastBgColor;
            }
            GUILayout.EndHorizontal();

            if (l.bOption)
            {
                GUILayout.BeginHorizontal();
                {
                    l.type = (LinkType)EditorGUILayout.EnumPopup(l.type);
                    EditorGUILayout.Space();
                    l.color = EditorGUILayout.ColorField(l.color, GUILayout.Width(60));
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
        // 绘制组间入连接详情.
        virtual protected void _DrawInLinkDetail(GKToyNode node, ref bool isFirst, GKToyGroupLink nl, ref Link selected)
        {
            if (!isFirst)
                GKEditor.DrawMiniInspectorSeperator();
            else
                isFirst = false;

            Link relateLink = null;
            if (-1 != nl.otherGroupId)
                relateLink = ((GKToyNode)CurRenderData.nodeLst[nl.otherGroupId]).FindLinkFromOtherNode(node.id);
            else
                relateLink = ((GKToyNode)CurRenderData.nodeLst[nl.sourceNodeId]).FindLinkFromOtherNode(node.id);
            string endName;
            if (0 == nl.linkNodeIds.Count)
            {
                GUILayout.BeginHorizontal();
                {
                    endName = node.name;
                    if (null != selected && selected == relateLink)
                    {
                        GUI.backgroundColor = ToyMakerBase._selectionColor;
                    }
                    if (GUILayout.Button(string.Format("{0}{1}", nl.name.Substring(nl.name.IndexOf("\n") + 1), endName)))
                    {
                        selected = relateLink;
                    }
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(ToyMakerBase._lineHeight)))
                    {
                        _RemoveGroupLink(nl);
                    }
                    GUI.backgroundColor = _lastBgColor;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                foreach (int linkNode in nl.linkNodeIds)
                {
                    GUILayout.BeginHorizontal();
                    {
                        endName = ((GKToyNode)CurRenderData.nodeLst[linkNode]).name;
                        if (null != selected && selected == relateLink)
                        {
                            GUI.backgroundColor = ToyMakerBase._selectionColor;
                        }
                        if (GUILayout.Button(string.Format("{0}{1}", nl.name.Substring(nl.name.IndexOf("\n") + 1), endName)))
                        {
                            selected = relateLink;
                        }
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(ToyMakerBase._lineHeight)))
                        {
                            _RemoveLinkAndVirtualLink(nl, linkNode);
                        }
                        GUI.backgroundColor = _lastBgColor;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        // 绘制组间出连接详情.
        virtual protected void _DrawOutLinkDetail(GKToyNode node, ref bool isFirst, GKToyGroupLink nl, ref Link selected)
        {
            if (!isFirst)
                GKEditor.DrawMiniInspectorSeperator();
            else
                isFirst = false;

            Link relateLink = null;
            if (-1 != nl.otherGroupId)
                relateLink = node.FindLinkFromOtherNode(nl.otherGroupId);
            else
                relateLink = node.FindLinkFromNode(nl.sourceNodeId);
            string startName;
            if (0 == nl.linkNodeIds.Count)
            {
                GUILayout.BeginHorizontal();
                {
                    startName = node.name;
                    if (null != selected && selected == relateLink)
                    {
                        GUI.backgroundColor = ToyMakerBase._selectionColor;
                    }
                    if (GUILayout.Button(string.Format("{0}{1}", startName, nl.name.Substring(nl.name.IndexOf("->")))))
                    {
                        selected = relateLink;
                    }
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(ToyMakerBase._lineHeight)))
                    {
                        _RemoveGroupLink(nl);
                    }
                    GUI.backgroundColor = _lastBgColor;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                foreach (int linkNode in nl.linkNodeIds)
                {
                    GUILayout.BeginHorizontal();
                    {
                        startName = ((GKToyNode)CurRenderData.nodeLst[linkNode]).name;
                        if (null != selected && selected == relateLink)
                        {
                            GUI.backgroundColor = ToyMakerBase._selectionColor;
                        }
                        if (GUILayout.Button(string.Format("{0}{1}", startName, nl.name.Substring(nl.name.IndexOf("->")))))
                        {
                            selected = relateLink;
                        }
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(ToyMakerBase._lineHeight)))
                        {
                            _RemoveLinkAndVirtualLink(nl, linkNode);
                        }
                        GUI.backgroundColor = _lastBgColor;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        #endregion

        #region ContentMenu
        //判断鼠标右键事件.
        protected void _DrawMenu(Rect rect)
        {
            if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                var _tSelectNodes = (0 == _tmpSelectNodes.Count) ? _selectNodes : _tmpSelectNodes;
                switch (_clickedElement)
                {
                    case ClickedElement.NoElement:
                        foreach (var item in TreeNode.Get().pathData)
                        {
                            menu.AddItem(new GUIContent(item.Key.Substring(5)), false, _HandleMenuAddNode, new object[] { Event.current.mousePosition, item.Value });
                        }
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent(_GetLocalization("Reset")), false, _HandleMenuReset, Event.current.mousePosition);
                        break;
                    case ClickedElement.NodeElement:
                        if (1 == _tSelectNodes.Count)
                        {
                            switch (_tSelectNodes[0].nodeType)
                            {
                                // 删除组.
                                case NodeType.Group:
                                    menu.AddItem(new GUIContent(string.Format("{0} {1}", _GetLocalization("Delete Group"), _tSelectNodes[0].name)), false, _HandleMenuDeleteGroup);
                                    menu.AddSeparator("");
                                    menu.AddItem(new GUIContent(_GetLocalization("Delete Group And All Sub Nodes")), false, _HandleMenuDeleteGroupAndSubNodes);
                                    break;
                                case NodeType.VirtualNode:
                                    GKToyGroupLink link = (GKToyGroupLink)_tSelectNodes[0];
                                    menu.AddItem(new GUIContent(string.Format("{0} {1}", _GetLocalization("Delete Node"), _tSelectNodes[0].name)), false, _HandleMenuDeleteGroupLink);
                                    break;
                                default:
                                    // 不可自建的节点不可删除.
                                    if (null != _curGroup)
                                        menu.AddItem(new GUIContent(_GetLocalization("Move out of group")), false, _HandleMenuMoveOutGroup);
                                    if (!GKToyMakerTypeManager.Instance().hideNodes.Contains(_tSelectNodes[0].className))
                                        menu.AddItem(new GUIContent(string.Format("{0} {1}", _GetLocalization("Delete Node"), _tSelectNodes[0].name)), false, _HandleMenuDeleteNode);
                                    break;
                            }
                        }
                        else
                        {
                            // 选中多个节点右键菜单.
                            if (null == _curGroup)
                            {
                                bool hasGroupNode = false;
                                foreach (GKToyNode node in _tSelectNodes)
                                {
                                    if (NodeType.Group == node.nodeType)
                                        hasGroupNode = true;
                                }
                                if (!hasGroupNode)
                                {
                                    menu.AddItem(new GUIContent(_GetLocalization("Create Node Group")), false, _HandleMenuCollapseNode, Event.current.mousePosition);
                                    menu.AddSeparator("");
                                }
                            }
                            else
                            {
                                bool hasVirtualNode = false;
                                foreach (GKToyNode node in _tSelectNodes)
                                {
                                    if (NodeType.VirtualNode == node.nodeType)
                                        hasVirtualNode = true;
                                }
                                if (!hasVirtualNode)
                                {
                                    menu.AddItem(new GUIContent(_GetLocalization("Move out of group")), false, _HandleMenuMoveOutGroup);
                                    menu.AddSeparator("");
                                }
                            }
                            menu.AddItem(new GUIContent(_GetLocalization("Delete Group And All Sub Nodes")), false, _HandleMenuDeleteSelectedNodes);
                        }

                        break;
                    case ClickedElement.LinkElement:
                        if (1 == _tSelectNodes.Count)
                            menu.AddItem(new GUIContent(string.Format("{0}: {1} -> {2}", _GetLocalization("Delete Link"), _tSelectNodes[0].name, ((GKToyNode)CurRenderData.nodeLst[_selectLink.next]).name)), false, _HandleMenuDeleteLink);
                        break;
                    case ClickedElement.VirtualLinkElement:
                        if (NodeType.Group != _tSelectNodes[0].nodeType && NodeType.Group != ((GKToyNode)CurRenderData.nodeLst[_selectLink.next]).nodeType)
                            menu.AddItem(new GUIContent(string.Format("{0}: {1} -> {2}", _GetLocalization("Delete Link"), _tSelectNodes[0].name, ((GKToyNode)CurRenderData.nodeLst[_selectLink.next]).name)), false, _HandleMenuDeleteVirtualLink);
                        break;
                }
                menu.ShowAsContext();
                // 设置该事件被使用.
                Event.current.Use();
            }
        }

        protected void _HandleMenuAddNode(object userData)
        {
            _tmpSelectNodes.Clear();
            Vector2 mousePos = (Vector2)((object[])userData)[0];
            string key = (string)((object[])userData)[1];
            Assembly assem = GKToyMakerTypeManager.Instance().typeAssemblyDict[key];
            GKToyNode node = assem.CreateInstance(key, true, System.Reflection.BindingFlags.Default, null, new object[] { _GenerateGUID(CurNodeIdx++) }, null, null) as GKToyNode;
            node.className = key;
            node.pos.x = (mousePos.x) / Scale;
            node.pos.y = (mousePos.y) / Scale;
            _CreateNode(node);
        }
        // 创建节点组.
        protected void _HandleMenuCollapseNode(object userData)
        {
            Vector2 mousePos = (Vector2)userData;
            GKToyNodeGroup node = new GKToyNodeGroup(_GenerateGUID(CurNodeIdx++));
            node.pos.x = (mousePos.x) / Scale;
            node.pos.y = (mousePos.y) / Scale;
            _CreateNodeGroup(node);
        }
        // 重置节点.
        protected void _HandleMenuReset(object userData)
        {
            _ResetNode();
        }

        protected void _HandleMenuDeleteLink()
        {
            _RemoveLink(_selectNodes[0], (GKToyNode)CurRenderData.nodeLst[_selectLink.next]);
        }

        protected void _HandleMenuDeleteNode()
        {
            _RemoveNode(_selectNodes[0]);
        }
        // 删除组节点.
        protected void _HandleMenuDeleteGroup()
        {
            _RemoveGroup((GKToyNodeGroup)_selectNodes[0]);
        }
        // 删除组节点和所有子节点.
        protected void _HandleMenuDeleteGroupAndSubNodes()
        {
            _RemoveGroupAndSubNodes((GKToyNodeGroup)_selectNodes[0]);
        }
        // 删除组间链接.
        protected void _HandleMenuDeleteGroupLink()
        {
            _RemoveGroupLink((GKToyGroupLink)_selectNodes[0]);
        }
        // 删除所有选中节点.
        protected void _HandleMenuDeleteSelectedNodes()
        {
            _RemoveSelectNodes();
        }
        // 删除虚拟节点.
        protected void _HandleMenuDeleteVirtualLink()
        {
            if (NodeType.VirtualNode == _selectNodes[0].nodeType)
                _RemoveLinkAndVirtualLink((GKToyGroupLink)_selectNodes[0], _selectLink.next);
            else
                _RemoveLinkAndVirtualLink((GKToyGroupLink)CurRenderData.nodeLst[_selectLink.next], _selectLink.previous);
        }
        // 节点移出当前组.
        protected void _HandleMenuMoveOutGroup()
        {
            _MoveOutGroup(_curGroup, _selectNodes);
        }
        // 增加节点.
        protected virtual void _CreateNode(GKToyNode node)
        {
            node.id = node.ID;
            if (GKToyMakerTypeManager.Instance().typeAttributeDict.ContainsKey(node.className))
            {
                string[] paths = GKToyMakerTypeManager.Instance().typeAttributeDict[node.className][_language.ToString()].treePath.Split('/');
                if (paths.Length > 0)
                {
                    if (paths[0] == _GetLocalization("Action"))
                        node.nodeType = NodeType.Action;
                    else if (paths[0] == _GetLocalization("Condition"))
                        node.nodeType = NodeType.Condition;
                    else if (paths[0] == _GetLocalization("Decoration"))
                        node.nodeType = NodeType.Decoration;
                    node.name = string.Format("{0}-{1}", paths[paths.Length - 1], CurNodeIdx);
                }
                else
                {
                    Debug.LogError("Incorrect node path:" + node.className);
                }
            }
            node.Init(_overlord);
            // 图标被序列化后，在场景重载时无法加载，重载节点图标.
            _UpdateNodeIcon(node);
            node.comment = "";
            if (null != _curGroup)
                _curGroup.subNodes.Add(node.id);
            _newNodeLst.Add(node.id, node);
            _tmpSelectNodes.Add(node);
            _tmpAddDrawingNodes.Add(node);
        }
        // 加载节点图标.
        protected void _UpdateNodeIcon(GKToyNode node)
        {
            if (GKToyMakerTypeManager.Instance().iconAttributeDict.ContainsKey(node.className))
            {
                string iconPath = GKToyMakerTypeManager.Instance().iconAttributeDict[node.className].iconPath;
                node.icon = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture)) as Texture;
            }
            else
            {
                switch (node.nodeType)
                {
                    case NodeType.Action:
                        node.icon = ToyMakerBase._actionIcon;
                        break;
                    case NodeType.Condition:
                        node.icon = ToyMakerBase._conditionIcon;
                        break;
                    case NodeType.Decoration:
                        node.icon = ToyMakerBase._decorationIcon;
                        break;
                    case NodeType.Group:
                    case NodeType.VirtualNode:
                        node.icon = null;
                        break;
                    default:
                        node.icon = ToyMakerBase._defaultIcon;
                        break;
                }
            }

        }

        // 删除节点.
        protected virtual void _RemoveNode(GKToyNode node)
        {
            if (GKToyMakerTypeManager.Instance().hideNodes.Contains(node.className))
                return;
            if (CurRenderData.nodeLst.ContainsValue(node))
            {
                GKToyGroupLink inNode, outNode;
                foreach (GKToyNode n in CurRenderData.nodeLst.Values)
                {
                    if (NodeType.Group == n.nodeType)
                    {
                        if (((GKToyNodeGroup)n).subNodes.Contains(node.id))
                            ((GKToyNodeGroup)n).subNodes.Remove(node.id);
                        inNode = ((GKToyNodeGroup)n).FindVirtualInLinkFromSource(node.id);
                        outNode = ((GKToyNodeGroup)n).FindVirtualOutLinkFromSource(node.id);
                        if (null != inNode)
                        {
                            _RemoveGroupLink(inNode);
                        }
                        if (null != outNode)
                        {
                            _RemoveGroupLink(outNode);
                        }
                    }
                    else
                    {
                        Link l = n.FindLinkFromNode(node.id);
                        if (l != null)
                        {
                            _RemoveLink(n, node);
                        }
                    }
                }
                foreach (Link l in node.links)
                {
                    _RemoveLink(node, (GKToyNode)CurRenderData.nodeLst[l.next]);
                }
                if (!_removeNodeLst.Contains(node.id))
                    _removeNodeLst.Add(node.id);
                List<NewNode> newNodes = new List<NewNode>();
                foreach (var newLink in _newLinkLst)
                {
                    newNodes.AddRange(newLink.Value);
                    foreach (var newNode in newNodes)
                    {
                        if (newNode.node == node)
                            newLink.Value.Remove(newNode);
                    }
                    newNodes.Clear();
                }
            }
            _selectNodes.Clear();
        }
        // 删除选中节点.
        protected void _RemoveSelectNodes()
        {
            List<GKToyNode> deleteNodes = new List<GKToyNode>();
            deleteNodes.AddRange(_selectNodes);
            foreach (GKToyNode node in deleteNodes)
            {
                switch (node.nodeType)
                {
                    case NodeType.Group:
                        _RemoveGroupAndSubNodes((GKToyNodeGroup)node);
                        break;
                    case NodeType.VirtualNode:
                        _RemoveGroupLink((GKToyGroupLink)node);
                        break;
                    default:
                        _RemoveNode(node);
                        break;
                }
            }
        }
        // 删除组节点和所有子节点.
        protected void _RemoveGroupAndSubNodes(GKToyNodeGroup group)
        {
            List<int> subNodes = new List<int>();
            subNodes.AddRange(group.subNodes);
            _RemoveGroup(group);
            foreach (int nodeId in subNodes)
            {
                _RemoveNode((GKToyNode)CurRenderData.nodeLst[nodeId]);
            }
            _selectNodes.Clear();
        }
        // 重置节点.
        protected void _ResetNode()
        {
            CurRenderData.nodeLst.Clear();
            _tmpSelectNodes.Clear();
            _selectNodes.Clear();
            _selectLink = null;
            _drawingNodes.Clear();
            GKToyNode node = new GKToyStart(_GenerateGUID(CurNodeIdx++));
            Type type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = (_contentScrollPos.x + ToyMakerBase._minWidth * 0.5f) / Scale;
            node.pos.y = (_contentScrollPos.y + ToyMakerBase._minHeight * 0.5f) / Scale;
            _CreateNode(node);
            node = new GKToyEnd(_GenerateGUID(CurNodeIdx++));
            type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = -10;
            node.pos.y = -10;
            _CreateNode(node);
        }

        // 删除链接. 当前节点ID/目标节点ID.
        protected virtual void _RemoveLink(GKToyNode srcNode, GKToyNode destNode)
        {
            if (!_removeLinkLst.ContainsKey(srcNode))
                _removeLinkLst[srcNode] = new List<GKToyNode>();

            _removeLinkLst[srcNode].Add(destNode);
            _selectLink = null;
        }
        // 添加链接.
        protected virtual void _AddLink(GKToyNode srcNode, GKToyNode destNode, string paramKey)
        {
            if (_newLinkLst.ContainsKey(srcNode))
            {
                foreach (var node in _newLinkLst[srcNode])
                {
                    if (destNode == node.node)
                        return;
                }
            }
            else
                _newLinkLst[srcNode] = new List<NewNode>();
            NewNode newNode = new NewNode();
            newNode.node = destNode;
            newNode.parmKey = paramKey;
            _newLinkLst[srcNode].Add(newNode);
        }
        #endregion

        #region ExternalData
        // 更新数据源名称.
        protected void _UpdateDataNames()
        {
            int idx = 1;
            if (null == _overlord)
                return;
            _dataNameList.Clear();
            _dataNameList.Add("0. " + _overlord.internalData.data.name);
            foreach (var ed in _overlord.externalDatas)
            {
                if (null != ed)
                {
                    _dataNameList.Add(string.Format("{0}. {1}", idx, ed.data.name));
                }
                idx++;
            }
            // 外部数据所因为_curDataIdx-1.
            if (_curDataIdx > _overlord.externalDatas.Count)
                _curDataIdx = 0;
        }
        #endregion

        #region Group
        // 进入节点组子视图.
        protected void _SetDrawingNodes()
        {
            _drawingNodes.Clear();
            if (null == _curGroup)
            {
                _drawingNodes.AddRange(CurRenderData.nodeLst.Select(x => (GKToyNode)x.Value));
                foreach (GKToyNode otherNode in CurRenderData.nodeLst.Values)
                {
                    if (NodeType.Group == otherNode.nodeType)
                    {
                        GKToyNodeGroup group = (GKToyNodeGroup)otherNode;
                        foreach (int id in group.subNodes)
                        {
                            _drawingNodes.Remove((GKToyNode)CurRenderData.nodeLst[id]);
                        }
                        foreach (int id in group.groupLinkNodes)
                        {
                            _drawingNodes.Remove((GKToyNode)CurRenderData.nodeLst[id]);
                        }
                    }
                }
            }
            else
            {
                foreach (int id in _curGroup.subNodes)
                {
                    _drawingNodes.Add((GKToyNode)CurRenderData.nodeLst[id]);
                }
                foreach (int id in _curGroup.groupLinkNodes)
                {
                    _drawingNodes.Add((GKToyNode)CurRenderData.nodeLst[id]);
                }
            }
            _BackToCenter();
            _selectLink = null;
            _selectNodes.Clear();
            _tmpSelectNodes.Clear();
            _linkReCal = 1;
        }
        // 增加节点组.
        protected void _CreateNodeGroup(GKToyNodeGroup node)
        {
            node.Init(_overlord);
            node.data = CurRenderData;
            node.name = string.Format("Group-{0}", CurNodeIdx);
            Type type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.nodeType = NodeType.Group;
            node.subNodes.AddRange(_selectNodes.Select(x => x.id));
            node.id = node.ID;
            node.comment = "";
            _GenerateLinkForGroup(node, _selectNodes);
            _newNodeLst.Add(node.id, node);
            _tmpSelectNodes.Clear();
            _tmpSelectNodes.Add(node);
            _tmpAddDrawingNodes.Add(node);
            _linkReCal = 1;
        }
        // 增加连入虚拟节点.
        protected GKToyGroupLink _CreateGroupInLinkNode(int id, GKToyNodeGroup group, GKToyNode sourceNode, int otherGroupId = -1, int count = 0)
        {
            if (NodeType.Group == sourceNode.nodeType)
            {
                List<int> unlinkNodeIds = ((GKToyNodeGroup)sourceNode).GetUnlinkedInNodes(group);
                if (0 == unlinkNodeIds.Count)
                    return null;
                sourceNode = (GKToyNode)CurRenderData.nodeLst[unlinkNodeIds[0]];
            }
            int inLinkCount = group.GetAllInNodes().Count + count;
            GKToyGroupLink linkNode = group.AddGroupLink(id, sourceNode.id, true, otherGroupId);
            linkNode.Init(_overlord);
            linkNode.name = string.Format("{0}-{1}\n{2}->", _GetLocalization("External Link"), CurNodeIdx, sourceNode.name);
            linkNode.pos.x = _contentView.x + ToyMakerBase._layoutSpace + inLinkCount / (int)(_contentView.height / ToyMakerBase._nodeMinHeight) * ToyMakerBase._nodeMinWidth;
            linkNode.pos.y = _contentView.y + ToyMakerBase._layoutSpace + inLinkCount % (_contentView.height / ToyMakerBase._nodeMinHeight) * ToyMakerBase._nodeMinHeight;
            linkNode.outputObject = sourceNode.outputObject;
            _newGroupLinkLst.Add(id, linkNode);
            return linkNode;
        }
        // 增加连出虚拟节点.
        protected GKToyGroupLink _CreateGroupOutLinkNode(int id, GKToyNodeGroup group, GKToyNode sourceNode, int otherGroupId = -1, int count = 0)
        {
            if (NodeType.Group == sourceNode.nodeType)
            {
                List<int> unlinkNodeIds = ((GKToyNodeGroup)sourceNode).GetUnlinkedOutNodes(group);
                if (0 == unlinkNodeIds.Count)
                    return null;
                sourceNode = (GKToyNode)CurRenderData.nodeLst[unlinkNodeIds[0]];
            }
            int outLinkCount = group.GetAllOutNodes().Count + count;
            GKToyGroupLink linkNode = group.AddGroupLink(id, sourceNode.id, false, otherGroupId);
            linkNode.Init(_overlord);
            linkNode.name = string.Format("{0}-{1}\n->{2}", _GetLocalization("External Link"), CurNodeIdx, sourceNode.name);
            linkNode.pos.x = _contentView.xMax - ToyMakerBase._layoutSpace - ToyMakerBase._nodeMinWidth
                - outLinkCount / (int)(_contentView.height / ToyMakerBase._nodeMinHeight) * ToyMakerBase._nodeMinWidth;
            linkNode.pos.y = _contentView.y + ToyMakerBase._layoutSpace
                + outLinkCount % (_contentView.height / ToyMakerBase._nodeMinHeight) * ToyMakerBase._nodeMinHeight;
            _CopyParamFromNode(linkNode, sourceNode);
            _newGroupLinkLst.Add(id, linkNode);
            return linkNode;
        }
        /// <summary>
        /// 删除节点组
        /// </summary>
        /// <param name="group"></param>
        protected void _RemoveGroup(GKToyNodeGroup group)
        {
            GKToyNode subNode;
            GKToyGroupLink linkNode;
            GKToyGroupLink otherLinkNode;
            GKToyNodeGroup otherGroup;
            List<GKToyNodeGroup> otherGroups = new List<GKToyNodeGroup>();
            // 显示子节点.
            foreach (int subNodeId in group.subNodes)
            {
                subNode = (GKToyNode)CurRenderData.nodeLst[subNodeId];
                otherGroups.Clear();
                foreach (Link link in subNode.otherLinks)
                {
                    linkNode = (GKToyGroupLink)CurRenderData.nodeLst[link.next];
                    if (-1 != linkNode.otherGroupId)
                    {
                        otherGroup = (GKToyNodeGroup)CurRenderData.nodeLst[linkNode.otherGroupId];
                        _AddLink(subNode, otherGroup, string.Empty);
                        if (!otherGroups.Contains(otherGroup))
                            otherGroups.Add(otherGroup);
                    }
                    _RemoveLink(subNode, (GKToyNode)CurRenderData.nodeLst[link.next]);
                }
                foreach (GKToyNodeGroup grp in otherGroups)
                {
                    grp.FindVirtualInLinkFromSource(subNodeId).otherGroupId = -1;
                }
                _drawingNodes.Add((GKToyNode)CurRenderData.nodeLst[subNodeId]);
            }
            // 删除子虚拟节点.
            foreach (int linkNodeId in group.groupLinkNodes)
            {
                linkNode = (GKToyGroupLink)CurRenderData.nodeLst[linkNodeId];
                if (!_removeGroupLinkLst.Contains(linkNodeId))
                    _removeGroupLinkLst.Add(linkNodeId);
                if (GroupLinkType.LinkOut == linkNode.linkType)
                    continue;
                if (-1 != linkNode.otherGroupId)
                {
                    otherGroup = (GKToyNodeGroup)CurRenderData.nodeLst[linkNode.otherGroupId];
                    foreach (Link link in linkNode.links)
                    {
                        otherLinkNode = otherGroup.FindVirtualOutLinkFromSource(link.next);
                        if (null != otherLinkNode)
                        {
                            otherLinkNode.otherGroupId = -1;
                        }
                        _AddLink(otherGroup, (GKToyNode)CurRenderData.nodeLst[link.next], string.Empty);
                    }
                    _RemoveLink(otherGroup, group);
                }
                else
                {
                    _RemoveLink((GKToyNode)CurRenderData.nodeLst[linkNode.sourceNodeId], group);
                }
            }
            if (!_removeNodeLst.Contains(group.id))
                _removeNodeLst.Add(group.id);
            _selectNodes.Clear();
        }
        /// <summary>
        /// 删除组间连接节点
        /// </summary>
        /// <param name="groupLink"></param>
        protected void _RemoveGroupLink(GKToyGroupLink groupLink)
        {
            if (0 != groupLink.linkNodeIds.Count)
            {
                foreach (int nodeId in groupLink.linkNodeIds)
                    _RemoveLinkAndVirtualLink(groupLink, nodeId);
            }
            _RemoveEmptyVirtualNode(groupLink, GroupLinkType.LinkIn == groupLink.linkType);
            _selectNodes.Clear();
        }
        // 删除节点所有链接.
        protected void _RemoveNodeLinks(GKToyNode node, string parmKey = "")
        {
            foreach (GKToyNode n in CurRenderData.nodeLst.Values)
            {
                Link l = n.FindLinkFromNode(node.id);

                // 检测链接是否为空.
                if (l == null)
                    continue;

                // 检测链接是否为目标参数链接.
                if (!string.IsNullOrEmpty(parmKey) && l.parmKey != parmKey)
                    continue;

                _RemoveLink(n, (GKToyNode)CurRenderData.nodeLst[l.next]);
            }
            Repaint();
        }
        /// <summary>
        /// 为新建的节点组生成虚拟节点和链接
        /// </summary>
        /// <param name="node">新节点组</param>
        protected void _GenerateLinkForGroup(GKToyNodeGroup node, List<GKToyNode> nodes)
        {
            int inCount = 0;
            int outCount = 0;
            GKToyGroupLink newLinkNode;
            GKToyNode linkNode;
            Dictionary<GKToyNode, GKToyGroupLink> newInNode = new Dictionary<GKToyNode, GKToyGroupLink>();
            Dictionary<GKToyNode, GKToyGroupLink> newOutNode = new Dictionary<GKToyNode, GKToyGroupLink>();
            foreach (int groupLinkId in node.groupLinkNodes)
            {
                newLinkNode = (GKToyGroupLink)CurRenderData.nodeLst[groupLinkId];
                if (GroupLinkType.LinkIn == newLinkNode.linkType)
                    newInNode.Add((GKToyNode)CurRenderData.nodeLst[newLinkNode.sourceNodeId], newLinkNode);
                else
                    newOutNode.Add((GKToyNode)CurRenderData.nodeLst[newLinkNode.sourceNodeId], newLinkNode);
            }
            foreach (GKToyNode subNode in nodes)
            {
                // 内部节点到外部节点的链接.
                foreach (Link link in subNode.links)
                {
                    linkNode = (GKToyNode)CurRenderData.nodeLst[link.next];
                    if (node.subNodes.Contains(link.next) || !_drawingNodes.Contains(linkNode))
                        continue;
                    if (newOutNode.ContainsKey(linkNode))
                    {
                        _AddLink(subNode, newOutNode[linkNode], link.parmKey);
                    }
                    else
                    {
                        newLinkNode = _CreateGroupOutLinkNode(_GenerateGUID(CurNodeIdx++), node, linkNode, -1, outCount++);
                        _AddLink(subNode, newLinkNode, link.parmKey);
                        _AddLink(node, linkNode, string.Empty);
                        newOutNode.Add(linkNode, newLinkNode);
                    }
                }
                foreach (Link otherLink in subNode.otherLinks)
                    _RemoveLink(subNode, (GKToyNode)CurRenderData.nodeLst[otherLink.next]);
                _drawingNodes.Remove(subNode);
            }
            List<GKToyGroupLink> inLinkNodes, outLinkNodes;
            foreach (GKToyNode otherNode in _drawingNodes)
            {
                if (node.subNodes.Contains(otherNode.id) || node == otherNode)
                    continue;
                if (NodeType.Group == otherNode.nodeType)
                {
                    GKToyNodeGroup otherGroup = (GKToyNodeGroup)otherNode;
                    otherGroup.FindGroupLinkFromNodes(node.subNodes, out inLinkNodes, out outLinkNodes);
                    // 内部节点到外部组.
                    if (0 != inLinkNodes.Count)
                    {
                        foreach (GKToyGroupLink groupLink in inLinkNodes)
                        {
                            foreach (Link link in groupLink.links)
                            {
                                linkNode = (GKToyNode)CurRenderData.nodeLst[link.next];
                                if (newOutNode.ContainsKey(linkNode))
                                {
                                    _AddLink((GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId], newOutNode[linkNode], link.parmKey);
                                }
                                else
                                {
                                    newLinkNode = _CreateGroupOutLinkNode(_GenerateGUID(CurNodeIdx++), node, linkNode, otherGroup.id, outCount++);
                                    _AddLink((GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId], newLinkNode, link.parmKey);
                                    newOutNode.Add(linkNode, newLinkNode);
                                }
                            }
                            groupLink.otherGroupId = node.id;
                        }
                        _AddLink(node, otherGroup, string.Empty);
                    }
                    // 外部组到内部结点.
                    if (0 != outLinkNodes.Count)
                    {
                        foreach (GKToyGroupLink groupLink in outLinkNodes)
                        {
                            foreach (int nodeId in groupLink.linkNodeIds)
                            {
                                linkNode = (GKToyNode)CurRenderData.nodeLst[nodeId];
                                if (newInNode.ContainsKey(linkNode))
                                {
                                    _AddLink(newInNode[linkNode], (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId], linkNode.FindLinkFromNode(groupLink.sourceNodeId).parmKey);
                                }
                                else
                                {
                                    newLinkNode = _CreateGroupInLinkNode(_GenerateGUID(CurNodeIdx++), node, linkNode, otherGroup.id, inCount++);
                                    _AddLink(newLinkNode, (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId], linkNode.FindLinkFromNode(groupLink.sourceNodeId).parmKey);
                                    newInNode.Add(linkNode, newLinkNode);
                                }
                            }
                            _RemoveLink(otherGroup, (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId]);
                            groupLink.otherGroupId = node.id;
                        }
                        _AddLink(otherGroup, node, string.Empty);
                    }
                }
                else
                {
                    Link link;
                    bool hasLink = false;
                    // 外部节点到内部节点.
                    foreach (GKToyNode subNode in nodes)
                    {
                        link = otherNode.FindLinkFromNode(subNode.id);
                        if (null == link)
                            continue;
                        if (newInNode.ContainsKey(otherNode))
                        {
                            _AddLink(newInNode[otherNode], (GKToyNode)CurRenderData.nodeLst[link.next], link.parmKey);
                        }
                        else
                        {
                            newLinkNode = _CreateGroupInLinkNode(_GenerateGUID(CurNodeIdx++), node, otherNode, -1, inCount++);
                            _AddLink(newLinkNode, (GKToyNode)CurRenderData.nodeLst[link.next], link.parmKey);
                            newInNode.Add(otherNode, newLinkNode);
                        }
                        hasLink = true;
                    }
                    if (hasLink)
                        _AddLink(otherNode, node, string.Empty);
                }
            }
        }
        /// <summary>
        /// 为虚拟节点生成所连接尾节点的参数
        /// </summary>
        /// <param name="groupLink"></param>
        /// <param name="node"></param>
        protected void _CopyParamFromNode(GKToyGroupLink groupLink, GKToyNode node)
        {
            if (node != null)
            {
                groupLink.ioStates = node.ioStates;
                groupLink.props = node.props;
                groupLink.propStates = node.propStates;
                groupLink.parmRect = new Dictionary<string, NodeParm>();
                for (int i = 0; i < groupLink.props.Length; i++)
                {
                    NodeParm np = new NodeParm();
                    np.propretyInfo = groupLink.props[i];
                    np.rect = Rect.zero;
                    groupLink.parmRect.Add(groupLink.props[i].Name, np);
                }
            }
            else
            {
                groupLink.ioStates = null;
                groupLink.props = null;
                groupLink.propStates = null;
                groupLink.parmRect = null;
            }
        }
        /// <summary>
        /// 删除虚拟链接和其对应的逻辑链接
        /// </summary>
        /// <param name="groupLink">虚拟节点Id</param>
        /// <param name="nodeId">连接到的节点Id</param>
        protected void _RemoveLinkAndVirtualLink(GKToyGroupLink groupLink, int nodeId)
        {
            if (GroupLinkType.LinkOut == groupLink.linkType)
            {
                GKToyNode firstNode = (GKToyNode)CurRenderData.nodeLst[nodeId];
                GKToyNode lastNode = (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId];
                _RemoveLink(firstNode, lastNode);
                _RemoveLink(firstNode, groupLink);
                if (-1 != groupLink.otherGroupId)
                    _RemoveLink(((GKToyNodeGroup)CurRenderData.nodeLst[groupLink.otherGroupId]).FindVirtualInLinkFromSource(firstNode.id), lastNode);
            }
            else
            {
                GKToyNode firstNode = (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId];
                GKToyNode lastNode = (GKToyNode)CurRenderData.nodeLst[nodeId];
                _RemoveLink(firstNode, lastNode);
                _RemoveLink(groupLink, lastNode);
                if (-1 != groupLink.otherGroupId)
                    _RemoveLink(firstNode, ((GKToyNodeGroup)CurRenderData.nodeLst[groupLink.otherGroupId]).FindVirtualOutLinkFromSource(lastNode.id));
            }
        }
        protected void _RemoveLinkAndVirtualLink(Link link)
        {
            GKToyNode previousNode = (GKToyNode)CurRenderData.nodeLst[link.previous];
            GKToyNode nextNode = (GKToyNode)CurRenderData.nodeLst[link.next];
            // 连入虚拟连线.
            if (NodeType.VirtualNode == previousNode.nodeType)
            {
                _RemoveLinkAndVirtualLink((GKToyGroupLink)previousNode, nextNode.id);
            }
            // 连出虚拟连线.
            else if (NodeType.VirtualNode == nextNode.nodeType)
            {
                _RemoveLinkAndVirtualLink((GKToyGroupLink)nextNode, previousNode.id);
            }
            // Inspector删除跨组逻辑连接.
            else if (!_drawingNodes.Contains(nextNode))
            {

                foreach (GKToyNode otherNode in CurRenderData.nodeLst.Values)
                {
                    if (NodeType.Group == otherNode.nodeType && ((GKToyNodeGroup)otherNode).subNodes.Contains(nextNode.id))
                    {
                        GKToyGroupLink groupLink = ((GKToyNodeGroup)otherNode).FindVirtualInLinkFromSource(previousNode.id);
                        if (null != groupLink)
                        {
                            _RemoveLinkAndVirtualLink(groupLink, nextNode.id);
                            break;
                        }
                    }
                }
            }
            else
                _RemoveLink(previousNode, nextNode);
        }
        /// <summary>
        /// 切换虚拟节点的源节点
        /// </summary>
        /// <param name="groupLink">要切换的虚拟节点</param>
        /// <param name="newSourceNode">新的源节点</param>
        protected void _ChangeVirtualSourceNode(GKToyGroupLink groupLink, GKToyNode newSourceNode)
        {
            foreach (int linkId in groupLink.linkNodeIds)
            {
                _RemoveLinkAndVirtualLink(groupLink, linkId);
            }
            groupLink.sourceNodeId = newSourceNode.id;
            if (GroupLinkType.LinkIn == groupLink.linkType)
            {
                groupLink.name = string.Format("{0}\n{1}->", groupLink.name.Substring(0, groupLink.name.IndexOf("\n")), newSourceNode.name);
                groupLink.outputObject = newSourceNode.outputObject;
            }
            else
            {
                groupLink.name = string.Format("{0}\n->{1}", groupLink.name.Substring(0, groupLink.name.IndexOf("\n")), newSourceNode.name);
                _CopyParamFromNode(groupLink, newSourceNode);
            }
        }
        /// <summary>
        /// 加载节点时，更新虚拟节点的参数
        /// </summary>
        /// <param name="groupLink">虚拟节点</param>
        /// <param name="sourceNode">源节点</param>
        protected void _UpdateVirtualParam(GKToyGroupLink groupLink, GKToyNode sourceNode)
        {
            if (GroupLinkType.LinkIn == groupLink.linkType)
            {
                groupLink.outputObject = sourceNode.outputObject;
            }
            else
            {
                _CopyParamFromNode(groupLink, sourceNode);
            }
        }
        /// <summary>
        /// 将选中节点移出节点组
        /// </summary>
        /// <param name="group">节点组</param>
        /// <param name="nodes">选中节点列表</param>
        protected void _MoveOutGroup(GKToyNodeGroup group, List<GKToyNode> nodes)
        {
            int outCount = 0;
            int inCount = 0;
            GKToyGroupLink newLinkNode;
            GKToyNode linkedNode;
            GKToyNode subNode;
            Dictionary<GKToyNode, GKToyGroupLink> newInNode = new Dictionary<GKToyNode, GKToyGroupLink>();
            Dictionary<GKToyNode, GKToyGroupLink> newOutNode = new Dictionary<GKToyNode, GKToyGroupLink>();
            // 选中节点到其他组内节点.
            foreach (GKToyNode node in nodes)
            {
                foreach (Link link in node.otherLinks)
                    _RemoveLink(node, (GKToyNode)CurRenderData.nodeLst[link.next]);
                foreach (Link link in node.links)
                {
                    linkedNode = (GKToyNode)CurRenderData.nodeLst[link.next];
                    if (group.subNodes.Contains(link.next) && !nodes.Contains(linkedNode))
                    {
                        if (newInNode.ContainsKey(node))
                        {
                            _AddLink(newInNode[node], linkedNode, link.parmKey);
                        }
                        else
                        {
                            newLinkNode = _CreateGroupInLinkNode(_GenerateGUID(CurNodeIdx++), group, node, -1, inCount++);
                            _tmpAddDrawingNodes.Add(newLinkNode);
                            _AddLink(newLinkNode, linkedNode, link.parmKey);
                            _AddLink(node, group, string.Empty);
                            newInNode.Add(node, newLinkNode);
                        }
                    }
                }
                group.subNodes.Remove(node.id);
                _drawingNodes.Remove(node);
            }
            // 其他组内节点到选中节点.
            foreach (int subNodeId in group.subNodes)
            {
                subNode = (GKToyNode)CurRenderData.nodeLst[subNodeId];
                foreach (Link link in subNode.links)
                {
                    linkedNode = (GKToyNode)CurRenderData.nodeLst[link.next];
                    if (nodes.Contains(linkedNode))
                    {
                        if (newOutNode.ContainsKey(linkedNode))
                        {
                            _AddLink(subNode, newOutNode[linkedNode], link.parmKey);
                        }
                        else
                        {
                            newLinkNode = _CreateGroupOutLinkNode(_GenerateGUID(CurNodeIdx++), group, linkedNode, -1, outCount++);
                            _tmpAddDrawingNodes.Add(newLinkNode);
                            _AddLink(subNode, newLinkNode, link.parmKey);
                            _AddLink(group, linkedNode, string.Empty);
                            newOutNode.Add(linkedNode, newLinkNode);
                        }
                    }
                }
            }
            // 处理虚拟节点和组外的连接.
            bool delete;
            List<GKToyNode> addLinkNodes = new List<GKToyNode>();
            GKToyGroupLink groupLink;
            GKToyNodeGroup otherGroup;
            foreach (int virtualNodeId in group.groupLinkNodes)
            {
                delete = true;
                addLinkNodes.Clear();
                groupLink = (GKToyGroupLink)CurRenderData.nodeLst[virtualNodeId];
                if (GroupLinkType.LinkOut == groupLink.linkType)
                {
                    foreach (int nodeId in groupLink.linkNodeIds)
                    {
                        if (!nodes.Contains((GKToyNode)CurRenderData.nodeLst[nodeId]))
                            delete = false;
                        else
                            addLinkNodes.Add((GKToyNode)CurRenderData.nodeLst[nodeId]);
                    }
                    if (-1 != groupLink.otherGroupId && 0 != addLinkNodes.Count)
                    {
                        otherGroup = (GKToyNodeGroup)CurRenderData.nodeLst[groupLink.otherGroupId];
                        foreach (GKToyNode node in addLinkNodes)
                        {
                            _AddLink(node, otherGroup, string.Empty);
                            otherGroup.FindVirtualInLinkFromSource(node.id).otherGroupId = -1;
                        }
                    }
                }
                else
                {
                    foreach (Link link in groupLink.links)
                    {
                        if (!nodes.Contains((GKToyNode)CurRenderData.nodeLst[link.next]))
                            delete = false;
                        else
                        {
                            _RemoveLink(groupLink, (GKToyNode)CurRenderData.nodeLst[link.next]);
                            addLinkNodes.Add((GKToyNode)CurRenderData.nodeLst[link.next]);
                        }
                        if (-1 != groupLink.otherGroupId && 0 != addLinkNodes.Count)
                        {
                            otherGroup = (GKToyNodeGroup)CurRenderData.nodeLst[groupLink.otherGroupId];
                            foreach (GKToyNode node in addLinkNodes)
                            {
                                _AddLink(otherGroup, node, string.Empty);
                                otherGroup.FindVirtualOutLinkFromSource(node.id).otherGroupId = -1;
                            }
                        }
                    }
                }
                // 删除已空的虚拟节点.
                if (delete)
                    _RemoveEmptyVirtualNode(groupLink, GroupLinkType.LinkOut == groupLink.linkType, nodes.Select(x => x.id).ToList());
            }
            if (0 == group.subNodes.Count)
            {
                _curGroup = null;
                _RemoveGroup(group);
                _SetDrawingNodes();
            }
        }
        /// <summary>
        /// 将选中节点移入节点组
        /// </summary>
        /// <param name="group">节点组</param>
        /// <param name="nodes">选中节点列表</param>
        protected void _MoveInGroup(GKToyNodeGroup group, List<GKToyNode> nodes)
        {
            GKToyGroupLink groupLink;
            foreach (int virtualNodeId in group.groupLinkNodes)
            {
                groupLink = (GKToyGroupLink)CurRenderData.nodeLst[virtualNodeId];
                if (nodes.Contains((GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId]))
                {
                    if (GroupLinkType.LinkOut == groupLink.linkType)
                    {
                        foreach (int nodeId in groupLink.linkNodeIds)
                        {
                            _RemoveLink((GKToyNode)CurRenderData.nodeLst[nodeId], groupLink);
                        }
                    }
                    _RemoveEmptyVirtualNode(groupLink, GroupLinkType.LinkOut == groupLink.linkType);
                }
            }
            group.subNodes.AddRange(nodes.Select(x => x.id).ToList());
            _GenerateLinkForGroup(group, nodes);
        }
        /// <summary>
        /// 移除空的虚拟节点和相应的外部连接
        /// </summary>
        /// <param name="groupLink">空虚拟节点</param>
        /// <param name="isIn">需要移除的外部组链接是入还是出</param>
        /// <param name="exceptionNodes">即将被一起移除的其他节点，用于判断外部连接是否需要被删除</param>
        protected void _RemoveEmptyVirtualNode(GKToyGroupLink groupLink, bool isIn, List<int> exceptionNodes)
        {
            if (-1 != groupLink.otherGroupId)
            {
                GKToyNodeGroup otherGroup = (GKToyNodeGroup)CurRenderData.nodeLst[groupLink.otherGroupId];
                bool hasLink = false;
                GKToyGroupLink otherLink;
                foreach (int id in otherGroup.groupLinkNodes)
                {
                    otherLink = (GKToyGroupLink)CurRenderData.nodeLst[id];
                    if (null != exceptionNodes && exceptionNodes.Contains(otherLink.sourceNodeId))
                        continue;
                    if (GroupLinkType.LinkOut == otherLink.linkType ^ isIn && otherLink.otherGroupId == groupLink.groupId)
                    {
                        hasLink = true;
                        break;
                    }
                }
                if (!hasLink)
                {
                    if (GroupLinkType.LinkOut == groupLink.linkType)
                        _RemoveLink((GKToyNodeGroup)CurRenderData.nodeLst[groupLink.groupId], otherGroup);
                    else
                        _RemoveLink(otherGroup, (GKToyNodeGroup)CurRenderData.nodeLst[groupLink.groupId]);
                }
            }
            else if (GroupLinkType.LinkIn == groupLink.linkType)
                _RemoveLink((GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId], (GKToyNode)CurRenderData.nodeLst[groupLink.groupId]);
            else
                _RemoveLink((GKToyNode)CurRenderData.nodeLst[groupLink.groupId], (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId]);
            if (!_removeGroupLinkLst.Contains(groupLink.id))
                _removeGroupLinkLst.Add(groupLink.id);
        }
        protected void _RemoveEmptyVirtualNode(GKToyGroupLink groupLink, bool isIn)
        {
            if (-1 != groupLink.otherGroupId)
            {
                GKToyNodeGroup otherGroup = (GKToyNodeGroup)CurRenderData.nodeLst[groupLink.otherGroupId];
                bool hasLink = false;
                GKToyGroupLink otherLink;
                foreach (int id in otherGroup.groupLinkNodes)
                {
                    otherLink = (GKToyGroupLink)CurRenderData.nodeLst[id];
                    if (GroupLinkType.LinkIn == otherLink.linkType ^ isIn && otherLink.otherGroupId == groupLink.groupId
                        && 1 == otherLink.linkNodeIds.Count && groupLink.sourceNodeId == otherLink.linkNodeIds[0])
                    {
                        hasLink = true;
                        break;
                    }
                }
                if (!hasLink)
                {
                    if (GroupLinkType.LinkOut == groupLink.linkType)
                        _RemoveLink((GKToyNodeGroup)CurRenderData.nodeLst[groupLink.groupId], otherGroup);
                    else
                        _RemoveLink(otherGroup, (GKToyNodeGroup)CurRenderData.nodeLst[groupLink.groupId]);
                }
            }
            else if (GroupLinkType.LinkIn == groupLink.linkType)
                _RemoveLink((GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId], (GKToyNode)CurRenderData.nodeLst[groupLink.groupId]);
            else
                _RemoveLink((GKToyNode)CurRenderData.nodeLst[groupLink.groupId], (GKToyNode)CurRenderData.nodeLst[groupLink.sourceNodeId]);
            if (!_removeGroupLinkLst.Contains(groupLink.id))
                _removeGroupLinkLst.Add(groupLink.id);
        }
        #endregion

        #region HotKeys
        /// <summary>
        /// 自定义快捷键
        /// </summary>
        protected void _CheckHotKeys()
        {
            if (Event.current.shift && Event.current.alt)
            {
                switch (Event.current.keyCode)
                {
                    // 保存.
                    case KeyCode.S:
                        if (_overlord != null)
                        {
                            Event.current.Use();
                            _ResetScaleData();
                            _overlord.Save();
                            _overlord.Backup();
                            ShowNotification(new GUIContent(_GetLocalization("Data saved")));
                        }
                        Event.current.Use();
                        break;
                    // 复制.
                    case KeyCode.D:
                        if (_overlord != null)
                        {
                            Event.current.Use();
                            _CopyNodes(_selectNodes);
                        }
                        break;
                    // 删除.
                    case KeyCode.T:
                        Event.current.Use();
                        if (EditorUtility.DisplayDialog(_GetLocalization("Confirmation"), _GetLocalization("Are you sure to delete selected node?"), _GetLocalization("OK"), _GetLocalization("Cancel")))
                            _RemoveSelectNodes();
                        break;
                }
            }
        }
        /// <summary>
        /// 复制.
        /// </summary>
        /// <param name="nodes"></param>
        protected void _CopyNodes(List<GKToyNode> nodes)
        {
            GKToyNode cloneNode, cloneSubNode;
            Dictionary<int, GKToyNode> cloneNodeMatch = new Dictionary<int, GKToyNode>();
            _tmpSelectNodes.Clear();
            // 复制节点和组.
            foreach (GKToyNode node in nodes)
            {
                if (NodeType.VirtualNode == node.nodeType || GKToyMakerTypeManager.Instance().hideNodes.Contains(node.className))
                    continue;
                if (NodeType.Group == node.nodeType)
                {
                    cloneNode = _CloneGroup((GKToyNodeGroup)node);
                    foreach (int subId in ((GKToyNodeGroup)node).subNodes)
                    {
                        if (GKToyMakerTypeManager.Instance().hideNodes.Contains(node.className))
                            continue;
                        cloneSubNode = _CloneNode((GKToyNode)CurRenderData.nodeLst[subId], 0);
                        ((GKToyNodeGroup)cloneNode).subNodes.Add(cloneSubNode.id);
                        cloneNodeMatch.Add(subId, cloneSubNode);
                        _tmpAddDrawingNodes.Remove(cloneSubNode);
                        _tmpSelectNodes.Remove(cloneSubNode);
                    }
                }
                else
                {
                    cloneNode = _CloneNode(node, 1);
                }
                cloneNodeMatch.Add(node.id, cloneNode);
            }
            // 复制虚拟节点.
            GKToyGroupLink groupLink;
            foreach (GKToyNode node in nodes)
            {
                if (NodeType.Group != node.nodeType)
                    continue;
                foreach (int linkId in ((GKToyNodeGroup)node).groupLinkNodes)
                {
                    groupLink = (GKToyGroupLink)CurRenderData.nodeLst[linkId];
                    if (cloneNodeMatch.ContainsKey(groupLink.sourceNodeId))
                    {
                        if (-1 == groupLink.otherGroupId)
                        {
                            cloneSubNode = _CloneVirtualNode(groupLink, (GKToyNodeGroup)cloneNodeMatch[node.id],
                                cloneNodeMatch[groupLink.sourceNodeId], -1);
                            cloneNodeMatch.Add(linkId, cloneSubNode);
                        }
                        else if (cloneNodeMatch.ContainsKey(groupLink.otherGroupId))
                        {
                            cloneSubNode = _CloneVirtualNode(groupLink, (GKToyNodeGroup)cloneNodeMatch[node.id],
                                cloneNodeMatch[groupLink.sourceNodeId], cloneNodeMatch[groupLink.otherGroupId].id);
                            cloneNodeMatch.Add(linkId, cloneSubNode);
                        }
                    }
                }
            }
            // 复制链接.
            foreach (int nodeId in cloneNodeMatch.Keys)
            {
                cloneNode = (GKToyNode)CurRenderData.nodeLst[nodeId];
                foreach (Link link in cloneNode.links)
                {
                    if (cloneNodeMatch.ContainsKey(link.next))
                        _AddLink(cloneNodeMatch[nodeId], cloneNodeMatch[link.next], link.parmKey);
                }
                foreach (Link link in cloneNode.otherLinks)
                {
                    if (cloneNodeMatch.ContainsKey(link.next))
                        _AddLink(cloneNodeMatch[nodeId], cloneNodeMatch[link.next], link.parmKey);
                }
            }
        }
        /// <summary>
        /// 复制节点
        /// </summary>
        /// <param name="node">要复制的节点</param>
        /// <param name="offset">位置偏移倍数</param>
        /// <returns></returns>
        protected GKToyNode _CloneNode(GKToyNode node, int offset)
        {
            Assembly assem = GKToyMakerTypeManager.Instance().typeAssemblyDict[node.className];
            GKToyNode cloneNode = assem.CreateInstance(node.className, true, System.Reflection.BindingFlags.Default, null, new object[] { _GenerateGUID(CurNodeIdx++) }, null, null) as GKToyNode;
            cloneNode.className = node.className;
            cloneNode.pos.x = node.pos.x + node.width * offset;
            cloneNode.pos.y = node.pos.y + node.height * offset;
            _CreateNode(cloneNode);
            cloneNode.comment = node.comment;
            for (int i = 0; i < node.propStates.Length; ++i)
                cloneNode.propStates[i] = node.propStates[i];
            for (int i = 0; i < node.ioStates.Length; ++i)
                cloneNode.ioStates[i] = node.ioStates[i];
            PropertyInfo[] props = cloneNode.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo prop in props)
            {
                prop.SetValue(cloneNode, prop.GetValue(node, null), null);
            }
            return cloneNode;
        }
        // 复制组.
        protected GKToyNode _CloneGroup(GKToyNodeGroup group)
        {
            GKToyNodeGroup cloneGroup = new GKToyNodeGroup(_GenerateGUID(CurNodeIdx++));
            cloneGroup.pos.x = group.pos.x + group.width;
            cloneGroup.pos.y = group.pos.y + group.height;
            cloneGroup.Init(_overlord);
            cloneGroup.data = CurRenderData;
            cloneGroup.name = string.Format("Group-{0}", CurNodeIdx);
            Type type = cloneGroup.GetType();
            cloneGroup.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            cloneGroup.nodeType = NodeType.Group;
            cloneGroup.id = cloneGroup.ID;
            _newNodeLst.Add(cloneGroup.id, cloneGroup);
            _tmpSelectNodes.Add(cloneGroup);
            _tmpAddDrawingNodes.Add(cloneGroup);
            cloneGroup.comment = group.comment;
            return cloneGroup;
        }
        // 复制虚拟节点.
        protected GKToyNode _CloneVirtualNode(GKToyGroupLink groupLink, GKToyNodeGroup group, GKToyNode sourceNode, int otherGroupId)
        {
            GKToyGroupLink cloneLink;
            if (GroupLinkType.LinkIn == groupLink.linkType)
            {
                cloneLink = group.AddGroupLink(_GenerateGUID(CurNodeIdx++), sourceNode.id, true, otherGroupId);
                cloneLink.name = string.Format("{0}-{1}\n{2}->", _GetLocalization("External Link"), CurNodeIdx, sourceNode.name);
                cloneLink.outputObject = sourceNode.outputObject;
            }
            else
            {
                cloneLink = group.AddGroupLink(_GenerateGUID(CurNodeIdx++), sourceNode.id, false, otherGroupId);
                cloneLink.name = string.Format("{0}-{1}\n->{2}", _GetLocalization("External Link"), CurNodeIdx, sourceNode.name);
                _CopyParamFromNode(cloneLink, sourceNode);
            }
            cloneLink.Init(_overlord);
            _newGroupLinkLst.Add(cloneLink.id, cloneLink);
            cloneLink.pos.x = groupLink.pos.x;
            cloneLink.pos.y = groupLink.pos.y;
            return cloneLink;
        }
        #endregion

        #region Other
        // 初始化.
        // 初始化数据必须需要再Enable中执行.
        protected void _Init()
        {
            if (null != _overlord)
            {
                _overlord = null;
                _ResetSelected(_overlord);
            }
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += _SaveDataWhenPlayModeChange;
#else
            EditorApplication.playmodeStateChanged += SaveDataWhenPlayModeChange;
#endif
            // 数据备份.
            _lastColor = GUI.color; ;
            _lastBgColor = GUI.backgroundColor;

            // 数据导入.
            //ToyMakerBase = ToyMakerBase;

            // 数据计算.
            ToyMakerBase._commentContentMargin = ToyMakerBase._commentStyle.padding.left
                                                + ToyMakerBase._commentStyle.padding.right
                                                + ToyMakerBase._commentStyle.margin.left
                                                + ToyMakerBase._commentStyle.margin.right
                                                + ToyMakerBase._commentStyle.contentOffset.x;


            _contentView = new Rect(ToyMakerBase._informationWidth + ToyMakerBase._layoutSpace * 3,
                                    ToyMakerBase._lineHeight * 2 + ToyMakerBase._layoutSpace,
                                    ToyMakerBase._minWidth - ToyMakerBase._informationWidth - ToyMakerBase._layoutSpace * 4,
                                    ToyMakerBase._minHeight - ToyMakerBase._lineHeight * 2 - ToyMakerBase._layoutSpace * 3);

            _nonContentRect = new Rect((ToyMakerBase._minWidth - 206) * 0.5f, (ToyMakerBase._minHeight - 100) * 0.5f, 206, 100);

            // 子类树生成.
            root = TreeNode.Get().GenerateFileTree(GKToyMakerTypeManager.Instance().typeAttributeDict, _language.ToString(), GKToyMakerTypeManager.Instance().hideNodes);

            // 初始化变量类型.
            _variableType.Clear();
            foreach (var t in typeof(GKToyVariable).Assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof(GKToyVariable)))
                {
                    _variableType.Add(t);
                }
            }
            // 移除ShardVariable & ShardLstVariable类.
            _variableType.RemoveAt(0);
            _variableType.RemoveAt(0);

            _variableTypeNames = GK.TypesToString(_variableType.ToArray());
            for (int i = 0; i < _variableTypeNames.Length; i++)
            {
                _variableTypeNames[i] = _variableTypeNames[i].Split('.')[1];
            }
        }

        // 序列化点选对象变更.
        protected void _SelectChanged()
        {
            if (!_isLocking)
            {
                var assets = Selection.GetFiltered(typeof(GKToyBaseOverlord), SelectionMode.Assets);
                if (0 == assets.Length)
                    return;

                if (null == _overlord || assets[0] != _overlord)
                {
                    _ResetSelected((GKToyBaseOverlord)assets[0]);
                    Repaint();
                }
            }
        }

        // 重置选择.
        protected void _ResetSelected(GKToyBaseOverlord target)
        {
            if (null != _overlord)
            {
                _ResetScaleData();
                _overlord.Save();
            }
            _overlord = target;
            _selectNodes.Clear();
            _selectLink = null;
            _drawingNodes.Clear();
            _curGroup = null;
            Scale = 1;
            _contentScrollPos = Vector2.zero;
            if (null != _overlord)
            {
                CurNodeIdx = CurRenderData.nodeGuid;
                CurLinkIdx = CurRenderData.linkGuid;
                if (!Application.isPlaying)
                {
                    _overlord.internalData.data.Init(_overlord);
                    foreach (var ed in _overlord.externalDatas)
                    {
                        if (null != ed && null != ed.data)
                            ed.data.Init(_overlord);
                    }
                    foreach (GKToyNode node in CurRenderData.nodeLst.Values)
                    {
                        _UpdateNodeIcon(node);
                        if (NodeType.VirtualNode == node.nodeType)
                            _UpdateVirtualParam((GKToyGroupLink)node, (GKToyNode)CurRenderData.nodeLst[((GKToyGroupLink)node).sourceNodeId]);
                    }
                }
                _SetDrawingNodes();
            }
            _UpdateDataNames();
        }
        /// <summary>
        /// 关闭数据前，计算Scale为1时的节点和链接位置
        /// </summary>
        protected void _ResetScaleData()
        {
            if (1 == Scale)
                return;
            Scale = 1;
            _curGroup = null;
            _SetDrawingNodes();
            foreach (GKToyNode node in _drawingNodes)
            {
                // 计算Node宽高.
                int w = name.Length * ToyMakerBase._charWidth + 4;
                if (w <= ToyMakerBase._nodeMinWidth)
                    w = ToyMakerBase._nodeMinWidth;
                node.width = w;
                node.height = ToyMakerBase._nodeMinHeight;
                // Right.
                _tmpRect.width = 10;
                _tmpRect.height = (node.height - 24);
                _tmpRect.x = (node.width + node.pos.x - 6);
                _tmpRect.y = (node.height * 0.5f + node.pos.y) - _tmpRect.height * 0.5f;
                node.outputRect = _tmpRect;
                // Left.
                _tmpRect.x = (node.pos.x - 6);
                node.inputRect = _tmpRect;
                // Bg.
                _tmpRect.width = node.width;
                _tmpRect.height = node.height;
                _tmpRect.x = node.pos.x;
                _tmpRect.y = node.pos.y;
                node.rect = _tmpRect;
            }
            _UpdateAllLinks();
        }

        // 创建实例.
        virtual public GKToyBaseOverlord _CreateData(string path)
        {
            // 生成数据源.
            var obj = ScriptableObject.CreateInstance<GKToyExternalData>();
            GKToyData tData = new GKToyData();
            obj.data = tData.Clone() as GKToyData;
            GKEditor.CreateAsset(obj, path);
            var externalData = AssetDatabase.LoadMainAssetAtPath(path) as GKToyExternalData;
            if (null == externalData)
                return null;

            string myName = Path.GetFileNameWithoutExtension(path);

            // 创建宿主.
            GameObject go = new GameObject();
            var tmpOverload = GK.GetOrAddComponent<GKToyBaseOverlord>(go);
            tmpOverload.internalData = externalData;
            tmpOverload.name = myName;
            tmpOverload.internalData.data.name = myName;

            // 初始化首次数据.
            Selection.activeGameObject = go;
            _ResetSelected(tmpOverload);
            GKToyNode node = new GKToyStart(_GenerateGUID(CurNodeIdx++));
            Type type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = (_contentScrollPos.x + ToyMakerBase._minWidth * 0.5f) / Scale;
            node.pos.y = (_contentScrollPos.y + ToyMakerBase._minHeight * 0.5f) / Scale;
            _CreateNode(node);
            node = new GKToyEnd(_GenerateGUID(CurNodeIdx++));
            type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = -10;
            node.pos.y = -10;
            _CreateNode(node);
            _Changed();
            _overlord.Save();
            _overlord.Backup();
            _overlord.SavePrefab(ToyMakerBase._defaultOverlordPath);
            return _overlord;
        }
        /// <summary>
        /// 生成唯一ID
        /// </summary>
        protected int _GenerateGUID(int _id)
        {
            return ((DateTime.Now.Millisecond & 32767) << 15) | (_id & 32767);
        }

#if UNITY_2017_2_OR_NEWER
        private void _SaveDataWhenPlayModeChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (_overlord != null)
                {
                    _ResetScaleData();
                    _overlord.Save();
                }
            }
            else if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                instance = GetWindow<GKToyMaker>(_GetLocalization("Neuron"), false);
                var assets = Selection.GetFiltered(typeof(GKToyBaseOverlord), SelectionMode.Assets);
                if (0 == assets.Length)
                    return;
                else
                    _ResetSelected((GKToyBaseOverlord)assets[0]);
            }
        }
#else
        private void SaveDataWhenPlayModeChange()
        {
            if (!EditorApplication.isPlaying && _overlord != null)
            {
                _ResetScaleData();
                _overlord.Save();
            }
            else if (null == _instance)
            {
                _instance = GetWindow<GKToyMaker>(_GetLocalization("Neuron"), false);
                var assets = Selection.GetFiltered(typeof(GKToyBaseOverlord), SelectionMode.Assets);
                if (0 == assets.Length)
                    return;
                else
                    _ResetSelected((GKToyBaseOverlord)assets[0]);
            }
        }
#endif
        protected string _GetParentNamespace()
        {
            return "GKToy";
        }
        protected virtual string _GetNamespace()
        {
            return "GKToy";
        }
        #endregion

        #region Enum
        public enum InformationType
        {
            Detail = 0,
            Actions,
            Variables,
            Search,
            Inspector,
        }

        public enum ClickedElement
        {
            NoElement = 0,
            NodeElement,
            LinkElement,
            VirtualLinkElement
        }

        public enum LanguageType
        {
            English = 0,
            Chinese
        }
        #endregion

        // 新增节点数据结构.
        public class NewNode
        {
            public string parmKey = string.Empty;
            public GKToyNode node;
        }
    }
}