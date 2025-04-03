using UnityEngine;


namespace SOSXR.SeaShark
{
    public class DestroyInBuild : MonoBehaviour
    {
        [SerializeField] private bool m_destroyInBuild = true;


        private void Awake()
        {
            if (!NeedsDestroying())
            {
                return;
            }

            Destroy(gameObject);
        }


        public bool NeedsDestroying()
        {
            return m_destroyInBuild && !Application.isEditor;
        }
    }
}