using UnityEngine;
using static ToyMakerSettings;

public class ToyMakerDialogueSettings : ScriptableObject
{
    [System.Serializable]
    public class ToyMakerDialogue : ToyMakerBase
    {
    }
    public ToyMakerDialogue toyMakerBase = new ToyMakerDialogue();
}
