using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GKBase;

namespace GKUI
{
    public class GKUIPanelWindow : EditorWindow
    {
        List<Entry> _settingAssetList = new List<Entry>();
        List<Entry> _skinAssetList = new List<Entry>();
        List<Entry> _panelList = new List<Entry>();

        static bool _settingAssetFoldout = true;
        static bool _skinsFoldout = true;
        static bool _panelFoldout = true;
        static bool _bShowDemoAssets = true;

        //[MenuItem("GK/UI/Convert BoxCollider 2D to 3D")]
        static public void MenuItem_ConvertBoxCollider2Dto3D()
        {
            foreach (var o in Selection.gameObjects)
            {
                _ConvertBoxCollider2Dto3D(o);
            }
        }

        //[MenuItem("GK/UI/Panel Window", false, GKEditorConfiger.MenuItemPriorityA)]
        static public void MenuItem_Window()
        {
            var w = EditorWindow.GetWindow<GKUIPanelWindow>("UI Panel Window Panels");
            w.autoRepaintOnSceneChange = true;
            w.minSize = new Vector2(200, 18);
            w.Show();
        }

        static void _ConvertBoxCollider2Dto3D(GameObject o)
        {
            o.layer = GK.LayerId("UI");

            var rb = o.GetComponent<Rigidbody>();
            if (rb)
            {

                GK.Destroy(rb);
            }

            var co = o.GetComponent<BoxCollider2D>();

            if (co)
            {

                Vector4 v4 = new Vector4(co.size.x, co.size.y, co.offset.x, co.offset.y);

                GK.Destroy(co);

                o.AddComponent<BoxCollider>();
                o.GetComponent<BoxCollider>().size = new Vector3(v4.x, v4.y, 1);
                o.GetComponent<BoxCollider>().center = new Vector3(v4.z, v4.w, 1);
            }

            foreach (Transform t in o.transform)
            {
                _ConvertBoxCollider2Dto3D(t.gameObject);
            }
        }

        public class Entry
        {
            public string path;
            public string name;
            private GameObject _sceneObj;
            public GameObject sceneObj
            {
                set 
                { 
                    _sceneObj = value;
                }
                get { return _sceneObj; }
            }

            public Entry(string path)
            {
                this.path = path.Replace('\\', '/');
                name = System.IO.Path.GetFileNameWithoutExtension(path);
                sceneObj = GameObject.Find("UIController/UIRoot/" + name);
            }

            public void Load()
            {
                GameObject go = GKEditor.LoadGameObject(path, true);
                sceneObj = go;

                GK.SetParent(go, UIController.instance.GetOrAddGroup("UIRoot"), false);
                var tran = go.GetComponent<RectTransform>();
                if (null != tran)
                {
                    tran.localPosition = Vector3.zero;
                    tran.sizeDelta = Vector2.zero;
                }
                Selection.activeGameObject = go;
            }

            public void Save()
            {
                if (sceneObj)
                {
                    GKEditor.CreateOrReplacePrefab(path, sceneObj);
                    AssetDatabase.SaveAssets();
                    GK.Destroy(sceneObj);
                    sceneObj = null;
                }
            }

            public void ToggleForSceneObject()
            {
                GUILayout.Space(16);
                var b = sceneObj != null;
                if (GUILayout.Toggle(b, name) != b)
                {
                    if (sceneObj)
                    {
                        Save();
                    }
                    else
                    {
                        Load();
                    }
                }
            }

            public void ButtonForSelect()
            {
                GUILayout.Space(16);
                if (GUILayout.Toggle(false, name))
                {
                    var o = GKEditor.LoadResource<Object>(path);
                    if (o)
                    {
                        Selection.activeObject = o;
                    }
                }
            }
        }


        void OnEnable()
        {
            RefreshList();
        }

        void RefreshList()
        {

            string rootPath = /*_bShowDemoAssets ? "Assets/Resources/_Demo/" :*/ "Assets/Utilities/Examples/Resources/";
            {
                _settingAssetList.Clear();
                if(System.IO.Directory.Exists(rootPath + "UI/Settings"))
                {
                    var list = System.IO.Directory.GetFiles(rootPath + "UI/Settings", "*.asset");
                    foreach (var p in list)
                    {
                        var e = new Entry(p);
                        _settingAssetList.Add(e);
                    }
                }
            }

            {
                _panelList.Clear();
                var list = System.IO.Directory.GetFiles(rootPath + "UI/Panels", "*.prefab");
                foreach (var p in list)
                {
                    var e = new Entry(p);
                    _panelList.Add(e);
                }
            }

            //{
            //	_skinAssetList.Clear(); 
            //	var list = System.IO.Directory.GetFiles( rootPath + "UI/Skins", "*.asset" );
            //	foreach( var p in list ) {
            //		var e = new Entry( p );
            //		_skinAssetList.Add( e );
            //	}
            //}
        }

        Vector2 scroll;

        void OnGUI()
        {

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Box(" ", EditorStyles.toolbarButton);
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {
                    RefreshList();
                }
                if (GUILayout.Toggle(_bShowDemoAssets, "Demo", EditorStyles.toolbarButton) != _bShowDemoAssets)
                {
                    _bShowDemoAssets = !_bShowDemoAssets;
                    RefreshList();
                }
                GUILayout.Box(" ", EditorStyles.toolbarButton);
            }
            EditorGUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            _settingAssetFoldout = EditorGUILayout.Foldout(_settingAssetFoldout, "Settings");
            if (_settingAssetFoldout)
            {
                foreach (var e in _settingAssetList)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        e.ButtonForSelect();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            _skinsFoldout = EditorGUILayout.Foldout(_skinsFoldout, "Skins");
            if (_skinsFoldout)
            {
                foreach (var e in _skinAssetList)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        e.ButtonForSelect();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            _panelFoldout = EditorGUILayout.Foldout(_panelFoldout, "Panels");
            if (_panelFoldout)
            {
                foreach (var e in _panelList)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        e.ToggleForSceneObject();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndScrollView();

        }
    }
}