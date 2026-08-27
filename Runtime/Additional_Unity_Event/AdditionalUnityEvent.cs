using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SOSXR.SeaShark
{
    /// <summary>
    /// Fires a configured <see cref="UnityEvent"/> from Unity lifecycle callbacks, physics callbacks,
    /// or an input action.
    /// </summary>
    public class AdditionalUnityEvent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_eventToFire;

        [SerializeField]
        private LifeCycleTriggerType m_triggerType = LifeCycleTriggerType.Awake;

        [SerializeField]
        private BuildTriggerType m_buildTriggerType = BuildTriggerType.Always;

        [SerializeField]
        [Range(0f, 10f)]
        private float m_delayInSeconds;

        [Header("Optional")]
        [SerializeField]
        [Optional(OptionalType.WillFind)]
        // [TagSelector]
        private string[] m_tags = { };

        [SerializeField]
        [Optional(OptionalType.WillGet)]
        private InputActionProperty m_inputAction;

        private Coroutine _activeCoroutine;
        private WaitForSeconds _cachedDelayWait;
        private float _cachedDelaySeconds = -1f;

        private void Awake()
        {
            if (m_triggerType == LifeCycleTriggerType.Awake)
            {
                SafeFireEvent();
            }
        }

        private void OnEnable()
        {
            if (m_triggerType == LifeCycleTriggerType.InputAction && m_inputAction.action != null)
            {
                m_inputAction.action.Enable();
                m_inputAction.action.performed += HandleInputActionPerformed;
            }

            if (m_triggerType == LifeCycleTriggerType.OnEnable)
            {
                SafeFireEvent();
            }
        }

        private void Start()
        {
            if (m_triggerType == LifeCycleTriggerType.Start)
            {
                SafeFireEvent();
            }
        }

        /// <summary>
        /// Handles the configured input action without allocating a per-enable lambda.
        /// </summary>
        /// <param name="context">Input action callback context.</param>
        private void HandleInputActionPerformed(InputAction.CallbackContext context)
        {
            SafeFireEvent();
        }

        private void Update()
        {
            if (m_triggerType == LifeCycleTriggerType.Update)
            {
                SafeFireEvent();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_triggerType == LifeCycleTriggerType.TriggerEnter && ShouldFireForTag(other.gameObject))
            {
                SafeFireEvent();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_triggerType == LifeCycleTriggerType.TriggerExit && ShouldFireForTag(other.gameObject))
            {
                SafeFireEvent();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_triggerType == LifeCycleTriggerType.CollisionEnter && ShouldFireForTag(collision.gameObject))
            {
                SafeFireEvent();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (m_triggerType == LifeCycleTriggerType.CollisionExit && ShouldFireForTag(collision.gameObject))
            {
                SafeFireEvent();
            }
        }

        /// <summary>
        /// Checks optional tag filters for trigger/collision callbacks.
        /// </summary>
        /// <param name="otherGameObject">GameObject received by the physics callback.</param>
        /// <returns><c>true</c> when the event may fire for this object.</returns>
        private bool ShouldFireForTag(GameObject otherGameObject)
        {
            if (m_tags == null || m_tags.Length == 0)
            {
                return true;
            }

            // CompareTag avoids the string allocation caused by reading .tag in hot callbacks.
            for (var i = 0; i < m_tags.Length; i++)
            {
                var configuredTag = m_tags[i];

                if (string.IsNullOrEmpty(configuredTag))
                {
                    continue;
                }

                if (otherGameObject.CompareTag(configuredTag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fires the event only when the current build filter allows it.
        /// </summary>
        [Button]
        public void SafeFireEvent()
        {
            if (!ShouldFire())
            {
                return;
            }

            FireEvent();
        }

        /// <summary>
        /// Fires the event immediately or starts the delayed invocation coroutine.
        /// </summary>
        [Button]
        public void FireEvent()
        {
            if (m_delayInSeconds > 0)
            {
                if (_activeCoroutine != null)
                {
                    StopCoroutine(_activeCoroutine);

                    return;
                }

                if (!enabled)
                {
                    return;
                }

                _activeCoroutine = StartCoroutine(FireEventCR());
            }
            else
            {
                m_eventToFire?.Invoke();
            }
        }

        /// <summary>
        /// Delays the configured event invocation.
        /// </summary>
        private IEnumerator FireEventCR()
        {
            yield return GetDelayWaitInstruction();

            m_eventToFire?.Invoke();

            _activeCoroutine = null;
        }

        /// <summary>
        /// Reuses the same wait instruction while the configured delay stays unchanged.
        /// </summary>
        /// <returns>Cached wait instruction for the current delay.</returns>
        private WaitForSeconds GetDelayWaitInstruction()
        {
            if (_cachedDelayWait == null || !Mathf.Approximately(_cachedDelaySeconds, m_delayInSeconds))
            {
                _cachedDelaySeconds = m_delayInSeconds;
                _cachedDelayWait = new WaitForSeconds(m_delayInSeconds);
            }

            return _cachedDelayWait;
        }

        /// <summary>
        /// Cancels a pending delayed invocation, if one exists.
        /// </summary>
        public void CancelEvent()
        {
            if (_activeCoroutine == null)
            {
                return;
            }

            // Debug.Log("An active UnityEvent was cancelled. It may mean that the event was not fired, or not fired at the right time.");
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }

        private void OnDisable()
        {
            if (m_triggerType == LifeCycleTriggerType.OnDisable)
            {
                if (m_delayInSeconds > 0)
                {
                    Debug.LogWarning("We cannot fire an event in OnDisable with a delay. Will fire immediately.");
                }

                m_eventToFire?.Invoke(); // You cannot fire a coroutine in OnDisable, so we fire the event immediately.
            }

            if (m_triggerType == LifeCycleTriggerType.InputAction && m_inputAction.action != null)
            {
                m_inputAction.action.performed -= HandleInputActionPerformed;
                m_inputAction.action.Disable();
            }

            CancelEvent();
        }

        /// <summary>
        /// Checks whether this event is allowed in the current build/runtime context.
        /// </summary>
        /// <returns><c>true</c> when the configured build filter matches.</returns>
        public bool ShouldFire()
        {
            if (m_buildTriggerType == BuildTriggerType.Always)
            {
                return true;
            }

#if UNITY_EDITOR
            if (m_buildTriggerType == BuildTriggerType.OnlyInEditor && Application.isPlaying)
            {
                return true;
            }
#endif

#if !UNITY_EDITOR
            if (m_buildTriggerType == BuildTriggerType.OnlyInBuilds && !Application.isPlaying)
            {
                return true;
            }
#endif

#if !UNITY_EDITOR && DEVELOPMENT_BUILD
            if (m_buildTriggerType == BuildTriggerType.OnlyInDevelopmentBuilds)
            {
                return true;
            }
#endif

#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            if (m_buildTriggerType == BuildTriggerType.OnlyInProductionBuilds)
            {
                return true;
            }
#endif

            Debug.LogError("BuildTriggerType is not set correctly. Please check the settings.");

            return false;
        }
    }

    /// <summary>
    /// Filters when an <see cref="AdditionalUnityEvent"/> may fire based on build type.
    /// </summary>
    public enum BuildTriggerType
    {
        Always,
        OnlyInEditor,
        OnlyInBuilds,
        OnlyInDevelopmentBuilds,
        OnlyInProductionBuilds,
    }

    /// <summary>
    /// Defines which Unity callback should trigger the event.
    /// </summary>
    public enum LifeCycleTriggerType
    {
        Awake,
        OnEnable,
        Start,
        Update,
        TriggerEnter,
        TriggerExit,
        CollisionEnter,
        CollisionExit,
        OnDisable,
        InputAction,
        Never,
    }
}
