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
    public class GKToyMakerDialogueCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerDialogueCom instance;

        static GKToyCameraTypeData _cameraTypeData;
        static public GKToyCameraTypeData CameraTypeData
        {
            get
            {
                if (null == _cameraTypeData)
                {
                    _cameraTypeData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyCameraTypeData.asset") as GKToyCameraTypeData;
                    if (null == _cameraTypeData)
                        Debug.LogError("Load cameraType faile.");
                }
                return _cameraTypeData;
            }
        }

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

        static GKToySoundTypeData _soundTypeData;
        static public GKToySoundTypeData SoundTypeData
        {
            get
            {
                if (null == _soundTypeData)
                {
                    _soundTypeData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToySoundTypeData.asset") as GKToySoundTypeData;
                    if (null == _soundTypeData)
                        Debug.LogError("Load soundType faile.");
                }
                return _soundTypeData;
            }
        }
        #endregion

        #region PrivateField
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        protected GKToyDialogue _data = null;
        private Color _defaultColor = Color.white;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerDialogueCom>(GKToyMaker._GetLocalization("Dialogue fragment"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(300, 300);
            instance.maxSize = new Vector2(300, 300);
            instance._data = null;
        }

        public static void InitSubData(GKToyDialogue data)
        {
            instance._data = data;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerDialogueCom>(GKToyMaker._GetLocalization("Dialogue fragment"), true);
                wantsMouseMove = true;
                minSize = new Vector2(300, 300);
                maxSize = new Vector2(300, 300);
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
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyMaker._GetLocalization("ID") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.ID.Value, (obj) => { _data.ID.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyMaker._GetLocalization("Content") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.Content.Value, (obj) => { _data.Content.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyMaker._GetLocalization("Menu Text") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.MenuText.Value, (obj) => { _data.MenuText.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyMaker._GetLocalization("ActionDescription") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.SpeakText.Value, (obj) => { _data.SpeakText.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    _defaultColor = GUI.backgroundColor;
                }
                GUILayout.EndHorizontal();

                GKEditor.DrawInspectorSeperator();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyMaker._GetLocalization("Camera") + ": ", GUILayout.Width(50));
                    int seleIdx = EditorGUILayout.Popup(_data.Camera.Value, CameraTypeData.GeTypeArray(), GUILayout.Width(130));
                    if (seleIdx != _data.Camera.Value)
                        _data.Camera.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.Camera.Value, (obj) => { _data.Camera.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyMaker._GetLocalization("Camera Value") + ": ", GUILayout.Width(50));
                    GKEditor.DrawBaseControl(true, _data.CameraValue.Value, (obj) => { _data.CameraValue.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GKEditor.DrawInspectorSeperator();
                
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

                GKEditor.DrawInspectorSeperator();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyMaker._GetLocalization("Sound") + ": ", GUILayout.Width(50));
                    int seleIdx = EditorGUILayout.Popup(_data.Sound.Value, SoundTypeData.GeTypeArray(), GUILayout.Width(130));
                    if (seleIdx != _data.Sound.Value)
                        _data.Sound.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.Sound.Value, (obj) => { _data.Sound.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyMaker._GetLocalization("Sound Value") + ": ", GUILayout.Width(50));
                    GKEditor.DrawBaseControl(true, _data.SoundValue.Value, (obj) => { _data.SoundValue.SetValue(obj); });
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
