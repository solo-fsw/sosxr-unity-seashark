using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class QualityLevel : MonoBehaviour
    {
        [SerializeField] private bool m_debugSimulateTabletInEditor = true;
        [SerializeField] private string[] m_availableQualityLevels;
        [SerializeField] private string m_currentQualityLevel;
        [SerializeField] private CurrentDevice m_platform;
        [SerializeField] private string m_defaultQualityLevel = "Quest 2";
        public bool IsValid { get; private set; }


        public void OnValidate()
        {
            IsValid = true;
        }


        private void Awake()
        {
            m_availableQualityLevels = QualitySettings.names;
        }


        private void OnEnable()
        {
            if (m_platform != null)
            {
                m_platform.Changed -= OnDeviceChanged;
                m_platform.Changed += OnDeviceChanged;
            }
        }


        private void Start()
        {
            SetDefaultQualityLevel();

            // The device may already be known (DeviceChecker sets it in Awake).
            if (m_platform != null && m_platform.Current != Device.None)
            {
                ChangeQualityLevel();
            }
        }


        private void OnDeviceChanged(CurrentDevice currentDevice)
        {
            ChangeQualityLevel();
        }


        private void SetDefaultQualityLevel()
        {
            var index = Array.IndexOf(m_availableQualityLevels, m_defaultQualityLevel);

            if (index == -1)
            {
                Debug.LogWarning($"Quality level '{m_defaultQualityLevel}' not found in QualitySettings; keeping current level.");

                return;
            }

            QualitySettings.SetQualityLevel(index);
            m_currentQualityLevel = "DEFAULT: " + m_defaultQualityLevel;
        }


        private void ChangeQualityLevel()
        {
            if (m_platform == null || string.IsNullOrEmpty(m_platform.DeviceName) || m_availableQualityLevels == null)
            {
                return;
            }

            if (m_debugSimulateTabletInEditor && m_platform.DeviceName == "MBP")
            {
                SetQualityLevel("Galaxy Tab A7", "We're debugging in the editor, and assuming we're simulating the tablet: ");
            }
            else if (IsSMT500Device(m_platform.DeviceName))
            {
                SetQualityLevel("SM-T500", "We set the Quality Settings to SM-T500");
            }
            else if (((IList) m_availableQualityLevels).Contains(m_platform.DeviceName))
            {
                SetQualityLevel(m_platform.DeviceName, "We set the Quality Settings to " + m_platform.DeviceName);
            }
        }


        private bool IsSMT500Device(string deviceName)
        {
            return Regex.IsMatch(deviceName, @"^SM-T500 \d+$");
        }


        private void SetQualityLevel(string qualityLevel, string successMessage)
        {
            var index = Array.IndexOf(m_availableQualityLevels, qualityLevel);

            if (index == -1)
            {
                return;
            }

            QualitySettings.SetQualityLevel(index);

            m_currentQualityLevel = m_availableQualityLevels[index];
            Debug.Log(successMessage);
        }


        private void OnDisable()
        {
            if (m_platform != null)
            {
                m_platform.Changed -= OnDeviceChanged;
            }
        }
    }
}