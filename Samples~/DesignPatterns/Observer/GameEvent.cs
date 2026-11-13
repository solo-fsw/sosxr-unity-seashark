using System.Collections.Generic;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Represents a ScriptableObject-based event channel implementing the Observer pattern.
    ///     <para>
    ///         This asset is the subject that publishes notifications to subscribed listeners without direct references.
    ///         Publishers call <see cref="Raise"/> and subscribers react through <see cref="GameEventListener"/> components.
    ///     </para>
    ///     <para>
    ///         Compared to C# events and <c>Action</c> delegates, ScriptableObject events are Inspector-configurable, scene-agnostic, and resilient across assembly reload workflows.
    ///         They are ideal for cross-system messages such as <c>TrialStarted</c>, <c>ResponseRecorded</c>, or <c>BlockCompleted</c>.
    ///     </para>
    /// </summary>
    [CreateAssetMenu(fileName = "New Game Event", menuName = "SOSXR/Design Patterns/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new();


        /// <summary>
        ///     Raises this event and notifies every currently registered listener.
        /// </summary>
        public void Raise()
        {
            for (var i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i] == null)
                {
                    _listeners.RemoveAt(i);
                    continue;
                }

                _listeners[i].OnEventRaised();
            }
        }


        /// <summary>
        ///     Registers a listener to receive notifications when <see cref="Raise"/> is called.
        /// </summary>
        /// <param name="listener">The listener instance to register.</param>
        public void RegisterListener(GameEventListener listener)
        {
            if (listener == null)
            {
                return;
            }

            if (_listeners.Contains(listener))
            {
                return;
            }

            _listeners.Add(listener);
        }


        /// <summary>
        ///     Unregisters a previously registered listener.
        /// </summary>
        /// <param name="listener">The listener instance to remove.</param>
        public void UnregisterListener(GameEventListener listener)
        {
            if (listener == null)
            {
                return;
            }

            _listeners.Remove(listener);
        }
    }
}
