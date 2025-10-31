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
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            // Do nothing
            #else
            HandleComponents();
            #endif
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