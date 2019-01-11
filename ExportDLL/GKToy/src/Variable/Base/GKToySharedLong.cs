using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedLong : GKToyShardVariable<long>
    {
        static public implicit operator GKToySharedLong(long value) { return new GKToySharedLong { Value = value }; }

        static public GKToySharedLong operator +(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            GKToySharedLong result = new GKToySharedLong();
            result = (long)(lhs.GetValue()) + (long)(rhs.GetValue());
            return result;
        }

        static public GKToySharedLong operator -(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            GKToySharedLong result = new GKToySharedLong();
            result = (long)(lhs.GetValue()) - (long)(rhs.GetValue());
            return result;
        }

        static public GKToySharedLong operator *(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            GKToySharedLong result = new GKToySharedLong();
            result = (long)(lhs.GetValue()) * (long)(rhs.GetValue());
            return result;
        }

        static public GKToySharedLong operator /(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            GKToySharedLong result = new GKToySharedLong();
            result = (long)(lhs.GetValue()) / (long)(rhs.GetValue());
            return result;
        }

        static public GKToySharedLong operator %(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            GKToySharedLong result = new GKToySharedLong();
            result = (long)(lhs.GetValue()) % (long)(rhs.GetValue());
            return result;
        }

        static public bool operator ==(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            return (long)lhs.GetValue() == (long)rhs.GetValue();
        }

        static public bool operator !=(GKToySharedLong lhs, GKToySharedLong rhs)
        {
            return (long)lhs.GetValue() != (long)rhs.GetValue();
        }

        override public void SetValue(object value)
        {
            if (Value != (long)value)
            {
                ValueChanged();
                Value = (long)value;
            }
        }
    }
}