using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private InputActionProperty m_submit;
        [SerializeField] private InputActionProperty m_cancel;
        [SerializeField] private AudioClip m_clip;
        [SerializeField] private List<ButtonMap> m_buttonMap;

        private UIDocument _document;
        private VisualElement _root;


        private void Awake()
        {
            if (_document == null)
            {
                _document = GetComponent<UIDocument>();
            }
            if (_root == null)
            {
                _root = _document?.rootVisualElement;
            }
        }

        private void OnEnable()
        {
            foreach (var map in m_buttonMap)
            {
                map.Button = _root.Q<Button>(map.UIName);
                map.Button.RegisterCallback<ClickEvent>(OnAnyButtonClicked);
                map.Button.RegisterCallback<ClickEvent>(_ => map.OnClicked?.Invoke());
            }

            if (m_submit != null)
            {
                m_submit.action.performed += AlternativeClick;
            }
            if (m_cancel != null)
            {
                m_cancel.action.performed += AlternativeCancel;
            }
        }

        public void OnAnyButtonClicked(ClickEvent evnt)
        {
            if (m_clip != null)
            {
                AudioSource.PlayClipAtPoint(m_clip, new Vector3(0, 0, 0));
            }
        }

        // Use this for basically anything other than mouse
        private void AlternativeClick(CallbackContext thing)
        {
            var focused = _root.panel.focusController.focusedElement;

            if (focused is Button button)
            {
                using var evt = ClickEvent.GetPooled();
                evt.target = button;
                button.SendEvent(evt); // This in effect sends out a ClickEvent (thus using the earlier callbacks from RegisterCallback)
            }
        }

        // So that using esc key will de-highlight the currently selected thing
        private void AlternativeCancel(CallbackContext thing)
        {
            var focused = _root.panel.focusController.focusedElement;
            focused?.Blur();
        }

        private void OnDisable()
        {
            foreach (var map in m_buttonMap)
            {
                map.Button.UnregisterCallback<ClickEvent>(OnAnyButtonClicked);
                map.Button.UnregisterCallback<ClickEvent>(_ => map.OnClicked?.Invoke());
            }

            if (m_submit != null)
            {
                m_submit.action.performed -= AlternativeClick;
            }

            if (m_cancel != null)
            {
                m_cancel.action.performed -= AlternativeCancel;
            }
        }
    }

    [Serializable]
    public class ButtonMap
    {
        [NonSerialized] public Button Button;
        public string UIName; // Use naming from UI Builder, but without #
        public UnityEvent OnClicked;
    }
}
