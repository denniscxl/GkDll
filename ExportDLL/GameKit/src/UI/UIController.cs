using UnityEngine;
using System.Collections.Generic;
using GKBase;

namespace GKUI
{
    public class UIController : MonoBehaviour
    {
        static UIController _instance;
        static public UIController instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = GK.GetOrAddComponent<UIController>(GK.TryLoadGameObject("Prefabs/Manager/UIController"));
                }
                return _instance;
            }
        }

        static List<UIBase> _list = new List<UIBase>();
        static public int width = 1124;
        static public int height = 2436;
        public Camera camera = null;

        void Awake()
        {
            DontDestroyOnLoad(this);
            _list.Clear();
        }

        static public void AddUI(UIBase ui)
        {
            if (!_list.Contains(ui))
            {
                _list.Add(ui);
            }

            _Sort();
        }

        static public void RemoveUI(UIBase ui)
        {
            if (_list.Contains(ui))
            {
                _list.Remove(ui);
            }
        }

        static void _Sort()
        {
            UIBase temp;
            for (int i = 0; i < _list.Count; i++)
            {
                for (int j = i + 1; j < _list.Count; j++)
                {
                    if (_list[j].priority < _list[i].priority)
                    {
                        temp = _list[j];
                        _list[j] = _list[i];
                        _list[i] = temp;
                    }
                }
            }

            foreach (var ui in _list)
            {
                if (null == ui || null == ui.GetComponent<RectTransform>())
                {
                    Debug.LogError("UI rectTranform is null");
                    continue;
                }
                ui.GetComponent<RectTransform>().SetSiblingIndex(100);
            }
        }

        //static UI_Settings _settings;
        //static public UI_Settings settings
        //{
        //    get
        //    {
        //        if (_settings == null)
        //        {
        //            _settings = GK.LoadResource<UI_Settings>("UI/Settings/UI_Settings");
        //        }
        //        return _settings;
        //    }
        //}

        public GameObject panelGroup;
        static public void ClosePanel<T>(T panel) where T : MonoBehaviour
        {
            if (panel != null)
            {
                GK.Destroy(panel.gameObject);
            }
        }

        static public T LoadDemoPanel<T>() where T : MonoBehaviour
        {
            return LoadPanel<T>("_Demo/UI/Panels/");
        }

        static public T LoadPanel<T>(string p = "UI/Panels/") where T : MonoBehaviour
        {
            var type = typeof(T);

            string path = p + type.Name;

            GameObject obj = null;

            if (!obj)
            {
                obj = GK.TryLoadGameObject(path);
            }

            if (!obj)
            {
                Debug.LogError("Error load UI panle " + type.Name);
                return null;
            }

            var c = obj.GetComponent<T>();
            if (!c)
            {
                Debug.LogError("Error GetComponent from UI Panel " + type.Name);
                return null;
            }

            GK.SetParent(c.gameObject, instance.panelGroup.gameObject, false);

            return c;
        }

        static public AnimationClip LoadAnimation(string clip)
        {
            string path = "UI/Animations/" + clip;

            AnimationClip obj = null;

            obj = Resources.Load(path, typeof(AnimationClip)) as AnimationClip;

            if (!obj)
            {
                Debug.LogError("Error load animation " + clip);
                return null;
            }

            return obj;
        }

        public GameObject GetOrAddGroup(string name)
        {
            var o = GK.FindChild(gameObject, name, true, false);
            if (!o)
            {
                o = new GameObject(name);
                GK.SetParent(o, gameObject, false);
                o.layer = GK.LayerId("UI");
            }
            return o;
        }

        public void Init()
        {
        }

        public void CloseAllPanel()
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                _list[i].DoClose();
            }
            _list.Clear();
        }

        public GameObject hudRoot;

        // 是否显示HUD.
        // Locked 锁定. 当锁定状态时，其他调用此函数除非同样为锁定, 否则无效.
        bool _hudLocked = false;
        public void ShowHUD(bool show, bool lodked = false)
        {
            if (lodked)
                _hudLocked = !_hudLocked;
            // 如果此次操作为Lock级.
            if (lodked)
                hudRoot.SetActive(show);
            else if (!_hudLocked)
                hudRoot.SetActive(show);

        }

        public void ClearHUD()
        {
            GK.DestroyAllChildren(hudRoot);
        }
    }
}