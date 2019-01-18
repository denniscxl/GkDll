using UnityEditor;
using UnityEngine;

/// <summary>
/// 快捷键提示窗口
/// </summary>
namespace GKToy
{
    public class GKToyMakerShortcuts : EditorWindow
    {
        const int width = 350;
        const int height = 150;
        const int widthMargin = 50;
        const int heightMargin = 40;

        Rect _area;
        void OnGUI()
        {
            GUILayout.BeginArea(_area);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GKToyMaker._GetLocalization("Save data"));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Shift + Alt + S");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GKToyMaker._GetLocalization("Duplicate nodes"));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Shift + Alt + D");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GKToyMaker._GetLocalization("Delete Node"));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Shift + Alt + T");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        void Awake()
        {
            minSize = new Vector2(width, height);
            maxSize = minSize;
            _area = new Rect(widthMargin, heightMargin, width - widthMargin * 2, height - heightMargin * 2);
        }
    }
}
