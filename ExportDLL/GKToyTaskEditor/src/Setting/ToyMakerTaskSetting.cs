using System;
using UnityEngine;
using static ToyMakerSettings;

public class ToyMakerTaskSettings : ScriptableObject
{
    [System.Serializable]
    public class ToyMakerTask : ToyMakerBase
    {
    }
    public ToyMakerTask toyMakerBase = new ToyMakerTask();
}
