using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GKBase;
using System;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace GKToy
{
    public class GKToyMakerTaskCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerTaskCom instance;
        #endregion

        #region PrivateField
        // 内容索引.
        static protected int _contentIndex = 0;
        protected string[] _strContent = {"Request", "Dialogue(Begin)", "Content", "Dialogue(End)", "Reward"};
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        protected GKToyTask _task = null;
        protected PropertyInfo [] _propertyInfo = null;
        private Color _defaultColor = Color.white;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerTaskCom>("", false);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            _contentIndex = 0;
            instance._task = null;
            instance._propertyInfo = null;
        }

        // 初始化任务数据(不包含核心任务数据).
        public static void InitSubData(GKToyTask task, PropertyInfo [] info)
        {
            instance._task = task;
            instance._propertyInfo = info;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerTaskCom>("", false);
                wantsMouseMove = true;
                minSize = new Vector2(GKToyMaker.instance.toyMakerBase._minWidth - 200, GKToyMaker.instance.toyMakerBase._minHeight - 300);
                maxSize = new Vector2(GKToyMaker.instance.toyMakerBase._minWidth - 200, GKToyMaker.instance.toyMakerBase._minHeight - 300);
            }
        }

        void OnGUI()
        {
            if (null == _task)
                return;
            
            GUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("<-", GUILayout.Width(40), GUILayout.Height(GKToyMaker.instance.toyMakerBase._minHeight - 305)))
                {
                    if (0 == _contentIndex)
                        _contentIndex = 5;
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
                            idx = 5;
                        idx--;
                        GUILayout.Label("<-" + _strContent[idx], GUILayout.Width(120));
                        
                       GUILayout.Label(_strContent[_contentIndex], _styleCenrer, GUILayout.Width(GKToyMaker.instance.toyMakerBase._minWidth - 550));

                        idx = _contentIndex;
                        if (4 == _contentIndex)
                            idx = -1;
                        idx++;
                        GUILayout.Label(_strContent[idx] + "->", _styleRight, GUILayout.Width(120));
                    }
                    GUILayout.EndHorizontal();

                    int count = 0;
                    // 详细.
                    switch(_contentIndex)
                    {
                        // Request.
                        case 0:
                            count = _task.RequestTypeLst.GetValueCount();
                            if (0 != count)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    if (0 != i)
                                        GKEditor.DrawInspectorSeperator();

                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.BeginVertical();
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Type: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.RequestTypeLst.GetValue(i), (obj) => { _task.RequestTypeLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Count: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.RequestCountLst.GetValue(i), (obj) => { _task.RequestCountLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.EndVertical();
                                        _defaultColor = GUI.backgroundColor;
                                        GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._removeBgColor;
                                        if (GUILayout.Button(" X ", GUILayout.Width(30), GUILayout.Height(30)))
                                        {
                                            _task.RequestTypeLst.RemoveAt(i);
                                            _task.RequestCountLst.RemoveAt(i);
                                        }
                                        GUI.backgroundColor = _defaultColor;
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }

                            GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._addBgColor;
                            if (GUILayout.Button(" Create Request."))
                            {
                                _task.RequestTypeLst.AddCapacity();
                                _task.RequestCountLst.AddCapacity();
                            }
                            GUI.backgroundColor = _defaultColor;
                            break;
                        // Dialogue.
                        case 1:
                            count = _task.BeginDialogueIdxLst.GetValueCount();
                            if (0 != count)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    if (0 != i)
                                        GKEditor.DrawInspectorSeperator();

                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.BeginVertical();
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("ID: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.BeginDialogueIdxLst.GetValue(i), (obj) => { _task.BeginDialogueIdxLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Content: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.BeginDialogueContentLst.GetValue(i), (obj) => { _task.BeginDialogueContentLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.EndVertical();
                                        _defaultColor = GUI.backgroundColor;
                                        GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._removeBgColor;
                                        if (GUILayout.Button(" X ", GUILayout.Width(30), GUILayout.Height(30)))
                                        {
                                            _task.BeginDialogueIdxLst.RemoveAt(i);
                                            _task.BeginDialogueContentLst.RemoveAt(i);
                                        }
                                        GUI.backgroundColor = _defaultColor;
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }

                            GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._addBgColor;
                            if (GUILayout.Button(" Create Dialogue."))
                            {
                                _task.BeginDialogueIdxLst.AddCapacity();
                                _task.BeginDialogueContentLst.AddCapacity();
                            }
                            GUI.backgroundColor = _defaultColor;
                            break;
                        // Task.
                        case 2:

                            if (null != _propertyInfo && 0 != _propertyInfo.Length)
                            {
                                for (int i = 0; i < _propertyInfo.Length; i++)
                                {
                                    if (0 != i)
                                        GKEditor.DrawMiniInspectorSeperator();

                                    var prop = _propertyInfo[i];
                                    var v = prop.GetValue(_task, null);

                                    // 检测是否为自定义变量.
                                    if (v is GKToyVariable)
                                    {
                                        // 检测是否为链表.
                                        if (((GKToyVariable)v).IsLst)
                                        {
                                            int tCount = ((GKToyVariable)v).GetValueCount();
                                            for (int j = 0; j < tCount; j++)
                                            {
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GUILayout.Label(prop.Name + ": ", GUILayout.Width(60));
                                                    GKEditor.DrawBaseControl(true, ((GKToyVariable)v).GetValue(j), (obj) => { ((GKToyVariable)v).SetValue(j, obj); });
                                                    GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._removeBgColor;
                                                    if (GUILayout.Button("X", GUILayout.Width(20)))
                                                    {
                                                        ((GKToyVariable)v).RemoveAt(j);
                                                    }
                                                    GUI.backgroundColor = _defaultColor;
                                                }
                                                GUILayout.EndHorizontal();
                                            }
                                            GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._addBgColor;
                                            if (GUILayout.Button(GKToyMaker._GetLocalization("New element")))
                                            {
                                                ((GKToyVariable)v).AddCapacity();
                                            }
                                            GUI.backgroundColor = _defaultColor;
                                        }
                                        else
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label(prop.Name + ": ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, ((GKToyVariable)v).GetValue(), (obj) => { ((GKToyVariable)v).SetValue(obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                    else
                                    {
                                        GUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Label(prop.Name + ": ", GUILayout.Width(60));
                                            GKEditor.DrawBaseControl(true, v, (obj) => { prop.SetValue(_task, obj, null); });
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }
                            }
                            break;
                        // Dialogue.
                        case 3:
                            count = _task.EndDialogueIdxLst.GetValueCount();
                            if (0 != count)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    if (0 != i)
                                        GKEditor.DrawInspectorSeperator();

                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.BeginVertical();
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("ID: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.EndDialogueIdxLst.GetValue(i), (obj) => { _task.EndDialogueIdxLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Content: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.EndDialogueContentLst.GetValue(i), (obj) => { _task.EndDialogueContentLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.EndVertical();
                                        _defaultColor = GUI.backgroundColor;
                                        GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._removeBgColor;
                                        if (GUILayout.Button(" X ", GUILayout.Width(30), GUILayout.Height(30)))
                                        {
                                            _task.EndDialogueIdxLst.RemoveAt(i);
                                            _task.EndDialogueContentLst.RemoveAt(i);
                                        }
                                        GUI.backgroundColor = _defaultColor;
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }

                            GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._addBgColor;
                            if (GUILayout.Button(" Create Dialogue."))
                            {
                                _task.EndDialogueIdxLst.AddCapacity();
                                _task.EndDialogueContentLst.AddCapacity();
                            }
                            GUI.backgroundColor = _defaultColor;
                            break;
                        // Reward.
                        case 4:
                            count = _task.RewardTypeLst.GetValueCount();
                            if (0 != count)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    if (0 != i)
                                        GKEditor.DrawInspectorSeperator();

                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.BeginVertical();
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Type: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.RewardTypeLst.GetValue(i), (obj) => { _task.RewardTypeLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Count: ", GUILayout.Width(60));
                                                GKEditor.DrawBaseControl(true, _task.RewardCountLst.GetValue(i), (obj) => { _task.RewardCountLst.SetValue(i, obj); });
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.EndVertical();
                                        _defaultColor = GUI.backgroundColor;
                                        GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._removeBgColor;
                                        if (GUILayout.Button(" X ", GUILayout.Width(30), GUILayout.Height(30)))
                                        {
                                            _task.RewardTypeLst.RemoveAt(i);
                                            _task.RewardCountLst.RemoveAt(i);
                                        }
                                        GUI.backgroundColor = _defaultColor;
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }

                            GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._addBgColor;
                            if (GUILayout.Button(" Create Reward."))
                            {
                                _task.RewardTypeLst.AddCapacity();
                                _task.RewardCountLst.AddCapacity();
                            }
                            GUI.backgroundColor = _defaultColor;
                            break;
                    }
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("->", GUILayout.Width(40), GUILayout.Height(GKToyMaker.instance.toyMakerBase._minHeight - 305)))
                {
                    if (4 == _contentIndex)
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
