using System;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedEnum : GKToyShardVariable<Enum>
    {
        static public implicit operator GKToySharedEnum(Enum value) { return new GKToySharedEnum { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Enum)value)
            {
                ValueChanged();
                Value = (Enum)value;
            }
        }
    }
}