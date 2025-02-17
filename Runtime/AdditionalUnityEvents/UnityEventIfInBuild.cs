namespace SOSXR.AdditionalUnityEvents
{
    public class UnityEventIfInBuild : AdditionalUnityEvent
    {
        private void Awake()
        {
            #if !UNITY_EDITOR
            FireEvent();
            #endif
        }
    }
}