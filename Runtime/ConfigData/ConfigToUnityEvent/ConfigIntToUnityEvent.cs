namespace SOSXR.SeaShark
{
    public class ConfigIntToUnityEvent : ConfigValueToUnityEvent<int>
    {
        protected override void FireEvent(int value)
        {
            EventToFire?.Invoke(value);
        }
    }
}