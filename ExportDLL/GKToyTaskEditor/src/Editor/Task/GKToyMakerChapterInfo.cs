using GKToy;
using GKUI;
using UnityEditor;
using UnityEngine;

namespace GKToyTaskEditor
{
    public class GKToyMakerChapterInfo : GKUIModalWindow
    {
        #region PublicField

        public static GKToyMakerChapterInfo instance;
        public GKToyBaseOverlord overlord;
        public GameObject obj;

        #endregion

        #region PrivateField
        const float WIDTH = 500;
        const float HEIGHT = 100;
        const float MARGIN = 40;
        #endregion

        #region PublicMethod
        public static GKToyMakerChapterInfo Create(IModal owner, Vector2 pos, float width, float height, string title, GKToyBaseOverlord overlord, GameObject obj)
        {
            instance = CreateInstance<GKToyMakerChapterInfo>();

            instance._owner = owner;
            instance._title = title;
            instance.overlord = overlord;
            instance.obj = obj;
            Rect rect = new Rect(pos.x, pos.y, 0, 0);
            instance.position = rect;
            instance.ShowAsDropDown(rect, new Vector2(width, height + MARGIN));

            return instance;
        }
        #endregion

        #region PrivateMethod
        protected override void _Draw(Rect region)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space((position.height - HEIGHT) / 2);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space((position.width - WIDTH) / 2);
                    GUILayout.BeginVertical("Box", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("ID Interval") + ": ", GUILayout.Width(60));
                                overlord.internalData.data.minLiteralId = EditorGUILayout.IntField(overlord.internalData.data.minLiteralId);
                                GUILayout.Label("-");
                                overlord.internalData.data.maxLiteralId = EditorGUILayout.IntField(overlord.internalData.data.maxLiteralId);
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndHorizontal();
                        if (GUILayout.Button(GKToyTaskMaker._GetTaskLocalization("OK")))
                        {
                            if (_CheckInterval())
                                _Ok();
                            else
                                ShowNotification(new GUIContent(GKToyTaskMaker._GetTaskLocalization("Wrong Interval")));
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private bool _CheckInterval()
        {
            return true;
        }
        #endregion
    }
}
