namespace SOSXR.SeaShark
{
    public class ConfigBoolToUnityEvent : ConfigValueToUnityEvent<bool>
    {
        public bool Invert;


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