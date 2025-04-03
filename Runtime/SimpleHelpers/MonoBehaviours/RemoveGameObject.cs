using UnityEngine;


namespace SOSXR.SeaShark
{
    public class RemoveGameObject : MonoBehaviour, ITargetReceiver
    {
        [SerializeField] private GameObject m_target;
        [SerializeField] [Range(0, 60)] private float m_delay = 1f;
        [SerializeField] private RemovalAction m_removalAction = RemovalAction.Destroy;


        public GameObject Target
        {
            get => m_target;
            set => m_target = value;
        }


        private void OnValidate()
        {
            if (m_target != null)
            {
                return;
            }

            m_target = gameObject;
        }


        [ContextMenu(nameof(RemoveAfterDelay))]
        public void RemoveAfterDelay()
        {
            if (m_target == null)
            {
                return;
            }

            if (m_removalAction == RemovalAction.Destroy)
            {
                if (Application.isPlaying)
                {
                    Debug.Log($"Scheduling destruction of {m_target.name} in {m_delay} seconds");
                    Destroy(m_target, m_delay);
                }
                else
                {
                    Debug.LogWarning($"Destroying immediately in Editor mode, delay of {m_delay} seconds will not be applied");
                    DestroyImmediate(m_target);
                }
            }
            else
            {
                if (Application.isPlaying)
                {
                    Debug.Log($"Scheduling deactivation of {m_target.name} in {m_delay} seconds");
                    Invoke(nameof(Remove), m_delay);
                }
                else
                {
                    Debug.LogWarning($"Deactivating immediately in Editor mode, delay of {m_delay} seconds will not be applied");
                    Remove();
                }
            }
        }


        private void Remove()
        {
            m_target.SetActive(false);
        }
    }
}