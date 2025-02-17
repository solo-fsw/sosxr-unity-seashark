using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.AdditionalUnityEvents
{
    public class AdditionalStringUnityEvent : MonoBehaviour
    {
        [SerializeField] protected UnityEvent<string> m_eventToFire;


        [ContextMenu(nameof(FireEvent))]
        protected void FireEvent(string input)
        {
            m_eventToFire?.Invoke(input);

            Debug.LogFormat("Fired event on {0} with input {1}", gameObject.name, input);
        }
    }
}