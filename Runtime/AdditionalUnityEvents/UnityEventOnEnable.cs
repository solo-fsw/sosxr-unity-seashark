namespace SOSXR.AdditionalUnityEvents
{
    public class UnityEventOnEnable : AdditionalUnityEvent
    {
        private void OnEnable()
        {
            FireEvent();
        }
    }
}