using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedFloat : GKToyShardVariable<float>
    {
        static public implicit operator GKToySharedFloat(float value) { return new GKToySharedFloat { Value = value }; }

        static public GKToySharedFloat operator +(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            GKToySharedFloat result = new GKToySharedFloat();
            result = (float)(lhs.GetValue()) + (float)(rhs.GetValue());
            return result;
        }

        static public GKToySharedFloat operator -(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            GKToySharedFloat result = new GKToySharedFloat();
            result = (float)(lhs.GetValue()) - (float)(rhs.GetValue());
            return result;
        }

        static public GKToySharedFloat operator *(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            GKToySharedFloat result = new GKToySharedFloat();
            result = (float)(lhs.GetValue()) * (float)(rhs.GetValue());
            return result;
        }

        static public GKToySharedFloat operator /(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            GKToySharedFloat result = new GKToySharedFloat();
            result = (float)(lhs.GetValue()) / (float)(rhs.GetValue());
            return result;
        }

        static public GKToySharedFloat operator %(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            GKToySharedFloat result = new GKToySharedFloat();
            result = (float)(lhs.GetValue()) % (float)(rhs.GetValue());
            return result;
        }

        static public bool operator ==(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            return (Mathf.Abs((float)(lhs.GetValue()) - (float)(rhs.GetValue())) <= 0.1f);
        }

        static public bool operator !=(GKToySharedFloat lhs, GKToySharedFloat rhs)
        {
            return (Mathf.Abs((float)(lhs.GetValue()) - (float)(rhs.GetValue())) > 0.1f);
        }

        override public void SetValue(object value)
        {
            if (Value != (float)value)
            {
                ValueChanged();
                Value = (float)value;
            }
        }
    }
}