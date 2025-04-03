namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData.ConfigToUnityEvent
{
    public class ConfigBoolToUnityEvent : ConfigValueToUnityEvent<bool>
    {
        public bool Invert = false;


        protected override void FireEvent(bool value)
        {
            if (Invert)
            {
                value = !value;
            }

            EventToFire?.Invoke(value);
        }
    }
}