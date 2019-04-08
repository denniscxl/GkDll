using GKBase;
using System.IO;
using UnityEditor;
using UnityEngine;
using GKToy;

namespace GKToyDialogue
{
    public class GKToyDialogueMaker : GKToyMaker
    {
        #region PublicField
        protected new ToyMakerDialogueSettings _settings;
        public new ToyMakerDialogueSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GK.LoadResource<ToyMakerDialogueSettings>("Settings/ToyMakerDialogueSettings");
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
        static public GKToyLocalizationData DialogueData
        {
            get
            {
                if (null == _dialogueData)
                {
                    _dialogueData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueLocalizationData.asset") as GKToyLocalizationData;
                    if (null == _dialogueData)
                        Debug.LogError("Load maker data faile.");
                }
                return _dialogueData;
            }
        }
        static GKToyLocalizationData _dialogueData;
        #endregion

        #region PublicMethod
        [MenuItem("GK/Dialogue Editor", false, GKEditorConfiger.MenuItemPriorityA)]
        public static void MenuItemWindow()
        {
            _instance = GetWindow<GKToyDialogueMaker>(_GetDialogueLocalization("Dialogue Editor"), false);
        }
        #endregion

        /// <summary>
        /// 导出游戏数据
        /// </summary>
        /// <param name="parameter">数据类型|导出路径（类型：1-客户端，2-服务器）</param>
        protected override void _ExportGameData(object parameter)
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
                        destPath = EditorUtility.OpenFolderPanel(_GetDialogueLocalization("Save path"), Application.dataPath, _GetDialogueLocalization("Select save path."));
                    } while (!Directory.Exists(destPath));
                    ToyMakerBase._defaultClientPath = destPath;
                }
                destPath = string.Format("{0}/{1}.lua", ToyMakerBase._defaultClientPath, fileName);
                GKToyMakerDialogueDataExporter.ExportClientData(CurRenderData, destPath);
            }
            else
            {
                // 服务器.
                destPath = ToyMakerBase._defaultServerPath;
                if (!Directory.Exists(destPath))
                {
                    do
                    {
                        destPath = EditorUtility.OpenFolderPanel(_GetDialogueLocalization("Save path"), Application.dataPath, _GetDialogueLocalization("Select save path."));
                    } while (!Directory.Exists(destPath));
                    ToyMakerBase._defaultServerPath = destPath;
                }
                destPath = string.Format("{0}/{1}.xml", ToyMakerBase._defaultServerPath, fileName);
                GKToyMakerDialogueDataExporter.ExportServerData(CurRenderData, destPath);
            }
        }

        /// <summary>
        /// 更新节点点击逻辑
        /// </summary>
        /// <returns></returns>
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
                        GKToyMakerDialogueNodeComSelector.SelectCom(node);
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
        /// 获取当前命名空间
        /// </summary>
        /// <returns></returns>
        protected override string _GetNamespace()
        {
            return "GKToyDialogue";
        }

        /// <summary>
        /// 当前编辑器的本地化数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public string _GetDialogueLocalization(string key)
        {
            GKToyLocalizationData.LocalizationData ld = DialogueData.GetLocalizationData(key);

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
    }
}