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
    public class GKToyMakerTextInput : EditorWindow
    {
        #region PublicField
        public static GKToyMakerTextInput instance;
        public delegate void CompletedEvent(string txt);
        #endregion

        #region PrivateField
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        private Color _defaultColor = Color.white;
        private CompletedEvent _completedEvent = null;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerTextInput>(GKToyMaker._GetLocalization("Text input"), true);
        }

        public static void InitSubData(CompletedEvent e)
        {
            instance._completedEvent += e;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerTextInput>(GKToyMaker._GetLocalization("Text input"), true);
                wantsMouseMove = true;
                minSize = new Vector2(300, 250);
                maxSize = new Vector2(300, 250);
            }
        }

        void OnGUI()
        { 
            // 主内容.
            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            //GUILayout.Label(GKToyMaker._GetLocalization("ID") + ": ", GUILayout.Width(60));
                            //GKEditor.DrawBaseControl(true, _data.ID.Value, (obj) => { _data.ID.SetValue(obj); });
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

        }

        void OnDestroy()
        {
            foreach(CompletedEvent d in instance._completedEvent.GetInvocationList())
                instance._completedEvent -= d;
            instance = null;
        }
        #endregion
    }
}
