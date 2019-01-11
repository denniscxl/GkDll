using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedMatrix4x4 : GKToyShardVariable<Matrix4x4>
    {
        static public implicit operator GKToySharedMatrix4x4(Matrix4x4 value) { return new GKToySharedMatrix4x4 { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Matrix4x4)value)
            {
                ValueChanged();
                Value = (Matrix4x4)value;
            }
        }
    }
}
