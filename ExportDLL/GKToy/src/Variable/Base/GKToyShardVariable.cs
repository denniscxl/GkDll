using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace GKToy
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class GKToyShardVariable<T> : GKToyVariable
    {
        [SerializeField]
        [JsonProperty]
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