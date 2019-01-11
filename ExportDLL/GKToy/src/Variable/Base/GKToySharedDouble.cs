using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedDouble : GKToyShardVariable<double>
    {
        static public implicit operator GKToySharedDouble(double value) { return new GKToySharedDouble { Value = value }; }

        override public void SetValue(object value)
        {
            if (Value != (double)value)
            {
                ValueChanged();
                Value = (double)value;
            }
        }
    }
}