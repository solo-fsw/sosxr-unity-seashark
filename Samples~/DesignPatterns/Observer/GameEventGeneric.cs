using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Defines a typed ScriptableObject event channel for the Observer pattern.
    ///     This generic event publishes payload data so observers can react with context, such as reaction time values, trial indices, or condition labels.
    /// </summary>
    /// <typeparam name="T">The payload type carried by this event.</typeparam>
    public abstract class GameEvent<T> : ScriptableObject
    {
        private readonly List<GameEventListener<T>> _listeners = new();


        /// <summary>
        ///     Raises this event and passes a typed payload value to all registered listeners.
        /// </summary>
        /// <param name="value">The payload value to broadcast.</param>
        public void Raise(T value)
        {
            for (var i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i] == null)
                {
                    _listeners.RemoveAt(i);
                    continue;
                }

                _listeners[i].OnEventRaised(value);
            }
        }


        /// <summary>
        ///     Registers a typed listener to this event channel.
        /// </summary>
        /// <param name="listener">The listener instance to register.</param>
        public void RegisterListener(GameEventListener<T> listener)
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
        ///     Unregisters a previously registered typed listener.
        /// </summary>
        /// <param name="listener">The listener instance to remove.</param>
        public void UnregisterListener(GameEventListener<T> listener)
        {
            if (listener == null)
            {
                return;
            }

            _listeners.Remove(listener);
        }
    }


    /// <summary>
    ///     Listens to a typed <see cref="GameEvent{T}"/> and invokes a typed response when raised.
    ///     Derive concrete listeners for Inspector-friendly binding of payload event assets and UnityEvents.
    /// </summary>
    /// <typeparam name="T">The payload type consumed by this listener.</typeparam>
    public abstract class GameEventListener<T> : MonoBehaviour
    {
        /// <summary>
        ///     Called by the observed event when a payload is raised.
        /// </summary>
        /// <param name="value">The payload value supplied by the event source.</param>
        public abstract void OnEventRaised(T value);
    }


    /// <summary>
    ///     Typed UnityEvent specialization for float payloads.
    /// </summary>
    [Serializable]
    public class FloatUnityEvent : UnityEvent<float>
    {
    }


    /// <summary>
    ///     Typed UnityEvent specialization for int payloads.
    /// </summary>
    [Serializable]
    public class IntUnityEvent : UnityEvent<int>
    {
    }


    /// <summary>
    ///     Typed UnityEvent specialization for string payloads.
    /// </summary>
    [Serializable]
    public class StringUnityEvent : UnityEvent<string>
    {
    }


    /// <summary>
    ///     Concrete float payload event asset.
    /// </summary>
    [CreateAssetMenu(fileName = "New Float Game Event", menuName = "SOSXR/Design Patterns/Game Event/Float")]
    public class FloatGameEvent : GameEvent<float>
    {
    }


    /// <summary>
    ///     Concrete int payload event asset.
    /// </summary>
    [CreateAssetMenu(fileName = "New Int Game Event", menuName = "SOSXR/Design Patterns/Game Event/Int")]
    public class IntGameEvent : GameEvent<int>
    {
    }


    /// <summary>
    ///     Concrete string payload event asset.
    /// </summary>
    [CreateAssetMenu(fileName = "New String Game Event", menuName = "SOSXR/Design Patterns/Game Event/String")]
    public class StringGameEvent : GameEvent<string>
    {
    }


    /// <summary>
    ///     Inspector-ready listener for <see cref="FloatGameEvent"/> payload notifications.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Float Game Event Listener")]
    public class FloatGameEventListener : GameEventListener<float>
    {
        [SerializeField] private FloatGameEvent m_event;
        [SerializeField] private FloatUnityEvent m_response;


        /// <summary>
        ///     Invokes the configured float response callback with the raised payload.
        /// </summary>
        /// <param name="value">The payload value from the event channel.</param>
        public override void OnEventRaised(float value)
        {
            m_response?.Invoke(value);
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


    /// <summary>
    ///     Inspector-ready listener for <see cref="IntGameEvent"/> payload notifications.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Int Game Event Listener")]
    public class IntGameEventListener : GameEventListener<int>
    {
        [SerializeField] private IntGameEvent m_event;
        [SerializeField] private IntUnityEvent m_response;


        /// <summary>
        ///     Invokes the configured int response callback with the raised payload.
        /// </summary>
        /// <param name="value">The payload value from the event channel.</param>
        public override void OnEventRaised(int value)
        {
            m_response?.Invoke(value);
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


    /// <summary>
    ///     Inspector-ready listener for <see cref="StringGameEvent"/> payload notifications.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/String Game Event Listener")]
    public class StringGameEventListener : GameEventListener<string>
    {
        [SerializeField] private StringGameEvent m_event;
        [SerializeField] private StringUnityEvent m_response;


        /// <summary>
        ///     Invokes the configured string response callback with the raised payload.
        /// </summary>
        /// <param name="value">The payload value from the event channel.</param>
        public override void OnEventRaised(string value)
        {
            m_response?.Invoke(value);
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
