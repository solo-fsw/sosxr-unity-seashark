namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData.ConfigToUnityEvent
{
    public class ConfigStringToUnityEvent : ConfigValueToUnityEvent<string>
    {
        protected override void FireEvent(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "";
            }

            EventToFire?.Invoke(value);
        }
    }
}