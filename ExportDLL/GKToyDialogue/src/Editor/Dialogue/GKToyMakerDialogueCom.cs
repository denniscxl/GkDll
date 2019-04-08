using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GKBase;
using GKToy;

namespace GKToyDialogue
{
    public class GKToyMakerDialogueCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerDialogueCom instance;

        static GKToyDialogueCameraTypeData _cameraTypeData;
        static public GKToyDialogueCameraTypeData CameraTypeData
        {
            get
            {
                if (null == _cameraTypeData)
                {
                    _cameraTypeData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueCameraTypeData.asset") as GKToyDialogueCameraTypeData;
                    if (null == _cameraTypeData)
                        Debug.LogError("Load cameraType faile.");
                }
                return _cameraTypeData;
            }
        }

        static GKToyDialogueSoundTypeData _soundTypeData;
        static public GKToyDialogueSoundTypeData SoundTypeData
        {
            get
            {
                if (null == _soundTypeData)
                {
                    _soundTypeData = AssetDatabase.LoadMainAssetAtPath("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueSoundTypeData.asset") as GKToyDialogueSoundTypeData;
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
            instance = GetWindow<GKToyMakerDialogueCom>(GKToyDialogueMaker._GetDialogueLocalization("Dialogue fragment"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(300, 260);
            instance.maxSize = new Vector2(300, 260);
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
                instance = GetWindow<GKToyMakerDialogueCom>(GKToyDialogueMaker._GetDialogueLocalization("Dialogue fragment"), true);
                wantsMouseMove = true;
                minSize = new Vector2(300, 250);
                maxSize = new Vector2(300, 250);
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
                            GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Entity") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.Entity.Value, (obj) => { _data.Entity.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Entity Name") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.Speaker.Value, (obj) => { _data.Speaker.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Content") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.SpeakText.Value, (obj) => { _data.SpeakText.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Menu Text") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.MenuText.Value, (obj) => { _data.MenuText.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("ActionDescription") + ": ", GUILayout.Width(50));
                            GKEditor.DrawBaseControl(true, _data.SpeakText2.Value, (obj) => { _data.SpeakText2.SetValue(obj); });
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
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Camera") + ": ", GUILayout.Width(50));
                    int seleIdx = EditorGUILayout.Popup(_data.CameraRes.Value, CameraTypeData.GeTypeArray(), GUILayout.Width(130));
                    if (seleIdx != _data.CameraRes.Value)
                        _data.CameraRes.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.CameraRes.Value, (obj) => { _data.CameraRes.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Camera Value") + ": ", GUILayout.Width(50));
                    GKEditor.DrawBaseControl(true, _data.CameraValue.Value, (obj) => { _data.CameraValue.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GKEditor.DrawInspectorSeperator();
                
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Animation") + ": ", GUILayout.Width(50));
                    GKEditor.DrawBaseControl(true, _data.AnimationRes.Value, (obj) => { _data.AnimationRes.SetValue(obj); });
                }
                GUILayout.EndHorizontal();
                
                GKEditor.DrawInspectorSeperator();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Sound") + ": ", GUILayout.Width(50));
                    int seleIdx = EditorGUILayout.Popup(_data.SoundRes.Value, SoundTypeData.GeTypeArray(), GUILayout.Width(130));
                    if (seleIdx != _data.SoundRes.Value)
                        _data.SoundRes.SetValue(seleIdx);
                    GKEditor.DrawBaseControl(true, _data.SoundRes.Value, (obj) => { _data.SoundRes.SetValue(obj); });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Sound Value") + ": ", GUILayout.Width(50));
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
