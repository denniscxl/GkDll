using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedBounds : GKToyShardVariable<Bounds>
    {
        static public implicit operator GKToySharedBounds(Bounds value) { return new GKToySharedBounds { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Bounds)value)
            {
                ValueChanged();
                Value = (Bounds)value;
            }
        }
    }
}