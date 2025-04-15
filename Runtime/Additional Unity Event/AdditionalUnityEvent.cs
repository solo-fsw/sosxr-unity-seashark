using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace SOSXR.SeaShark
{
    public class AdditionalUnityEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_eventToFire;
        [SerializeField] private LifeCycleTriggerType m_triggerType = LifeCycleTriggerType.Awake;
        [SerializeField] private BuildTriggerType m_buildTriggerType = BuildTriggerType.Always;
        [SerializeField] [Range(0f, 10f)] private float m_delayInSeconds = 0f;

        [Header("Optional")]
        [SerializeField] [Optional(OptionalType.WillFind)] private string[] m_tags = { };
        [SerializeField] [Optional(OptionalType.WillGet)] private InputActionProperty m_inputAction;

        private Coroutine _activeCoroutine;


        private void Awake()
        {
            if (m_triggerType == LifeCycleTriggerType.Awake)
            {
                FireEvent();
            }
        }


        private void OnEnable()
        {
            if (m_triggerType == LifeCycleTriggerType.InputAction && m_inputAction.action != null)
            {
                m_inputAction.action.Enable();
                m_inputAction.action.performed += context => FireEvent();
            }

            if (m_triggerType == LifeCycleTriggerType.OnEnable)
            {
                FireEvent();
            }
        }


        private void Start()
        {
            if (m_triggerType == LifeCycleTriggerType.Start)
            {
                FireEvent();
            }
        }


        private void Update()
        {
            if (m_triggerType == LifeCycleTriggerType.Update)
            {
                FireEvent();
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (m_triggerType == LifeCycleTriggerType.TriggerEnter && ShouldFireForTag(other.tag))
            {
                FireEvent();
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (m_triggerType == LifeCycleTriggerType.TriggerExit && ShouldFireForTag(other.tag))
            {
                FireEvent();
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (m_triggerType == LifeCycleTriggerType.CollisionEnter && ShouldFireForTag(collision.gameObject.tag))
            {
                FireEvent();
            }
        }


        private void OnCollisionExit(Collision collision)
        {
            if (m_triggerType == LifeCycleTriggerType.CollisionExit && ShouldFireForTag(collision.gameObject.tag))
            {
                FireEvent();
            }
        }


        private bool ShouldFireForTag(string tag)
        {
            return m_tags.Length == 0 || m_tags.Contains(tag);
        }


        public void FireEvent()
        {
            if (!ShouldFire())
            {
                return;
            }

            if (m_delayInSeconds > 0)
            {
                if (_activeCoroutine != null)
                {
                    StopCoroutine(_activeCoroutine);

                    return;
                }

                _activeCoroutine = StartCoroutine(FireEventCR());
            }
            else
            {
                m_eventToFire?.Invoke();
            }
        }


        private IEnumerator FireEventCR()
        {
            yield return new WaitForSeconds(m_delayInSeconds);
            m_eventToFire?.Invoke();
            _activeCoroutine = null;
        }


        public void CancelEvent()
        {
            if (_activeCoroutine == null)
            {
                return;
            }

            Debug.LogFormat(this, "An active UnityEvent was cancelled. It may mean that the event was not fired, or not fired at the right time.");
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

                FireEvent();
            }

            if (m_triggerType == LifeCycleTriggerType.InputAction && m_inputAction.action != null)
            {
                m_inputAction.action.Disable();
            }

            CancelEvent();
        }


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
            if (buildTriggerType == BuildTriggerType.OnlyInBuilds && !Application.isPlaying)
            {
                return true;
            }
            #endif

            #if !UNITY_EDITOR && DEVELOPMENT_BUILD
            if (buildTriggerType == BuildTriggerType.OnlyInDevelopmentBuilds )
            {
                return true;
            }
            #endif

            #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            if (buildTriggerType == BuildTriggerType.OnlyInProductionBuilds)
            {
                return true;
            }
            #endif

            Debug.LogError("BuildTriggerType is not set correctly. Please check the settings.");

            return false;
        }
    }


    public enum BuildTriggerType
    {
        Always,
        OnlyInEditor,
        OnlyInBuilds,
        OnlyInDevelopmentBuilds,
        OnlyInProductionBuilds
    }


    // Simplified enum of trigger types
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
        InputAction
    }
}