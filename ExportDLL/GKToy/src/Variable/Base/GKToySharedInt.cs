namespace GKToy
{
    [System.Serializable]
    public class GKToySharedInt : GKToyShardVariable<int>
    {
        static public implicit operator GKToySharedInt(int value) { return new GKToySharedInt { Value = value }; }

        static public GKToySharedInt operator +(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            GKToySharedInt result = new GKToySharedInt();
            result = (int)(lhs.GetValue()) + (int)(rhs.GetValue());
            return result;
        }

        static public GKToySharedInt operator -(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            GKToySharedInt result = new GKToySharedInt();
            result = (int)(lhs.GetValue()) - (int)(rhs.GetValue());
            return result;
        }

        static public GKToySharedInt operator *(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            GKToySharedInt result = new GKToySharedInt();
            result = (int)(lhs.GetValue()) * (int)(rhs.GetValue());
            return result;
        }

        static public GKToySharedInt operator /(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            GKToySharedInt result = new GKToySharedInt();
            result = (int)(lhs.GetValue()) / (int)(rhs.GetValue());
            return result;
        }

        static public GKToySharedInt operator %(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            GKToySharedInt result = new GKToySharedInt();
            result = (int)(lhs.GetValue()) % (int)(rhs.GetValue());
            return result;
        }

        static public bool operator ==(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            return (int)lhs.GetValue() == (int)rhs.GetValue();
        }

        static public bool operator !=(GKToySharedInt lhs, GKToySharedInt rhs)
        {
            return (int)lhs.GetValue() != (int)rhs.GetValue();
        }

        override public void SetValue(object value)
        {
            if (Value != (int)value)
            {
                ValueChanged();
                Value = (int)value;
            }
        }
    }
}