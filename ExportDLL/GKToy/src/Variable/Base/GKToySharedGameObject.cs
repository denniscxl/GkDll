using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedGameObject : GKToyShardVariable<GameObject>
    {
        static public implicit operator GKToySharedGameObject(GameObject value) { return new GKToySharedGameObject { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (GameObject)value)
            {
                ValueChanged();
                Value = (GameObject)value;
            }
        }
    }
}