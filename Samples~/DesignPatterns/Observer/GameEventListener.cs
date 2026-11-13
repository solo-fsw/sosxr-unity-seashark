using UnityEngine;
using UnityEngine.Events;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Attach this to any GameObject to react to a <see cref="GameEvent"/>.
    ///     Wire the response in the Inspector so systems can communicate without direct code coupling.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Game Event Listener")]
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent m_event;
        [SerializeField] private UnityEvent m_response;


        /// <summary>
        ///     Invoked by <see cref="GameEvent"/> when the observed event is raised.
        /// </summary>
        public void OnEventRaised()
        {
            m_response?.Invoke();
        }


        private void OnEnable()
        {
            if (m_event == null)
            {
                return;
            }

            m_event.RegisterListener(this);
        }


        private void OnDisable()
        {
            if (m_event == null)
            {
                return;
            }

            m_event.UnregisterListener(this);
        }
    }
}
