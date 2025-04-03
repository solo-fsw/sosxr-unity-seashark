using UnityEngine;


namespace SOSXR.SeaShark.Attributes.InEditorSamples.ConfigData.Scripts
{
    /// <summary>
    ///     This demo is maybe a little specific to be in the ConfigData package, but it's a good example of how to use the
    ///     ConfigData in research tasks.
    /// </summary>
    public class BuildQueryURL : MonoBehaviour
    {
        [SerializeField] private DemoConfigData m_configData;

        // Testes
        // Reverse


        private void OnEnable()
        {
            m_configData.Subscribe(nameof(m_configData.PPN), _ => BuildQuery());
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
            m_configData.Unsubscribe(nameof(m_configData.PPN), _ => BuildQuery());
        }
    }
}