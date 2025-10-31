using UnityEngine;


namespace SOSXR.SeaShark
{
    public class ConfigVector3ToUnityEvent : ConfigValueToUnityEvent<Vector3>
    {
        protected override void FireEvent(Vector3 value)
        {
            EventToFire?.Invoke(value);
        }
    }
}