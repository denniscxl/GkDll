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
    public class GKToyMakerDialogueConditionCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerDialogueConditionCom instance;

        static GKToyConditionTypeData _conditionType;
        static public GKToyConditionTypeData ConditionType
        {
            get
            {
                if (null == _conditionType)
                {
                    _conditionType = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyConditionTypeData.asset") as GKToyConditionTypeData;
                    if (null == _conditionType)
                        Debug.LogError("Load conditionType faile.");
                }
                return _conditionType;
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
            instance = GetWindow<GKToyMakerDialogueConditionCom>(GKToyMaker._GetLocalization("Dialogue condition"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(300, 300);
            instance.maxSize = new Vector2(300, 300);
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
                minSize = new Vector2(200, 300);
                maxSize = new Vector2(200, 300);
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
                    GUILayout.Label(GKToyMaker._GetLocalization("Condition") + ": ", GUILayout.Width(60));

                    int seleIdx = EditorGUILayout.Popup(_data.CondPara.Value, ConditionType.GetConditionTypeArray(), GUILayout.Width(160));
                    if (seleIdx != _data.CondPara.Value)
                        _data.CondPara.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.CondPara.Value, (obj) => { _data.CondPara.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyMaker._GetLocalization("Condition Value") + ": ", GUILayout.Width(60));
                    GKEditor.DrawBaseControl(true, _data.CondValue.Value, (obj) => { _data.CondValue.SetValue(obj); });
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
