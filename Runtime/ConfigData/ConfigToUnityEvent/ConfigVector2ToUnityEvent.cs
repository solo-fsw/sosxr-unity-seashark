using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData.ConfigToUnityEvent
{
    public class ConfigVector2ToUnityEvent : ConfigValueToUnityEvent<Vector2>
    {
        protected override void FireEvent(Vector2 value)
        {
            EventToFire?.Invoke(value);
        }
    }
}