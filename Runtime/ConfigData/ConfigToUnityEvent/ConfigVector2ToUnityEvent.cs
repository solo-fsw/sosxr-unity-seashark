using UnityEngine;


namespace SOSXR.SeaShark
{
    public class ConfigVector2ToUnityEvent : ConfigValueToUnityEvent<Vector2>
    {
        protected override void FireEvent(Vector2 value)
        {
            EventToFire?.Invoke(value);
        }
    }
}