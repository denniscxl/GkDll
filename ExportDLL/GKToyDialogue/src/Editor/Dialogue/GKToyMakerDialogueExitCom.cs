using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GKBase;
using GKToy;

namespace GKToyDialogue
{
    public class GKToyMakerDialogueExitCom : EditorWindow
    {
        #region PublicField
        public static GKToyMakerDialogueExitCom instance;
        #endregion

        #region PrivateField
        protected Vector2 _contentScrollPos = new Vector2(0f, 0f);
        static protected GUIStyle _styleCenrer = new GUIStyle();
        static protected GUIStyle _styleRight = new GUIStyle();
        protected GKToyDialogueExit _data = null;
        private Color _defaultColor = Color.white;
        #endregion

        #region PublicMethod
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerDialogueExitCom>(GKToyDialogueMaker._GetDialogueLocalization("Dialogue exit"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(300, 80);
            instance.maxSize = new Vector2(300, 80);
            instance._data = null;
        }

        public static void InitSubData(GKToyDialogueExit data)
        {
            instance._data = data;
        }
        #endregion

        #region PrivateMethod
        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerDialogueExitCom>("", true);
                wantsMouseMove = true;
                minSize = new Vector2(200, 70);
                maxSize = new Vector2(200, 70);
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
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(GKToyDialogueMaker._GetDialogueLocalization("Menu Text") + ": ", GUILayout.Width(50));
                        GKEditor.DrawBaseControl(true, _data.MenuText.Value, (obj) => { _data.MenuText.SetValue(obj); });
                    }
                    GUILayout.EndHorizontal();
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
