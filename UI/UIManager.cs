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
        }

        private void OnEnable()
        {
            // UIDocument rebuilds its visual tree when (re)enabled, so fetch the root every time.
            _root = _document != null ? _document.rootVisualElement : null;

            if (_root == null)
            {
                Debug.LogWarning("UIDocument has no root visual element", this);

                return;
            }

            foreach (var map in m_buttonMap)
            {
                map.Button = _root.Q<Button>(map.UIName);

                if (map.Button == null)
                {
                    Debug.LogWarning($"Button '{map.UIName}' not found in UIDocument", this);

                    continue;
                }

                var buttonMap = map;
                map.ClickCallback = _ => buttonMap.OnClicked?.Invoke();
                map.Button.RegisterCallback<ClickEvent>(OnAnyButtonClicked);
                map.Button.RegisterCallback(map.ClickCallback);
            }

            if (m_submit.action != null)
            {
                m_submit.action.performed += AlternativeClick;
                m_submit.action.Enable();
            }

            if (m_cancel.action != null)
            {
                m_cancel.action.performed += AlternativeCancel;
                m_cancel.action.Enable();
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
            var focused = _root?.panel?.focusController?.focusedElement;

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
            var focused = _root?.panel?.focusController?.focusedElement;
            focused?.Blur();
        }

        private void OnDisable()
        {
            foreach (var map in m_buttonMap)
            {
                if (map.Button == null)
                {
                    continue;
                }

                map.Button.UnregisterCallback<ClickEvent>(OnAnyButtonClicked);

                if (map.ClickCallback != null)
                {
                    map.Button.UnregisterCallback(map.ClickCallback);
                    map.ClickCallback = null;
                }
            }

            if (m_submit.action != null)
            {
                m_submit.action.performed -= AlternativeClick;

                if (m_submit.reference == null)
                {
                    m_submit.action.Disable(); // Only disable actions we own, not shared asset references.
                }
            }

            if (m_cancel.action != null)
            {
                m_cancel.action.performed -= AlternativeCancel;

                if (m_cancel.reference == null)
                {
                    m_cancel.action.Disable();
                }
            }
        }
    }

    [Serializable]
    public class ButtonMap
    {
        [NonSerialized] public Button Button;
        [NonSerialized] public EventCallback<ClickEvent> ClickCallback;
        public string UIName; // Use naming from UI Builder, but without #
        public UnityEvent OnClicked;
    }
}
