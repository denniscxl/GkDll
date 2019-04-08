using GKBase;
using System.IO;
using UnityEditor;
using UnityEngine;
using GKToy;
using System;
using GKUI;
using GKToyTaskDialogue;

namespace GKToyTaskEditor
{
    public class GKToyTaskMaker : GKToyMaker, IModal
    {
        #region PublicField
        protected new static GKToyTaskMaker _instance;
        public new static GKToyTaskMaker Instance
        {
            get
            {
                if (null == _instance)
                    _instance = GetWindow<GKToyTaskMaker>(_GetTaskLocalization("Task Editor"), false);
                return _instance;
            }
        }
        protected new ToyMakerTaskSettings _settings;
        public new ToyMakerTaskSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GK.LoadResource<ToyMakerTaskSettings>("Settings/ToyMakerTaskSettings");
                }
                return _settings;
            }
        }
        public override ToyMakerSettings.ToyMakerBase ToyMakerBase
        {
            get
            {
                return Settings.toyMakerBase;
            }
        }
        static public GKToyLocalizationData TaskData
        {
            get
            {
                if (null == _taskData)
                {
                    _taskData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyTaskLocalizationData.asset") as GKToyLocalizationData;
                    if (null == _taskData)
                        Debug.LogError("Load maker data faile.");
                }
                return _taskData;
            }
        }
        static GKToyLocalizationData _taskData;
        #endregion

        #region PrivateField
        #endregion

        #region PublicMethod
        [MenuItem("GK/Task Editor", false, GKEditorConfiger.MenuItemPriorityA)]
        public static void MenuItemWindow()
        {
            _instance = GetWindow<GKToyTaskMaker>(_GetTaskLocalization("Task Editor"), false);
        }

        [MenuItem("GK/Export Task Client Data", false, GKEditorConfiger.MenuItemPriorityA)]
        public static void ExportTaskClientData()
        {
            ToyMakerTaskSettings.ToyMakerTask exportSetting = GK.LoadResource<ToyMakerTaskSettings>("Settings/ToyMakerTaskSettings").toyMakerBase;
            DirectoryInfo directoryInfo = new DirectoryInfo(exportSetting._defaultOverlordPath);
            FileInfo[] files = directoryInfo.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            GKToyExternalData externalData;
            for (int i = 0; i < files.Length; i++)
            {
                externalData = AssetDatabase.LoadMainAssetAtPath(string.Format("{0}/{1}", exportSetting._defaultOverlordPath, files[i].Name)) as GKToyExternalData;
                if (null!= externalData)
                    _ExportGameDataByMenuItem(1, exportSetting, externalData.data);
            }
        }

        [MenuItem("GK/Export Task Server Data", false, GKEditorConfiger.MenuItemPriorityA)]
        public static void ExportTaskServerData()
        {
            ToyMakerTaskSettings.ToyMakerTask exportSetting = GK.LoadResource<ToyMakerTaskSettings>("Settings/ToyMakerTaskSettings").toyMakerBase;
            DirectoryInfo directoryInfo = new DirectoryInfo(exportSetting._defaultOverlordPath);
            FileInfo[] files = directoryInfo.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            GKToyExternalData externalData;
            for (int i = 0; i < files.Length; i++)
            {
                externalData = AssetDatabase.LoadMainAssetAtPath(string.Format("{0}/{1}", exportSetting._defaultOverlordPath, files[i].Name)) as GKToyExternalData;
                if (null != externalData)
                    _ExportGameDataByMenuItem(2, exportSetting, externalData.data);
            }
        }

        public static void _ExportGameDataByMenuItem(int dataType, ToyMakerTaskSettings.ToyMakerTask exportSetting, GKToyData curData)
        {
            string destPath;
            // 导出路径.
            if (1 == dataType)
            {
                // 客户端.
                destPath = exportSetting._defaultClientPath;
                if (!Directory.Exists(destPath))
                {
                    do
                    {
                        destPath = EditorUtility.OpenFolderPanel(_GetLocalization("Save path"), Application.dataPath, _GetLocalization("Select save path."));
                    } while (!Directory.Exists(destPath));
                    exportSetting._defaultClientPath = destPath;
                }
                GKToyMakerTaskDataExporter.ExportClientData(curData, destPath, curData.name);
            }
            else
            {
                // 服务器.
                destPath = exportSetting._defaultServerPath;
                if (!Directory.Exists(destPath))
                {
                    do
                    {
                        destPath = EditorUtility.OpenFolderPanel(_GetLocalization("Save path"), Application.dataPath, _GetLocalization("Select save path."));
                    } while (!Directory.Exists(destPath));
                    exportSetting._defaultServerPath = destPath;
                }
                GKToyMakerTaskDataExporter.ExportServerData(curData, destPath, curData.name);
            }
        }
        #endregion

        #region Data Export
        /// <summary>
        /// 导出游戏数据
        /// </summary>
        /// <param name="parameter">数据类型|导出路径（类型：1-客户端，2-服务器）</param>
        protected override void _ExportGameData(object parameter)
        {
        }
        #endregion

        #region Event

        // 更新节点点击逻辑.
        protected override bool _UpdateNodeTouch()
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
                    // 禁止从虚拟出节点连出、禁止从已有连接的虚拟入节点连出.
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
                        GKToyMakerTaskNodeComSelector.SelectCom(node, CurRenderData);
                        node.DoubleClick();
                    }
                    else
                        _CheckNodeClick(node, mousePos);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 添加链接时，添加任务间的关系
        /// </summary>
        /// <param name="srcNode"></param>
        /// <param name="destNode"></param>
        /// <param name="paramKey"></param>
        protected override void _AddLink(GKToyNode srcNode, GKToyNode destNode, string paramKey)
        {
            base._AddLink(srcNode, destNode, paramKey);
            if ("GKToyTaskEditor.GKToyTask" == srcNode.className)
            {
                if ("GKToyTaskEditor.GKToyTask" == destNode.className)
                {
                    GKToyTask destTask = (GKToyTask)destNode;
                    ((GKToyTask)srcNode).nextTaskIds.Add(destTask.id);
                    destTask.preTaskIds.Add(srcNode.id);
                    if (1 < destTask.preTaskIds.Count)
                        destTask.preSeperator.Add("&");
                }
                else if (destNode.className.Contains("GKToyTaskEditor.GKToySubTask"))
                {
                    _SetMainTask((GKToySubTask)destNode, (GKToyTask)srcNode);
                }
            }
            else if (srcNode.className.Contains("GKToyTaskEditor.GKToySubTask"))
            {
                if ("GKToyTaskEditor.GKToyTask" == destNode.className)
                {
                    GKToyTask destTask = (GKToyTask)destNode;
                    int mainId = ((GKToySubTask)srcNode).mainTask;
                    if (CurRenderData.nodeLst.ContainsKey(mainId))
                    {
                        ((GKToyTask)CurRenderData.nodeLst[mainId]).nextTaskIds.Add(destTask.id);
                        destTask.preTaskIds.Add(mainId);
                        if (1 < destTask.preTaskIds.Count)
                            destTask.preSeperator.Add("&");
                    }
                }
                else if (destNode.className.Contains("GKToyTaskEditor.GKToySubTask"))
                {
                    int mainId = ((GKToySubTask)srcNode).mainTask;
                    if (CurRenderData.nodeLst.ContainsKey(mainId))
                        _SetMainTask((GKToySubTask)destNode, (GKToyTask)CurRenderData.nodeLst[mainId]);
                }
            }
        }
        /// <summary>
        /// 删除链接时，删除任务间的关系
        /// </summary>
        /// <param name="srcNode"></param>
        /// <param name="destNode"></param>
        protected override void _RemoveLink(GKToyNode srcNode, GKToyNode destNode)
        {
            base._RemoveLink(srcNode, destNode);
            if ("GKToyTaskEditor.GKToyTask" == srcNode.className)
            {
                if ("GKToyTaskEditor.GKToyTask" == destNode.className)
                {
                    GKToyTask destTask = (GKToyTask)destNode;
                    ((GKToyTask)srcNode).nextTaskIds.Remove(destTask.id);
                    int linkId = destTask.preTaskIds.IndexOf(srcNode.id);
                    if (0 <= linkId && 0 < destTask.preSeperator.Count)
                    {
                        destTask.preTaskIds.RemoveAt(linkId);
                        if (0 == linkId)
                            destTask.preSeperator.RemoveAt(0);
                        else
                            destTask.preSeperator.RemoveAt(linkId - 1);
                    }
                }
                else if (destNode.className.Contains("GKToyTaskEditor.GKToySubTask"))
                {
                    _CancelMainTask((GKToySubTask)destNode, (GKToyTask)srcNode);
                }
            }
            else if (srcNode.className.Contains("GKToyTaskEditor.GKToySubTask"))
            {
                if ("GKToyTaskEditor.GKToyTask" == destNode.className)
                {
                    GKToyTask destTask = (GKToyTask)destNode;
                    int mainId = ((GKToySubTask)srcNode).mainTask;
                    if (CurRenderData.nodeLst.ContainsKey(mainId))
                    {
                        ((GKToyTask)CurRenderData.nodeLst[mainId]).nextTaskIds.Remove(destTask.id);
                        int linkId = destTask.preTaskIds.IndexOf(mainId);
                        if (0 <= linkId && 0 < destTask.preSeperator.Count)
                        {
                            destTask.preTaskIds.RemoveAt(linkId);
                            if (0 == linkId)
                                destTask.preSeperator.RemoveAt(0);
                            else
                                destTask.preSeperator.RemoveAt(linkId - 1);
                        }
                    }
                }
                else if (destNode.className.Contains("GKToyTaskEditor.GKToySubTask"))
                {
                    int mainId = ((GKToySubTask)srcNode).mainTask;
                    if (CurRenderData.nodeLst.ContainsKey(mainId))
                        _CancelMainTask((GKToySubTask)destNode, (GKToyTask)CurRenderData.nodeLst[mainId]);
                }
            }
        }
        #endregion

        #region Data
        // 创建实例.
        public override GKToyBaseOverlord _CreateData(string path)
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
            GKToyMakerChapterInfo.Create(this, position.min, ToyMakerBase._minWidth, ToyMakerBase._minHeight, _GetTaskLocalization("Chapter Infomation"), tmpOverload, go);
            return tmpOverload;
        }
        /// <summary>
        /// 设置ID区间完毕后，设置实例
        /// </summary>
        /// <param name="window"></param>
        public void _ModalClosed(GKUIModalWindow window)
        {
            GKToyMakerChapterInfo info = window as GKToyMakerChapterInfo;

            if (info == null)
                return;
            if (window.Result != WindowResult.Ok)
            {
                info.Focus();
                return;
            }
            info.overlord.internalData.data.curLiteralId = info.overlord.internalData.data.minLiteralId;
            Selection.activeGameObject = info.obj;
            _ResetSelected(info.overlord);
            GKToyNode node = new GKToyStart(_GenerateGUID(CurNodeIdx++));
            Type type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = (_contentScrollPos.x + ToyMakerBase._minWidth * 0.5f) / Scale;
            node.pos.y = (_contentScrollPos.y + ToyMakerBase._minHeight * 0.5f) / Scale;
            _CreateNode(node);
            _Changed();
            _overlord.Save();
            _overlord.Backup();
            _overlord.SavePrefab(ToyMakerBase._defaultOverlordPath);
        }

        public void _ModalRequest(bool shift)
        {
            // Modal Rename doesn't have a MenuItem implementation. So this method is not used.
        }

        public void SaveData()
        {
            _ResetScaleData();
            _overlord.Save();
            _overlord.Backup();
        }

        protected override void _CreateNode(GKToyNode node)
        {
            base._CreateNode(node);
            if("GKToyTaskEditor.GKToyTask" == node.className)
            {
                GKToyTask taskNode = (GKToyTask)node;
                taskNode.initTaskId = CurRenderData.curLiteralId++;
                taskNode.TaskID = taskNode.initTaskId;
                taskNode.AcceptDfgObject = _CreateDialogueData(string.Format("{0}/TaskDfg/", ToyMakerBase._defaultOverlordPath), taskNode.TaskID.Value, "AcceptDfg");
                taskNode.SubmitDfgObject = _CreateDialogueData(string.Format("{0}/TaskDfg/", ToyMakerBase._defaultOverlordPath), taskNode.TaskID.Value, "SubmitDfg");
                AssetDatabase.Refresh();
            }
            else if (node.className.Contains("GKToyTaskEditor.GKToySubTask"))
            {
                GKToySubTask taskNode = (GKToySubTask)node;
                if("GKToyTaskEditor.GKToySubTaskInteract" == node.className)
                {
                    ((GKToySubTaskInteract)taskNode).InteractDfgObject = _CreateDialogueData(string.Format("{0}/TaskDfg/", ToyMakerBase._defaultOverlordPath), taskNode.TargetID.Value, "Interact");
                    AssetDatabase.Refresh();
                }
            }
        }

        protected override void _RemoveNode(GKToyNode node)
        {
            base._RemoveNode(node);
            if ("GKToyTaskEditor.GKToyTask" == node.className)
            {
                GKToyTask taskNode = (GKToyTask)node;
                string path = string.Format("{0}/TaskDfg/AcceptDfg_{1}.Asset", ToyMakerBase._defaultOverlordPath, taskNode.TaskID.Value);
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
                path = string.Format("{0}/TaskDfg/SubmitDfg_{1}.Asset", ToyMakerBase._defaultOverlordPath, taskNode.TaskID.Value);
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
                path = string.Format("{0}/TaskDfg/AcceptDfg_{1}.prefab", ToyMakerBase._defaultOverlordPath, taskNode.TaskID.Value);
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
                path = string.Format("{0}/TaskDfg/SubmitDfg_{1}.prefab", ToyMakerBase._defaultOverlordPath, taskNode.TaskID.Value);
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }
            else if ("GKToyTaskEditor.GKToySubTaskInteract" == node.className)
            {
                GKToySubTaskInteract taskNode = (GKToySubTaskInteract)node;
                string path = string.Format("{0}/TaskDfg/Interact_{1}.Asset", ToyMakerBase._defaultOverlordPath, taskNode.TargetID.Value);
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
                path = string.Format("{0}/TaskDfg/Interact_{1}.prefab", ToyMakerBase._defaultOverlordPath, taskNode.TargetID.Value);
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }
        }

        GameObject _CreateDialogueData(string path, int taskId, string dfgType)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 生成数据源.
            var obj = ScriptableObject.CreateInstance<GKToyExternalData>();
            GKToyData tData = new GKToyData();
            obj.data = tData.Clone() as GKToyData;
            string assetPath = string.Format("{0}{1}_{2}.Asset", path, dfgType, taskId);
            GKEditor.CreateAsset(obj, assetPath);
            var externalData = AssetDatabase.LoadMainAssetAtPath(assetPath) as GKToyExternalData;
            if (null == externalData)
                return null;

            string myName = string.Format("{0}_{1}", dfgType, taskId);

            // 创建宿主.
            GameObject go = new GameObject();
            var tmpOverload = GK.GetOrAddComponent<GKToyBaseOverlord>(go);
            tmpOverload.internalData = externalData;
            tmpOverload.name = myName;
            tmpOverload.internalData.data.name = myName;

            // 初始化首次数据.
            GKToyNode node = new GKToyStart(_GenerateGUID(tmpOverload.internalData.data.nodeGuid++));
            Type type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = (_contentScrollPos.x + ToyMakerBase._minWidth * 0.5f) / Scale;
            node.pos.y = (_contentScrollPos.y + ToyMakerBase._minHeight * 0.5f) / Scale;
            node.id = node.ID;
            node.nodeType = NodeType.Action;
            node.name = "开始-1";
            node.Init(tmpOverload);
            node.comment = "";
            tmpOverload.internalData.data.nodeLst.Add(node.id, node);
            node = new GKToyEnd(_GenerateGUID(_GenerateGUID(tmpOverload.internalData.data.nodeGuid++)));
            type = node.GetType();
            node.className = string.Format("{0}.{1}", type.Namespace, type.Name);
            node.pos.x = -10;
            node.pos.y = -10;
            node.id = node.ID;
            node.nodeType = NodeType.Action;
            node.name = "结束-2";
            node.Init(tmpOverload);
            node.comment = "";
            tmpOverload.internalData.data.nodeLst.Add(node.id, node);
            tmpOverload.Save();
            tmpOverload.Backup();
            string prefabPath = string.Format("{0}{1}_{2}.prefab", path, dfgType, taskId);
            GameObject prefab;
            if (!File.Exists(prefabPath))
            {
                prefab = PrefabUtility.CreatePrefab(prefabPath, go);
            }
            else
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                prefab = PrefabUtility.ReplacePrefab(go, prefab);
            }
            DestroyImmediate(go);
            return prefab;
        }

        /// <summary>
        /// 给子任务设置主任务节点
        /// </summary>
        /// <param name="subTask">子任务节点</param>
        /// <param name="mainTask">主任务节点，留空则最后一个节点</param>
        /// <returns>子任务编号</returns>
        private void _SetMainTask(GKToySubTask subTask, GKToyTask mainTask)
        {
            if(null != mainTask)
            {
                subTask.subId = mainTask.subTasks.Count + 1;
                subTask.mainTask = mainTask.id;
                subTask.initTargetId = mainTask.LiteralId * 10000 + subTask.type * 1000 + subTask.subId;
                subTask.ChangeTaskID(subTask.initTargetId);
                mainTask.subTasks.Add(subTask.id);
            }
            else
            {
                subTask.subId = -1;
            }
            foreach(Link link in subTask.links)
            {
                if (CurRenderData.nodeLst.ContainsKey(link.next) && ((GKToyNode)CurRenderData.nodeLst[link.next]).className.Contains("GKToyTaskEditor.GKToySubTask"))
                {
                    _SetMainTask(((GKToySubTask)CurRenderData.nodeLst[link.next]), mainTask);
                }
            }
        }
        /// <summary>
        /// 取消主、子任务间的链接
        /// </summary>
        /// <param name="subTask">子任务</param>
        /// <param name="mainTask">主任务</param>
        private void _CancelMainTask(GKToySubTask subTask, GKToyTask mainTask)
        {
            mainTask.subTasks.Remove(subTask.id);
            subTask.ChangeTaskID(subTask.id);
            foreach (Link link in subTask.links)
            {
                if (CurRenderData.nodeLst.ContainsKey(link.next) && ((GKToyNode)CurRenderData.nodeLst[link.next]).className.Contains("GKToyTaskEditor.GKToySubTask"))
                {
                    _CancelMainTask(((GKToySubTask)CurRenderData.nodeLst[link.next]), mainTask);
                }
            }
        }

        #endregion

        #region Other

        protected override string _GetNamespace()
        {
            return "GKToyTaskEditor";
        }

        static public string _GetTaskLocalization(string key)
        {
            GKToyLocalizationData.LocalizationData ld = TaskData.GetLocalizationData(key);

            if (null != ld)
            {
                switch (_language)
                {
                    case LanguageType.Chinese:
                        return ld.chinese;
                    case LanguageType.English:
                        return ld.english;
                }
            }
            return _GetLocalization(key);
        }
        #endregion
    }

    public enum TaskIdType
    {
        FunctionalNode,
        MainTask,
        SubTask
    }
}