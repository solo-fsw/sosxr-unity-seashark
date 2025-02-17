using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.AdditionalUnityEvents
{
    public class AdditionalUnityEvent : MonoBehaviour
    {
        [SerializeField] protected UnityEvent m_eventToFire;


        [ContextMenu(nameof(FireEvent))]
        protected void FireEvent()
        {
            m_eventToFire?.Invoke();

            Debug.LogFormat("Fired event on {0}", gameObject.name);
        }
    }
}