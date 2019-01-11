using System.Collections.Generic;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedIntLst : GKToyShardLstVariable<int>
    {
        static public implicit operator GKToySharedIntLst(List<int> value) { return new GKToySharedIntLst { Value = value.ToArray() }; }
    }
}