using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Runs UnityEvent on Start, depending on which Device we're currently on.
    ///     One Event is called if we are on TargetDevice, the other if we're not.
    /// </summary>
    public class DeviceDependentUnityEvents : MonoBehaviour
    {
        [Header("Device")]
        [SerializeField] private CurrentDevice m_currentDevice;
        [SerializeField] private Device m_targetDevice;

        [Header("Timing: READ TOOLTIPS!")]
        [Tooltip("This can work if it is not in the same Scene as where the DeviceChecker is, since that one SETS the device on Start")]
        [SerializeField] private bool m_runOnStart = true;
        [Tooltip("Since the Device is set in the DeviceChecker on Start, we might miss the event if we're not subscribed to it.")]
        [SerializeField] private bool m_subscribeToEvents = true;

        [Header("Events")]
        [SerializeField] private UnityEvent m_eventIfOnTargetDevice;
        [SerializeField] private UnityEvent m_eventIfNotOnTargetDevice;


        private void OnValidate()
        {
            if (m_runOnStart && m_subscribeToEvents)
            {
                Debug.LogWarning("Are you sure we want to run this on Start AND subscribe to events?");
            }

            if (!m_runOnStart && !m_subscribeToEvents)
            {
                Debug.LogWarning("Since neither bool is set, the UnityEvents will not be fired. Is this intentional?");
            }
        }


        private void Start()
        {
            if (m_runOnStart)
            {
                SendUnityEvents();
            }
        }


        private void OnEnable()
        {
            if (m_subscribeToEvents)
            {
                Debug.LogWarning("Subscribe to Events from here.");
            }
        }


        private void SendUnityEvents()
        {
            if (m_currentDevice.Current == m_targetDevice)
            {
                Debug.Log("CurrentDevice is " + m_targetDevice + " so running event.");
                m_eventIfOnTargetDevice?.Invoke();
            }
            else if (m_currentDevice.Current != m_targetDevice)
            {
                Debug.Log("CurrentDevice is not " + m_targetDevice + " so running event.");
                m_eventIfNotOnTargetDevice?.Invoke();
            }
            else if (m_currentDevice.Current == Device.None)
            {
                Debug.LogError("CurrentDevice has not yet been set.");
            }
            else
            {
                Debug.LogError("Something went wrong.");
            }
        }


        private void OnDisable()
        {
            if (m_subscribeToEvents)
            {
                Debug.LogWarning("UNSubscribe to Events from here.");
            }
        }
    }
}