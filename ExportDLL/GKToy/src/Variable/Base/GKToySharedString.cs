namespace GKToy
{
    [System.Serializable]
    public class GKToySharedString : GKToyShardVariable<string>
    {
        public GKToySharedString()
        {
            Value = "";
        }

        static public implicit operator GKToySharedString(string value) { return new GKToySharedString { Value = value }; }

        override public void SetValue(object value)
        {
            if(Value != (string)value)
            {
                ValueChanged();
                Value = (string)value;
            }
        }
    }
}