using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Detects whether the app runs in the Editor, on an XR head-mounted display, or on a tablet on Awake, and fires the matching UnityEvent.
    /// </summary>
    public class DeviceChecker : MonoBehaviour
    {
        [SerializeField]
        private CurrentDevice m_platform;

        [SerializeField]
        private UnityEvent m_isHMD;

        [SerializeField]
        private UnityEvent m_isTablet;

        [SerializeField]
        private UnityEvent m_isEditor;

        [SerializeField]
        private UnityEvent m_isClone;

        private void Awake()
        {
            m_platform.Current = Device.None;

            CheckDevice();
        }

        private void CheckDevice()
        {
            if (Application.isEditor)
            {
                m_platform.Current = Device.Editor;
#if UNITY_EDITOR
                Debug.Log("We're in the editor");
                m_isEditor?.Invoke();
#endif
            }
            else if (IsXRActive())
            {
                Debug.Log("XR Device is active");
                m_platform.Current = Device.HMD;
                m_isHMD?.Invoke();
            }
            else
            {
                Debug.Log("XR Device is not active, we're assuming this is a tablet");
                m_platform.Current = Device.Tablet;
                m_isTablet?.Invoke();
            }

            m_platform.DeviceName = SystemInfo.deviceName;

            NotifyOfCurrentDevice();
        }

        private bool IsXRActive()
        {
            var displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displaySubsystems);

            return displaySubsystems.Exists(subsystem => subsystem.running);
        }

        /// <summary>
        ///     Sets the platform to HMD and fires the HMD event, regardless of the detected device.
        /// </summary>
        public void ActAsHMD()
        {
            m_platform.Current = Device.HMD;
            m_isHMD?.Invoke();

            Debug.Log("Acting as HMD, fired the HMD event");
        }

        /// <summary>
        ///     Sets the platform to Tablet and fires the Tablet event, regardless of the detected device.
        /// </summary>
        public void ActAsTablet()
        {
            m_platform.Current = Device.Tablet;
            m_isTablet?.Invoke();

            Debug.Log("Acting as Tablet, fired the Tablet event");
        }

        private void NotifyOfCurrentDevice()
        {
            Debug.Log("This is where you'd notify the rest of the app of the current device");
        }
    }
}

