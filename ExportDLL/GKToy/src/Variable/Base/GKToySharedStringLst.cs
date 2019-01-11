using System.Collections.Generic;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedStringLst : GKToyShardLstVariable<string>
    {
        static public implicit operator GKToySharedStringLst(List<string> value) { return new GKToySharedStringLst { Value = value.ToArray() }; }

        override public void AddCapacity()
        {
            if (null == _value)
                _value = new List<string>();

            _value.Add("");
            ValueChanged();
        }
    }
}