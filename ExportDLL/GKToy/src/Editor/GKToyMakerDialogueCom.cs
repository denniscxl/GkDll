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
            instance = GetWindow<GKToyMakerDialogueCom>("", false);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
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
                instance = GetWindow<GKToyMakerDialogueCom>("", false);
                wantsMouseMove = true;
                minSize = new Vector2(GKToyMaker.instance.toyMakerBase._minWidth - 200, GKToyMaker.instance.toyMakerBase._minHeight - 300);
                maxSize = new Vector2(GKToyMaker.instance.toyMakerBase._minWidth - 200, GKToyMaker.instance.toyMakerBase._minHeight - 300);
            }
        }

        void OnGUI()
        {
            if (null == _data)
                return;
            
            // 主内容.
            GUILayout.BeginVertical("Box");
            {
                int count = _data._IDLst.GetValueCount();
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
                                    GKEditor.DrawBaseControl(true, _data._IDLst.GetValue(i), (obj) => { _data._IDLst.SetValue(i, obj); });
                                }
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Content: ", GUILayout.Width(60));
                                    GKEditor.DrawBaseControl(true, _data.ContentLst.GetValue(i), (obj) => { _data.ContentLst.SetValue(i, obj); });
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                            _defaultColor = GUI.backgroundColor;
                            GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._removeBgColor;
                            if (GUILayout.Button(" X ", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                _data._IDLst.RemoveAt(i);
                                _data.ContentLst.RemoveAt(i);
                            }
                            GUI.backgroundColor = _defaultColor;
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                GUI.backgroundColor = GKToyMaker.instance.toyMakerBase._addBgColor;
                if (GUILayout.Button(" Create dialogue."))
                {
                    _data._IDLst.AddCapacity();
                    _data.ContentLst.AddCapacity();
                }
                GUI.backgroundColor = _defaultColor;
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
