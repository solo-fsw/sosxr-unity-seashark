namespace SOSXR.AdditionalUnityEvents
{
    public class UnityEventOnAwake : AdditionalUnityEvent
    {
        private void Awake()
        {
            FireEvent();
        }
    }
}