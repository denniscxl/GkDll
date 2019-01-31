﻿using System.Collections.Generic;
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
    public class GKToyMakerDialogueActionCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerDialogueActionCom instance;
        
        static GKToyActionTypeData _actionTypeData;
        static public GKToyActionTypeData ActionTypeData
        {
            get
            {
                if (null == _actionTypeData)
                {
                    _actionTypeData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyActionTypeData.asset") as GKToyActionTypeData;
                    if (null == _actionTypeData)
                        Debug.LogError("Load actionType faile.");
                }
                return _actionTypeData;
            }
        }
        #endregion

        #region PrivateField
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        protected GKToyDialogueAction _data = null;
        private Color _defaultColor = Color.white;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerDialogueActionCom>(GKToyMaker._GetLocalization("Dialogue action"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(300, 70);
            instance.maxSize = new Vector2(300, 70);
            instance._data = null;
        }

        public static void InitSubData(GKToyDialogueAction data)
        {
            instance._data = data;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerDialogueActionCom>(GKToyMaker._GetLocalization("Dialogue action"), true);
                wantsMouseMove = true;
                minSize = new Vector2(300, 70);
                maxSize = new Vector2(300, 70);
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
                    GUILayout.Label(GKToyMaker._GetLocalization("Action") + ": ", GUILayout.Width(50));
                    int seleIdx = EditorGUILayout.Popup(_data.Action.Value, ActionTypeData.GetActionTypeArray(), GUILayout.Width(130));
                    if (seleIdx != _data.Action.Value)
                        _data.Action.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.Action.Value, (obj) => { _data.Action.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyMaker._GetLocalization("Action Value") + ": ", GUILayout.Width(50));
                    GKEditor.DrawBaseControl(true, _data.ActionValue.Value, (obj) => { _data.ActionValue.SetValue(obj); });
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
