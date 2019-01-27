using UnityEngine;
using UnityEditor;

namespace GKToy
{
    [System.Serializable]
    public abstract class GKToyShardVariable<T> : GKToyVariable
    {
        [SerializeField]
        protected T _value;
        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        override public object GetValue()
        {
            return Value;
        }

        override public void SetValue(object value)
        {
            Value = (T)value;
            ValueChanged();
        }

        override public string ToString()
        {
            return Value.ToString();
        }
    }
}