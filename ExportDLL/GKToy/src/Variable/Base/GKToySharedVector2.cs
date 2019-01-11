using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedVector2 : GKToyShardVariable<Vector2>
    {
        static public implicit operator GKToySharedVector2(Vector2 value) { return new GKToySharedVector2 { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Vector2)value)
            {
                ValueChanged();
                Value = (Vector2)value;
            }
        }
    }
}