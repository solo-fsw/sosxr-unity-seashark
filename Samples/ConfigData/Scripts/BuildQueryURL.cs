using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     This demo is maybe a little specific to be in the ConfigData package, but it's a good example of how to use the
    ///     ConfigData in research tasks.
    /// </summary>
    public class BuildQueryURL : MonoBehaviour
    {
        [SerializeField] private DemoConfigData m_configData;

        private System.Action<object> _onPpnChanged; // Cached so Unsubscribe removes the same delegate


        private void OnEnable()
        {
            if (m_configData == null)
            {
                return;
            }

            _onPpnChanged ??= _ => BuildQuery();
            m_configData.Subscribe(nameof(m_configData.PPN), _onPpnChanged);
        }


        [ContextMenu(nameof(BuildQuery))]
        public void BuildQuery()
        {
            if (m_configData == null)
            {
                Debug.LogError("ConfigData not assigned.");

                return;
            }

            m_configData.QueryStringURL = m_configData.BuildQueryStringURL(m_configData.BaseURL, m_configData.TaskName, m_configData.VideoName, m_configData.PPN);
        }


        private void OnDisable()
        {
            if (m_configData != null && _onPpnChanged != null)
            {
                m_configData.Unsubscribe(nameof(m_configData.PPN), _onPpnChanged);
            }
        }
    }
}