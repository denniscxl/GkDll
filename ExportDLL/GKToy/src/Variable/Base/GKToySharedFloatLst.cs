using UnityEngine;
using System.Collections.Generic;

namespace GKToy
{
    [System.Serializable]
    public class GKToySharedFloatLst : GKToyShardLstVariable<float>
    {
        static public implicit operator GKToySharedFloatLst(List<float> value) { return new GKToySharedFloatLst { Value = value.ToArray() }; }
    }
}