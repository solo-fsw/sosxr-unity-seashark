using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Reads the assigned ConfigData as JSON every m_readJsonInterval seconds (minimum 1) and passes the text to a UnityEvent.
    /// </summary>
    public class ConfigDataJsonToUnityEvent : MonoBehaviour
    {
        [SerializeField] private BaseConfigData m_configData;

        [SerializeField] private UnityEvent<string> m_displayCompleteJSON;

        [SerializeField] [Range(1, 30)] private int m_readJsonInterval = 1;


        private void Start()
        {
            if (m_configData == null)
            {
                Debug.LogError("ConfigData not assigned.");

                return;
            }

            // InvokeRepeating throws on a repeat rate <= 0, so guard against old serialized 0 values.
            InvokeRepeating(nameof(ReadJson), 0, Mathf.Max(1, m_readJsonInterval));
        }


        private void ReadJson()
        {
            m_displayCompleteJSON?.Invoke(m_configData.ReadJson());
        }
    }
}