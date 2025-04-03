namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData.ConfigToUnityEvent
{
    public class ConfigIntToUnityEvent : ConfigValueToUnityEvent<int>
    {
        protected override void FireEvent(int value)
        {
            EventToFire?.Invoke(value);
        }
    }
}