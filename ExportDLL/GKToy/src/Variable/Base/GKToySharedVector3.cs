using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedVector3 : GKToyShardVariable<Vector3>
    {
        static public implicit operator GKToySharedVector3(Vector3 value) { return new GKToySharedVector3 { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (Vector3)value)
            {
                ValueChanged();
                Value = (Vector3)value;
            }
        }
    }
}