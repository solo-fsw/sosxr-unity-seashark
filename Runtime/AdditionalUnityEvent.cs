using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;



namespace SOSXR.SimpleHelpers
{
    public class AdditionalUnityEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_eventToFire;
        [SerializeField] private UnityEventTrigger m_triggerEventOn = UnityEventTrigger.Awake;
        [SerializeField] [Range(0f, 60f)] private float m_secondsToWaitAfterCalling = 0f;
        [Tooltip("Tags to check against when using Trigger or Collision events. Leave empty to fire on any tag.")]
      
        [SerializeField] private string[] m_tagsToCheckAgainst = { };

        [SerializeField] private InputActionProperty m_inputAction;

        [SerializeField] private bool m_autoStart;
        [SerializeField] [Range(0f, 60f)] private float m_initialFireInterval = 1.3f;
        [SerializeField] [Range(0f, 60f)] private float m_perIntervalChange = 0.02f;

        [SerializeField] private Vector2 m_minMax = new(0.5f, 2f);

        [Header("Not for editing")]
        [SerializeField] private float m_currentFireInterval;

        private Coroutine _coroutine;
        private bool _timeWarningShown;
        private float _lastFireTime;


        private void OnValidate()
        {
            if (m_triggerEventOn.HasFlag(UnityEventTrigger.VariableIntervalLoop) && m_triggerEventOn != UnityEventTrigger.VariableIntervalLoop)
            {
                Debug.LogWarning("VariableIntervalLoop should be set alone, or it will not work as expected.");
            }

            if (m_secondsToWaitAfterCalling > 0 &&
                (m_triggerEventOn.HasFlag(UnityEventTrigger.Update) ||
                 m_triggerEventOn.HasFlag(UnityEventTrigger.LateUpdate) ||
                 m_triggerEventOn.HasFlag(UnityEventTrigger.FixedUpdate) ||
                 m_triggerEventOn.HasFlag(UnityEventTrigger.OnTriggerStay) ||
                 m_triggerEventOn.HasFlag(UnityEventTrigger.OnCollisionStay)))
            {
                Debug.LogWarning("Time delay set with repeated calls may impact performance and/or is redundant. Consider using a different event trigger, or setting the delay to 0.");
            }
        }


        private void Awake()
        {
            if (m_triggerEventOn.HasFlag(UnityEventTrigger.Awake))
            {
                FireEvent();
            }

            if (m_triggerEventOn.HasFlag(UnityEventTrigger.VariableIntervalLoop) && m_autoStart)
            {
                FireEvent();
            }

            if (m_triggerEventOn.HasFlag(UnityEventTrigger.InDevelopmentBuild))
            {
                #if DEVELOPMENT_BUILD
                    FireEvent();
                #endif
            }

            if (m_triggerEventOn.HasFlag(UnityEventTrigger.InProductionBuild))
            {
                #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
                    FireEvent();
                #endif
            }
        }


        private void OnEnable()
        {
            if (m_triggerEventOn.HasFlag(UnityEventTrigger.OnEnable))
            {
                FireEvent();
            }

            if (m_triggerEventOn.HasFlag(UnityEventTrigger.OnInputAction) && m_inputAction.action != null)
            {
                m_inputAction.action.Enable();
                m_inputAction.action.performed += FireEvent;
            }
        }


        private void FireEvent(InputAction.CallbackContext callback)
        {
            FireEvent();
        }


        private void Start()
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.Start))
            {
                return;
            }

            FireEvent();
        }


        private void FixedUpdate()
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.FixedUpdate))
            {
                return;
            }

            TimeWarning();
            FireEvent();
        }


        private void Update()
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.Update))
            {
                return;
            }

            TimeWarning();
            FireEvent();
        }


        private void LateUpdate()
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.LateUpdate))
            {
                return;
            }

            TimeWarning();
            FireEvent();
        }


        private void OnApplicationPause(bool pauseStatus)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnApplicationPause))
            {
                return;
            }

            FireEvent();
        }


        private void OnApplicationFocus(bool hasFocus)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnApplicationFocus))
            {
                return;
            }

            FireEvent();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnTriggerEnter))
            {
                return;
            }

            if (m_tagsToCheckAgainst.Length > 0 && !m_tagsToCheckAgainst.Contains(other.tag))
            {
                return;
            }

            FireEvent();
        }


        private void OnTriggerStay(Collider other)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnTriggerStay))
            {
                return;
            }

            if (m_tagsToCheckAgainst.Length > 0 && !m_tagsToCheckAgainst.Contains(other.tag))
            {
                return;
            }

            TimeWarning();
            FireEvent();
        }


        private void OnTriggerExit(Collider other)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnTriggerExit))
            {
                return;
            }

            if (m_tagsToCheckAgainst.Length > 0 && !m_tagsToCheckAgainst.Contains(other.tag))
            {
                return;
            }

            FireEvent();
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnCollisionEnter))
            {
                return;
            }

            if (m_tagsToCheckAgainst.Length > 0 && !m_tagsToCheckAgainst.Contains(collision.collider.tag))
            {
                return;
            }

            FireEvent();
        }


        private void OnCollisionStay(Collision collision)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnCollisionStay))
            {
                return;
            }

            if (m_tagsToCheckAgainst.Length > 0 && !m_tagsToCheckAgainst.Contains(collision.collider.tag))
            {
                return;
            }

            TimeWarning();
            FireEvent();
        }


        private void OnCollisionExit(Collision collision)
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnCollisionExit))
            {
                return;
            }

            if (m_tagsToCheckAgainst.Length > 0 && !m_tagsToCheckAgainst.Contains(collision.collider.tag))
            {
                return;
            }

            FireEvent();
        }


        private void OnDisable()
        {
            if (m_triggerEventOn.HasFlag(UnityEventTrigger.OnDisable))
            {
                FireEvent();
            }

            if (m_triggerEventOn.HasFlag(UnityEventTrigger.OnInputAction) && m_inputAction.action != null)
            {
                m_inputAction.action.performed -= FireEvent;
                m_inputAction.action.Disable();
            }
        }


        private void OnApplicationQuit()
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnApplicationQuit))
            {
                return;
            }

            FireEvent();
        }


        private void OnDestroy()
        {
            if (!m_triggerEventOn.HasFlag(UnityEventTrigger.OnDestroy))
            {
                return;
            }

            FireEvent();

            StopFiring();
        }


        private void TimeWarning()
        {
            if (_timeWarningShown || m_secondsToWaitAfterCalling <= 0)
            {
                return;
            }

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning($"Time delay set on {gameObject.name} may impact performance and/or is redundant when it is called on {m_triggerEventOn}. It may also come too late when firing on ApplicationQuit / OnDisable / OnDestroy. Consider using a different event trigger, or setting the delay to 0.");
            #endif

            _timeWarningShown = true;
        }


        private IEnumerator FireEventOnLoopCR()
        {
            m_currentFireInterval = m_initialFireInterval;

            for (;;)
            {
                FireEventImmediately();

                yield return new WaitForSeconds(m_currentFireInterval);

                if (m_currentFireInterval + m_perIntervalChange >= m_minMax.x && m_currentFireInterval + m_perIntervalChange <= m_minMax.y)
                {
                    m_currentFireInterval += m_perIntervalChange;
                    m_currentFireInterval = (float) Math.Round(m_currentFireInterval, 2, MidpointRounding.AwayFromZero);
                }
            }
        }


        [ContextMenu(nameof(FireEvent))]
        public void FireEvent()
        {
            if (m_triggerEventOn is UnityEventTrigger.OnApplicationQuit or UnityEventTrigger.OnDisable or UnityEventTrigger.OnDestroy)
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"Time delay set on {gameObject.name} is dangerous when called on {m_triggerEventOn}, since it will fire after the lifecycle of the object or the game. It will now force fire immediately. Consider using a different event trigger, or setting the delay to 0.");
                #endif

                FireEventImmediately();

                return;
            }

            if (m_secondsToWaitAfterCalling <= 0)
            {
                FireEventImmediately();

                return;
            }

            if (Time.time < _lastFireTime + m_secondsToWaitAfterCalling)
            {
                return;
            }

            if (_coroutine != null)
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning("Stopping previous coroutine");
                #endif

                StopCoroutine(_coroutine);
            }

            if (m_triggerEventOn.HasFlag(UnityEventTrigger.VariableIntervalLoop))
            {
                _coroutine = StartCoroutine(FireEventOnLoopCR());
            }
            else
            {
                _coroutine = StartCoroutine(FireEventCR());
            }

            _lastFireTime = Time.time;
        }


        private IEnumerator FireEventCR()
        {
            yield return new WaitForSeconds(m_secondsToWaitAfterCalling);

            FireEventImmediately();
            _coroutine = null;
        }


        public void FireEventImmediately()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogFormat("Fire event during {0} on {1} after {2} seconds", m_triggerEventOn, gameObject.name, m_secondsToWaitAfterCalling);
            #endif

            m_eventToFire?.Invoke();
        }


        [ContextMenu(nameof(StopFiring))]
        public void StopFiring()
        {
            StopAllCoroutines();
        }
    }


    [Flags]
    public enum UnityEventTrigger
    {
        None = 0,
        Awake = 1 << 0,
        OnEnable = 1 << 1,
        Start = 1 << 2,
        FixedUpdate = 1 << 3,
        Update = 1 << 4,
        LateUpdate = 1 << 5,
        OnApplicationPause = 1 << 6,
        OnApplicationFocus = 1 << 7,
        OnTriggerEnter = 1 << 8,
        OnTriggerStay = 1 << 9,
        OnTriggerExit = 1 << 10,
        OnCollisionEnter = 1 << 11,
        OnCollisionStay = 1 << 12,
        OnCollisionExit = 1 << 13,
        OnDisable = 1 << 14,
        OnApplicationQuit = 1 << 15,
        OnDestroy = 1 << 16,
        InDevelopmentBuild = 1 << 17,
        InProductionBuild = 1 << 18,
        OnInputAction = 1 << 19,
        VariableIntervalLoop = 1 << 20
    }
}