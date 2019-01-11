using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedRect : GKToyShardVariable<Rect>
    {
        static public implicit operator GKToySharedRect(Rect value) { return new GKToySharedRect { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Rect)value)
            {
                ValueChanged();
                Value = (Rect)value;
            }
        }
    }
}