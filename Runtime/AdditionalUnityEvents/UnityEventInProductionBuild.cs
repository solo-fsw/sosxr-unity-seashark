namespace SOSXR.AdditionalUnityEvents
{
    public class UnityEventInProductionBuild : AdditionalUnityEvent
    {
        private void Awake()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            // Do nothing
            #else
        FireEvent();
            #endif
        }
    }
}