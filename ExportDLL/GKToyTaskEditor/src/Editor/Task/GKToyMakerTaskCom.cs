using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GKBase;
using System;
using System.Reflection;
using GKToy;
using GKToyTaskDialogue;

namespace GKToyTaskEditor
{
    public class GKToyMakerTaskCom : EditorWindow
    {
        const int LABEL_WIDTH = 120;
        const int BUTTON_WIDTH = 100;
        const int NOTE_WIDTH = 300;
        #region PublicField
        public static GKToyMakerTaskCom instance;
        #endregion

        #region PrivateField
        // 内容索引.
        static protected int _contentIndex = 0;
        protected string[] _strContent = { "Basic Information", "Text Information"};
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        protected GKToyTask _task = null;
        protected GKToyData _data = null;
        private Color _defaultColor = Color.white;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerTaskCom>(GKToyTaskMaker._GetTaskLocalization("Main Task Config"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            _contentIndex = 0;
            instance._task = null;
        }

        // 初始化任务数据(不包含核心任务数据).
        public static void InitSubData(GKToyTask task, GKToyData data)
        {
            instance._task = task;
            instance._data = data;
            instance.tmpTaskId = task.TaskID.Value;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerTaskCom>("", true);
                wantsMouseMove = true;
                minSize = new Vector2(GKToyTaskMaker.Instance.ToyMakerBase._minWidth - 500, GKToyTaskMaker.Instance.ToyMakerBase._minHeight - 300);
                maxSize = new Vector2(GKToyTaskMaker.Instance.ToyMakerBase._minWidth - 500, GKToyTaskMaker.Instance.ToyMakerBase._minHeight - 300);
                for (int i = 0; i < instance._strContent.Length; ++i)
                {
                    instance._strContent[i] = GKToyTaskMaker._GetTaskLocalization(instance._strContent[i]);
                }
            }
        }

        int tmpTaskId;
        void OnGUI()
        {
            if (null == _task)
                return;
            
            GUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("<-", GUILayout.Width(40), GUILayout.Height(GKToyTaskMaker.Instance.ToyMakerBase._minHeight - 305)))
                {
                    if (0 == _contentIndex)
                        _contentIndex = instance._strContent.Length;
                    _contentIndex--;
                }

                // 主内容.
                GUILayout.BeginVertical("Box");
                {
                    // Title.
                    GUILayout.BeginHorizontal();
                    {
                        int idx = _contentIndex;
                        if (0 == _contentIndex)
                            idx = instance._strContent.Length;
                        idx--;
                        GUILayout.Label("<-" + _strContent[idx], GUILayout.Width(120));
                        
                        GUILayout.Label(_strContent[_contentIndex], _styleCenrer, GUILayout.Width(GKToyTaskMaker.Instance.ToyMakerBase._minWidth - 850));

                        idx = _contentIndex;
                        if (instance._strContent.Length - 1 == _contentIndex)
                            idx = -1;
                        idx++;
                        GUILayout.Label(_strContent[idx] + "->", _styleRight, GUILayout.Width(120));
                    }
                    GUILayout.EndHorizontal();
                    // 详细.
                    switch(_contentIndex)
                    {
                        // Basic Info.
                        case 0:
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task ID") + ": ", GUILayout.Width(LABEL_WIDTH));
                                tmpTaskId = EditorGUILayout.IntField(tmpTaskId);
                                // 检查手填的任务id是否有效.
                                if (GUILayout.Button(GKToyTaskMaker._GetTaskLocalization("Check and Save"), GUILayout.Width(BUTTON_WIDTH)))
                                {
                                    bool isValid = false;
                                    if (tmpTaskId >=_data.minLiteralId && tmpTaskId <= _data.maxLiteralId)
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
                                        tmpTaskId = _task.TaskID.Value;
                                        ShowNotification(new GUIContent(GKToyTaskMaker._GetTaskLocalization("Invalid ID")));
                                        GUI.FocusControl(null);
                                    }
                                }
                                if (GUILayout.Button(GKToyTaskMaker._GetTaskLocalization("Reset"), GUILayout.Width(BUTTON_WIDTH)))
                                {
                                    _task.TaskID = _task.initTaskId;
                                    tmpTaskId = _task.TaskID.Value;
                                    GUI.FocusControl(null);
                                }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(LABEL_WIDTH);
                                GUILayout.Label(string.Format("ID范围：{0}-{1}，且不能与已有ID重复", _data.minLiteralId, _data.maxLiteralId));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("If Tell") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.IfTell.Value, (obj) => { _task.IfTell.SetValue(obj); });
                                GUILayout.Label(string.Format("1有；0没有", _data.minLiteralId, _data.maxLiteralId), GUILayout.Width(NOTE_WIDTH));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.TaskType.Value, (obj) => { _task.TaskType.SetValue(obj); });
                                GUILayout.Label(string.Format("1主线；2支线", _data.minLiteralId, _data.maxLiteralId), GUILayout.Width(NOTE_WIDTH));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Reset Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.ResetType.Value, (obj) => { _task.ResetType.SetValue(obj); });
                                GUILayout.Label(string.Format("1每天；2每周；3每月；0不自动重置", _data.minLiteralId, _data.maxLiteralId), GUILayout.Width(NOTE_WIDTH));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("If Give Up") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.IfGiveUp.Value, (obj) => { _task.IfGiveUp.SetValue(obj); });
                                GUILayout.Label(string.Format("1能；0不能", _data.minLiteralId, _data.maxLiteralId), GUILayout.Width(NOTE_WIDTH));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task Lua") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.TaskLua.Value, (obj) => { _task.TaskLua.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Npc") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.AcceptNpc.Value, (obj) => { _task.AcceptNpc.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Scene") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.AcceptScene.Value, (obj) => { _task.AcceptScene.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Lv") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.AcceptLv.Value, (obj) => { _task.AcceptLv.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Reput") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.AcceptReput.Value, (obj) => { _task.AcceptReput.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.AcceptType.Value, (obj) => { _task.AcceptType.SetValue(obj); });
                                GUILayout.Label(string.Format("0：走交互-选择-对话-确认流程；1：自动接取", _data.minLiteralId, _data.maxLiteralId), GUILayout.Width(NOTE_WIDTH));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Movie") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.AcceptMovie.Value, (obj) => { _task.AcceptMovie.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Npc") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.SubmitNpc.Value, (obj) => { _task.SubmitNpc.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Scene") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.SubmitScene.Value, (obj) => { _task.SubmitScene.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Lv") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.SubmitLv.Value, (obj) => { _task.SubmitLv.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Reput") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.SubmitReput.Value, (obj) => { _task.SubmitReput.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.SubmitType.Value, (obj) => { _task.SubmitType.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Failed Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.FailedType.Value, (obj) => { _task.FailedType.SetValue(obj); });
                                GUILayout.Label(string.Format("1失败状态不删除任务", _data.minLiteralId, _data.maxLiteralId), GUILayout.Width(NOTE_WIDTH));
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Prize ID") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.Prize.Value, (obj) => { _task.Prize.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Movie") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.SubmitMovie.Value, (obj) => { _task.SubmitMovie.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Pre Task") + ": ", GUILayout.Width(LABEL_WIDTH));
                                for(int i =0;i<_task.preTaskIds.Count;++i)
                                {
                                    GUILayout.Label(((GKToyTask)_data.nodeLst[_task.preTaskIds[i]]).TaskID.ToString(), GUILayout.Width(LABEL_WIDTH));
                                    if (i < _task.preSeperator.Count)
                                        _task.preSeperator[i] = GUILayout.TextField(_task.preSeperator[i], GUILayout.Width(20));
                                }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(LABEL_WIDTH);
                                GUILayout.Label("且& 或|");
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Next Task") + ": ", GUILayout.Width(LABEL_WIDTH));
                                for (int i = 0; i < _task.nextTaskIds.Count; ++i)
                                {
                                    GUILayout.Label(((GKToyTask)_data.nodeLst[_task.nextTaskIds[i]]).TaskID.ToString(), GUILayout.Width(LABEL_WIDTH));
                                    if (i < _task.nextTaskIds.Count - 1)
                                        GUILayout.Label(",", GUILayout.Width(20));
                                }
                            }
                            GUILayout.EndHorizontal();
                            break;
                        // Text Information.
                        case 1:
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task Name") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.TaskName.Value, (obj) => { _task.TaskName.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task Target") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.TaskTarget.Value, (obj) => { _task.TaskTarget.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task Desc") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.TaskDesc.Value, (obj) => { _task.TaskDesc.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Task Can Submit") + ": ", GUILayout.Width(LABEL_WIDTH));
                                GKEditor.DrawBaseControl(true, _task.TaskCanSubmit.Value, (obj) => { _task.TaskCanSubmit.SetValue(obj); });
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Accept Dfg") + ": ", GUILayout.Width(LABEL_WIDTH));
                                if (GUILayout.Button(_task.AcceptDfg.Value))
                                {
                                    GKToyDialogueMaker.Instance.ShowDialogue(_task.AcceptDfgObject.Value.GetComponent<GKToyBaseOverlord>());
                                }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Submit Dfg") + ": ", GUILayout.Width(LABEL_WIDTH));
                                if (GUILayout.Button(_task.SubmitDfg.Value))
                                {
                                    GKToyDialogueMaker.Instance.ShowDialogue(_task.SubmitDfgObject.Value.GetComponent<GKToyBaseOverlord>());
                                }
                            }
                            GUILayout.EndHorizontal();
                            break;
                    }
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("->", GUILayout.Width(40), GUILayout.Height(GKToyTaskMaker.Instance.ToyMakerBase._minHeight - 305)))
                {
                    if (instance._strContent.Length - 1 == _contentIndex)
                        _contentIndex = -1;
                    _contentIndex++;
                }
            }
            GUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            instance = null;
        }
        #endregion
    }
}
