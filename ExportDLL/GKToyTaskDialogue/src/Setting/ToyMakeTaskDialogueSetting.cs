using UnityEngine;
using static ToyMakerSettings;
[CreateAssetMenu(fileName = "ToyMakerTaskDialogueSettings", menuName = "(custom) X")]
public class ToyMakerTaskDialogueSettings : ScriptableObject
{
    [System.Serializable]
    public class ToyMakerTaskDialogue : ToyMakerBase
    {
    }
    public ToyMakerTaskDialogue toyMakerBase = new ToyMakerTaskDialogue();
}
