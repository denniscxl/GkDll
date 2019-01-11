using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedColor : GKToyShardVariable<Color>
    {
        static public implicit operator GKToySharedColor(Color value) { return new GKToySharedColor { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Color)value)
            {
                ValueChanged();
                Value = (Color)value;
            }
        }
    }
}