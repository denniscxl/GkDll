using UnityEngine;
using UnityEditor;
using GKBase;
using GKToy;

namespace GKToyTaskEditor
{
    public class GKToyMakerSubTaskCom : EditorWindow
    {
        protected const int LABEL_WIDTH = 100;
        protected const int BUTTON_WIDTH = 100;
        protected const int NOTE_WIDTH = 150;

        #region PrivateField
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        private Color _defaultColor = Color.white;
        protected GKToySubTask _task = null;
        protected GKToyData _data = null;
        #endregion

        #region PrivateMethod
        void OnGUI()
        {
            if (null == _data)
                return;

            // 内容.
            GUILayout.BeginVertical("Box");
            {
                DrawCommonUI();
                DrawSubUI();
            }
            GUILayout.EndVertical();

        }
        protected int tmpTaskId;
        /// <summary>
        /// 绘制子任务通用属性
        /// </summary>
        void DrawCommonUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Target ID") + ": ", GUILayout.Width(LABEL_WIDTH));
                tmpTaskId = EditorGUILayout.IntField(tmpTaskId);
                // 检查手填的任务id是否有效.
                if (GUILayout.Button(GKToyTaskMaker._GetTaskLocalization("Check and Save"), GUILayout.Width(BUTTON_WIDTH)))
                {
                    bool isValid = false;
                    if (tmpTaskId >= _data.minLiteralId * 10000 && tmpTaskId <= _data.maxLiteralId * 10000 + 9999)
                    {
                        isValid = true;
                        foreach (GKToyNode node in _data.nodeLst.Values)
                        {
                            if (!node.className.Contains("GKToyTaskEditor"))
                                continue;
                            if (tmpTaskId == node.LiteralId)
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                    if (isValid)
                    {
                        _task.ChangeTaskID(tmpTaskId);
                        GKToyTaskMaker.Instance.SaveData();
                        ShowNotification(new GUIContent(GKToyTaskMaker._GetTaskLocalization("Save Success")));
                    }
                    else
                    {
                        tmpTaskId = _task.TargetID.Value;
                        ShowNotification(new GUIContent(GKToyTaskMaker._GetTaskLocalization("Invalid ID")));
                        GUI.FocusControl(null);
                    }
                }
                // 重置ID.
                if (GUILayout.Button(GKToyTaskMaker._GetTaskLocalization("Reset"), GUILayout.Width(BUTTON_WIDTH)))
                {
                    _task.TargetID = _task.initTargetId;
                    tmpTaskId = _task.TargetID.Value;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(LABEL_WIDTH);
                GUILayout.Label(string.Format("ID范围：{0}-{1}，且不能与已有ID重复", _data.minLiteralId * 10000, _data.maxLiteralId * 10000 + 9999));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Target Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                GUILayout.Label(_task.TargetType.ToString());
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Scene") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _task.Scene.Value, (obj) => { _task.Scene.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Target Info") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _task.TargetInfo.Value, (obj) => { _task.TargetInfo.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Target Text") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _task.TargetText.Value, (obj) => { _task.TargetText.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制子任务特有属性
        /// </summary>
        protected virtual void DrawSubUI(){}
        #endregion
    }
}
