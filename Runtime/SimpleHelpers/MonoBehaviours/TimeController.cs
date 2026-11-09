using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Controls the timescale using input actions.
    /// </summary>
    public class TimeController : MonoBehaviour
    {
        [SerializeField] [Range(0f, 2f)] private float m_timeScaleIncrease = 0.25f;
        [SerializeField] [Range(-2f, 0f)] private float m_timeScaleDecrease = -0.10f;

        [Header("Input Action References")] [SerializeField]
        private InputActionReference m_slowActionReference;

        [SerializeField] private InputActionReference m_pauseActionReference;
        [SerializeField] private InputActionReference m_normalActionReference;
        [SerializeField] private InputActionReference m_fastActionReference;

        [SerializeField] private bool m_onlyInEditor = true;

        private InputAction _slowAction;
        private InputAction _pauseAction;
        private InputAction _normalAction;
        private InputAction _fastAction;

        public UnityAction<float> OnTimeScaleChanged;


        private void OnEnable()
        {
            if (m_onlyInEditor && !Application.isEditor)
            {
                return;
            }

            SetActionsFromReference();
            EnableActions();
            RegisterActionHandlers();
        }


        private void SetActionsFromReference()
        {
            _slowAction = m_slowActionReference != null ? m_slowActionReference.action : null;
            _pauseAction = m_pauseActionReference != null ? m_pauseActionReference.action : null;
            _normalAction = m_normalActionReference != null ? m_normalActionReference.action : null;
            _fastAction = m_fastActionReference != null ? m_fastActionReference.action : null;
        }


        private void EnableActions()
        {
            _slowAction?.Enable();
            _pauseAction?.Enable();
            _normalAction?.Enable();
            _fastAction?.Enable();
        }


        private void RegisterActionHandlers()
        {
            if (_slowAction != null)
            {
                _slowAction.performed += SlowTime;
            }

            if (_pauseAction != null)
            {
                _pauseAction.performed += PauseTime;
            }

            if (_normalAction != null)
            {
                _normalAction.performed += NormalTime;
            }

            if (_fastAction != null)
            {
                _fastAction.performed += SpeedUpTime;
            }
        }


        private void SlowTime(CallbackContext context)
        {
            ChangeTimeScale(m_timeScaleDecrease);
        }


        private void PauseTime(CallbackContext context)
        {
            SetTimeScale(0f);
        }


        private void NormalTime(CallbackContext context)
        {
            SetTimeScale(1f);
        }


        private void SpeedUpTime(CallbackContext context)
        {
            ChangeTimeScale(m_timeScaleIncrease);
        }


        private void ChangeTimeScale(float delta)
        {
            var newTimeScale = Mathf.Clamp(Time.timeScale + delta, 0, 100f);

            SetTimeScale(newTimeScale);
        }


        private void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;

            OnTimeScaleChanged?.Invoke(Time.timeScale);
        }


        private void OnDisable()
        {
            UnregisterActionHandlers();
            DisableActions();
        }


        private void UnregisterActionHandlers()
        {
            if (_slowAction != null)
            {
                _slowAction.performed -= SlowTime;
            }

            if (_pauseAction != null)
            {
                _pauseAction.performed -= PauseTime;
            }

            if (_normalAction != null)
            {
                _normalAction.performed -= NormalTime;
            }

            if (_fastAction != null)
            {
                _fastAction.performed -= SpeedUpTime;
            }
        }


        private void DisableActions()
        {
            _slowAction?.Disable();
            _pauseAction?.Disable();
            _normalAction?.Disable();
            _fastAction?.Disable();
        }
    }
}