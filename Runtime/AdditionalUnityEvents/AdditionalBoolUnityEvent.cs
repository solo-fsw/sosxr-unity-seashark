using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.AdditionalUnityEvents
{
    public class AdditionalBoolUnityEvent : MonoBehaviour
    {
        [SerializeField] protected UnityEvent<bool> m_eventToFire;


        [ContextMenu(nameof(FireEvent))]
        protected void FireEvent(bool input)
        {
            m_eventToFire?.Invoke(input);

            Debug.LogFormat("Fired event on {0} with input {1}", gameObject.name, input);
        }
    }
}