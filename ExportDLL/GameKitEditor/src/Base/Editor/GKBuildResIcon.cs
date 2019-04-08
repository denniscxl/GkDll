using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GKBuildResIcon : EditorWindow
{
    List<Drawing> _drawings;
    List<UnityEngine.Object> _objects;
    Rect _oldPosition;
    float _scrollPos;
    float _maxY;
    bool _showingStyles = true;
    bool _showingIcons = false;
    string _search = "";

    public delegate void CallBack();

    //[MenuItem("GK/Built-in styles and icons")]
    public static void ShowWindow()
    {
        GKBuildResIcon window = (GKBuildResIcon)EditorWindow.GetWindow<GKBuildResIcon>();
        window.Show();
    }

    void OnGUI()
    {
        if (position.width != _oldPosition.width && Event.current.type == EventType.Layout)
        {
            _drawings = null;
            _oldPosition = position;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Toggle(_showingStyles, "Styles", EditorStyles.toolbarButton) != _showingStyles)
        {
            _showingStyles = !_showingStyles;
            _showingIcons = !_showingStyles;
            _drawings = null;
        }

        if (GUILayout.Toggle(_showingIcons, "Icons", EditorStyles.toolbarButton) != _showingIcons)
        {
            _showingIcons = !_showingIcons;
            _showingStyles = !_showingIcons;
            _drawings = null;
        }

        GUILayout.EndHorizontal();

        string newSearch = GUILayout.TextField(_search);
        if (newSearch != _search)
        {
            _search = newSearch;
            _drawings = null;
        }

        float top = 36;

        if (_drawings == null)
        {
            _drawings = new List<Drawing>();
            string lowerSearch = _search.ToLower();
            GUIContent inactiveText = new GUIContent("inactive");
            GUIContent activeText = new GUIContent("active");
            float x = 5.0f;
            float y = 5.0f;

            if (_showingStyles)
            {
                foreach (GUIStyle ss in GUI.skin.customStyles)
                {
                    if (lowerSearch != "" && !ss.name.ToLower().Contains(lowerSearch))
                        continue;

                    GUIStyle thisStyle = ss;
                    Drawing draw = new Drawing();
                    float width = Mathf.Max(
                        100.0f,
                        GUI.skin.button.CalcSize(new GUIContent(ss.name)).x,
                        ss.CalcSize(inactiveText).x + ss.CalcSize(activeText).x
                                      ) + 16.0f;

                    float height = 60.0f;

                    if (x + width > position.width - 32 && x > 5.0f)
                    {
                        x = 5.0f;
                        y += height + 10.0f;
                    }

                    draw.rect = new Rect(x, y, width, height);
                    width -= 8.0f;
                    draw.draw = () =>
                    {
                        if (GUILayout.Button(thisStyle.name, GUILayout.Width(width)))
                            CopyText("(GUIStyle)\"" + thisStyle.name + "\"");

                        GUILayout.BeginHorizontal();
                        GUILayout.Toggle(false, inactiveText, thisStyle, GUILayout.Width(width / 2));
                        GUILayout.Toggle(false, activeText, thisStyle, GUILayout.Width(width / 2));
                        GUILayout.EndHorizontal();
                    };

                    x += width + 18.0f;
                    _drawings.Add(draw);
                }
            }
            else if (_showingIcons)
            {
                if (_objects == null)
                {
                    _objects = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll(typeof(Texture)));
                    _objects.Sort((pA, pB) => System.String.Compare(pA.name, pB.name, System.StringComparison.OrdinalIgnoreCase));
                }

                float rowHeight = 0.0f;

                foreach (UnityEngine.Object oo in _objects)
                {
                    Texture texture = (Texture)oo;

                    if (texture.name == "")
                        continue;

                    if (lowerSearch != "" && !texture.name.ToLower().Contains(lowerSearch))
                        continue;

                    Drawing draw = new Drawing();

                    float width = Mathf.Max(
                        GUI.skin.button.CalcSize(new GUIContent(texture.name)).x,
                        texture.width
                    ) + 8.0f;

                    float height = texture.height + GUI.skin.button.CalcSize(new GUIContent(texture.name)).y + 8.0f;

                    if (x + width > position.width - 32.0f)
                    {
                        x = 5.0f;
                        y += rowHeight + 8.0f;
                        rowHeight = 0.0f;
                    }

                    draw.rect = new Rect(x, y, width, height);

                    rowHeight = Mathf.Max(rowHeight, height);

                    width -= 8.0f;

                    draw.draw = () =>
                    {
                        if (GUILayout.Button(texture.name, GUILayout.Width(width)))
                            CopyText("EditorGUIUtility.FindTexture( \"" + texture.name + "\" )");

                        Rect textureRect = GUILayoutUtility.GetRect(texture.width, texture.width, texture.height, texture.height, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
                        EditorGUI.DrawTextureTransparent(textureRect, texture);
                    };

                    x += width + 8.0f;

                    _drawings.Add(draw);
                }
            }

            _maxY = y;
        }

        Rect r = position;
        r.y = top;
        r.height -= r.y;
        r.x = r.width - 16;
        r.width = 16;

        float areaHeight = position.height - top;
        _scrollPos = GUI.VerticalScrollbar(r, _scrollPos, areaHeight, 0.0f, _maxY);

        Rect area = new Rect(0, top, position.width - 16.0f, areaHeight);
        GUILayout.BeginArea(area);

        int count = 0;
        foreach (Drawing draw in _drawings)
        {
            Rect newRect = draw.rect;
            newRect.y -= _scrollPos;

            if (newRect.y + newRect.height > 0 && newRect.y < areaHeight)
            {
                GUILayout.BeginArea(newRect, GUI.skin.textField);
                draw.draw();
                GUILayout.EndArea();
                count++;
            }
        }

        GUILayout.EndArea();
    }

    void CopyText(string pText)
    {
        TextEditor editor = new TextEditor();
        //editor.content = new GUIContent(pText); // Unity 4.x code
        editor.text = pText; // Unity 5.x code
        editor.SelectAll();
        editor.Copy();
    }

    struct Drawing
    {
        public Rect rect;
        public CallBack draw;
    }
}
