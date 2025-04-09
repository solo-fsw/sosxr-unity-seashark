using UnityEngine;


namespace SOSXR.SeaShark.Attributes.InEditorSamples.Samples
{
    public class TagSelectorDemo : MonoBehaviour
    {
        [TagSelector] [SerializeField] private string m_tag;

        [DisableEditing] [SerializeField] private GameObject m_gameObject;


        private void Awake()
        {
            if (m_gameObject != null)
            {
                Debug.LogError("This should be null");
            }

            if (m_tag == "")
            {
                Debug.LogError("This should not be empty");
            }

            Debug.Log("Searching for gameobject with tag: " + m_tag);

            m_gameObject = GameObject.FindWithTag(m_tag);

            if (m_gameObject == null)
            {
                Debug.LogError("Could not find GameObject with tag: " + m_tag);
            }

            Debug.LogFormat(m_gameObject, "Found gameObject: " + m_gameObject.name);
        }
    }
}