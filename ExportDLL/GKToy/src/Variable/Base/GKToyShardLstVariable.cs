using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GKToy
{
    [System.Serializable]
    public abstract class GKToyShardLstVariable<T> : GKToyVariable
    {
        [SerializeField]
        protected List<T> _value;
        public T [] Value
        {
            get {
                if (null == _value)
                    _value = new List<T>();
                return _value.ToArray();
            }
            set 
            {
                _value.Clear();
                if(null != value)
                {
                    foreach (var v in value)
                        _value.Add(v);
                }   
            }
        }

        public GKToyShardLstVariable()
        {
            IsLst = true;
        }

        override public object GetValue()
        {
            return Value;
        }

        override public void SetValue(object value)
        {
            _value = (List<T>)value;
            ValueChanged();
        }

        override public string ToString()
        {
            return _value.ToString();
        }

        override public object GetValue(int idx)
        {
            if (null == _value || _value.Count <= idx || 0 > idx)
                return null;

            return _value[idx];
        }

        override public void SetValue(int idx, object obj) 
        {
            if (null == _value || _value.Count <= idx || 0 > idx)
                return;
            _value[idx] = (T)obj;
            ValueChanged();
        }

        override public void RemoveAt(int idx)
        {
            if (null == _value || _value.Count <= idx || 0 > idx)
                return;
            _value.RemoveAt(idx);
            ValueChanged();
        }

        override public void AddCapacity() 
        {
            if (null == _value)
                _value = new List<T>();

            _value.Add(default(T));
            ValueChanged();
        }

        override public int GetValueCount()
        {
            if (null == _value)
                return 0;
            return _value.Count;
        }
    }
}