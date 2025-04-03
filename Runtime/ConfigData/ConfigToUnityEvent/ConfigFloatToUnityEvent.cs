namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData.ConfigToUnityEvent
{
    public class ConfigFloatToUnityEvent : ConfigValueToUnityEvent<float>
    {
        protected override void FireEvent(float value)
        {
            EventToFire?.Invoke(value);
        }
    }
}