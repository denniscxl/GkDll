using UnityEngine;
using System.Collections;
using GKBase;

namespace GKUI
{
    [DisallowMultipleComponent]
    public class UIBase : MonoBehaviour
    {

        public int priority = 0;

        public GameObject FindChild(string name)
        {
            var c = TryFindChild(name);
            if (!c) Debug.LogWarning("Panel cannot find child: Panel=" + this.gameObject.name + " child=" + name);
            return c;
        }

        public GameObject TryFindChild(string name)
        {
            return GK.FindChild(this.gameObject, name, true);
        }

        virtual public void OnOpen()
        {
        }

        virtual public void OnClose() { }
        public void DoClose()
        {
            OnClose();
            UIController.ClosePanel(this);
        }

        virtual public void OnActive(bool b) { }

        virtual public void OnPosition(Vector3 pos) { }

        virtual public void OnSize(Vector2 size) { }

    }

    public class SingletonUIBase<T> : UIBase where T : UIBase
    {
        static protected T _instance;
        static public T instance { get { return _instance; } }
        static public bool active { get { if (instance) return instance.gameObject.activeSelf; return false; } }
        static bool _keepInMemory = false;
        static public bool KeepInMemory
        {
            set
            {
                //			Debug.Log (string.Format ("keepInMemory Set: {0}", value));
                _keepInMemory = value;
            }
            get
            {
                return _keepInMemory;
            }
        }

        static public T Open(bool b)
        {
            return b ? Open() : Close();
        }

        static public T Open()
        {
            if (!_instance)
            {
                _instance = UIController.LoadPanel<T>();
            }
            _Init(_instance);
            return _instance;
        }

        static public T OpenDemo()
        {
            if (!_instance)
            {
                _instance = UIController.LoadDemoPanel<T>();
            }
            _Init(_instance);
            return _instance;
        }

        static void _Init(T instance)
        {
            if (instance)
            {
                instance.gameObject.SetActive(true);
                RectTransform rt = GK.GetOrAddComponent<RectTransform>(instance.gameObject);
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector3.zero;
                UIController.AddUI(instance);
                instance.OnOpen();
            }
        }

        static public T Close()
        {
            if (!_instance) return null;
            //		Debug.Log (string.Format("keepInMemory: {0}", keepInMemory));
            if (KeepInMemory)
            {
                _instance.gameObject.SetActive(false);
                _instance.OnClose();
            }
            else
            {
                UIController.RemoveUI(_instance);
                Release();

            }
            return null;
        }

        static public void Release()
        {
            if (_instance)
            {
                _instance.DoClose();
                _instance = null;
            }
        }

        public void PlayCilckSound()
        {

        }
    }
}