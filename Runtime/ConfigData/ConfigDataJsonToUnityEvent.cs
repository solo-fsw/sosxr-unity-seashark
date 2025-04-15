using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SeaShark
{
    public class ConfigDataJsonToUnityEvent : MonoBehaviour
    {
        [SerializeField] private BaseConfigData m_configData;

        [SerializeField] private UnityEvent<string> m_displayCompleteJSON;

        [SerializeField] [Range(0, 30)] private int m_readJsonInterval = 1;


        private void Start()
        {
            if (m_configData == null)
            {
                Debug.LogError("ConfigData not assigned.");

                return;
            }

            InvokeRepeating(nameof(ReadJson), 0, m_readJsonInterval);
        }


        private void ReadJson()
        {
            m_displayCompleteJSON?.Invoke(m_configData.ReadJson());
        }
    }
}