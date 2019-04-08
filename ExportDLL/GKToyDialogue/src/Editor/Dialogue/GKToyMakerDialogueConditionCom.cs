using UnityEngine;
using UnityEditor;
using GKBase;
using GKToy;

namespace GKToyDialogue
{
    public class GKToyMakerDialogueConditionCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerDialogueConditionCom instance;

        static GKToyDialogueConditionTypeData _conditionType;
        static public GKToyDialogueConditionTypeData ConditionType
        {
            get
            {
                if (null == _conditionType)
                {
                    _conditionType = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueConditionTypeData.asset") as GKToyDialogueConditionTypeData;
                    if (null == _conditionType)
                        Debug.LogError("Load conditionType faile.");
                }
                return _conditionType;
            }
        }

        static GKToyDialogueConditionOutputTypeData _conditionOutputType;
        static public GKToyDialogueConditionOutputTypeData ConditionOutputType
        {
            get
            {
                if (null == _conditionOutputType)
                {
                    Debug.Log(1);
                    _conditionOutputType = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueConditionOutputTypeData.asset") as GKToyDialogueConditionOutputTypeData;
                    if (null == _conditionType)
                        Debug.LogError("Load conditionType faile.");
                }
                return _conditionOutputType;
            }
        }
        #endregion

        #region PrivateField
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        protected GKToyDialogueCondition _data = null;
        private Color _defaultColor = Color.white;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerDialogueConditionCom>(GKToyDialogueMaker._GetDialogueLocalization("Dialogue condition"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(300, 100);
            instance.maxSize = new Vector2(300, 100);
            instance._data = null;
        }

        public static void InitSubData(GKToyDialogueCondition data)
        {
            instance._data = data;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerDialogueConditionCom>("", true);
                wantsMouseMove = true;
                minSize = new Vector2(200, 100);
                maxSize = new Vector2(200, 100);
            }
        }

        void OnGUI()
        {
            if (null == _data)
                return;
            
            // 主内容.
            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Condition") + ": ", GUILayout.Width(60));

                    int seleIdx = EditorGUILayout.Popup(_data.CondPara.Value, ConditionType.GetConditionTypeArray(), GUILayout.Width(160));
                    if (seleIdx != _data.CondPara.Value)
                        _data.CondPara.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.CondPara.Value, (obj) => { _data.CondPara.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Condition Value") + ": ", GUILayout.Width(60));
                    GKEditor.DrawBaseControl(true, _data.CondValue.Value, (obj) => { _data.CondValue.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GKEditor.DrawInspectorSeperator();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("OutputType") + ": ", GUILayout.Width(60));

                    int seleIdx = EditorGUILayout.Popup(_data.OutPutType.Value, ConditionOutputType.GetArray(), GUILayout.Width(160));
                    if (seleIdx != _data.OutPutType.Value)
                        _data.OutPutType.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.OutPutType.Value, (obj) => { _data.OutPutType.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("YesNodeID") + ": ", GUILayout.Width(60));
                    GUILayout.Label(_data.IfYesNode.Value.ToString(), GUILayout.Width(60));
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("NoNodeID") + ": ", GUILayout.Width(60));
                    GUILayout.Label(_data.IfNoNode.Value.ToString(), GUILayout.Width(60));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

        }

        void OnDestroy()
        {
            instance = null;
        }
        #endregion
    }
}
