using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;


namespace SOSXR.AdditionalUnityEvents
{
    public class UnityEventOnInputAction : AdditionalUnityEvent
    {
        [SerializeField] private InputActionProperty m_inputAction;


        private void OnEnable()
        {
            m_inputAction.action.Enable();
            m_inputAction.action.performed += FireEvent;
        }


        private void FireEvent(CallbackContext callbackContext)
        {
            FireEvent();
        }


        private void OnDisable()
        {
            m_inputAction.action.performed -= FireEvent;
            m_inputAction.action.Disable();
        }
    }
}