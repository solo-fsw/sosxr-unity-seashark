using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.AdditionalUnityEvents
{
    [RequireComponent(typeof(Collider))]
    public class UnityEventIfOnTriggerEnter : MonoBehaviour
    {
        [Header("Tag")]
        [SerializeField] private bool m_checkSpecificTag = true;
        [SerializeField] private string[] m_tagToCheckAgainst = {"FaceChooser"};

        [Header("Events")]
        [SerializeField] [CanBeNull] private UnityEvent<Collider> m_eventToFire;


        private void OnTriggerEnter(Collider other)
        {
            if (m_checkSpecificTag && !m_tagToCheckAgainst.Contains(other.tag))
            {
                return;
            }

            FireEvent(other);

            Debug.Log("Triggered UnityEvent on " + gameObject.name);
        }


        [ContextMenu(nameof(FireEvent))]
        private void FireEvent(Collider other = null)
        {
            m_eventToFire?.Invoke(other);
        }
    }
}