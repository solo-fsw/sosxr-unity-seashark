using UnityEngine;


namespace SOSXR.SeaShark
{
    public class ToggleMultipleRenderers : MonoBehaviour
    {
        [Info("If you don't set the renderers, it will try to get all child renders")]
        [SerializeField] private Renderer[] m_renderers;


        private void Awake()
        {
            if (m_renderers == null || m_renderers.Length == 0)
            {
                m_renderers = GetComponentsInChildren<Renderer>();
                //this.Verbose("Will try to find renderers in child gameObjects");
            }
        }


        [Button]
        public void Toggle(bool setEnabled)
        {
            if (m_renderers == null || m_renderers.Length == 0)
            {
                Debug.LogWarning("No renderers found. Should this be the case?");

                enabled = false;

                return;
            }

            foreach (var rend in m_renderers)
            {
                if (setEnabled && !rend.gameObject.activeInHierarchy)
                {
                    Debug.Log("You will not see the result of this toggle at this moment, because this GameObject has not been enabled. This toggle will only affect the mesh renderer.");
                }

                rend.enabled = setEnabled;
            }
        }
    }
}
