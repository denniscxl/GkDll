namespace GKToy
{
    [System.Serializable]
    public class GKToySharedBool : GKToyShardVariable<bool>
    {
        static public implicit operator GKToySharedBool(bool value) { return new GKToySharedBool { Value = value }; }

        override public void SetValue(object value)
        {
            if(Value != (bool)value)
            {
                ValueChanged();
                Value = (bool)value;
            }
        }
    }
}