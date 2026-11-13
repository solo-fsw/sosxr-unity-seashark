using UnityEngine;


namespace SOSXR.SeaShark
{
    public class InProductionBuild : MonoBehaviour
    {
        [SerializeField] private DoInProductionBuild m_doInProductionBuild;
        private GameObject _objectToDestroy;


        private void OnValidate()
        {
            if (_objectToDestroy == null)
            {
                _objectToDestroy = gameObject;
            }
        }


        private void Awake()
        {
            // Debug.isDebugBuild is true in the Editor and in Development builds: do nothing there.
            // Runtime check replaces the DEVELOPMENT_BUILD symbol, which is deprecated in Unity 6.6 (UAC0009).
            if (Debug.isDebugBuild)
            {
                return;
            }

            HandleComponents();
        }


        private void HandleComponents()
        {
            if (m_doInProductionBuild == DoInProductionBuild.Disable)
            {
                enabled = false;
            }
            else if (m_doInProductionBuild == DoInProductionBuild.Destroy)
            {
                Destroy(_objectToDestroy);
            }
        }


        private enum DoInProductionBuild
        {
            Disable,
            Destroy
        }
    }
}