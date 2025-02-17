using System.Linq;
using UnityEngine;
#if SOSXR_EDITORTOOLS_INSTALLED
using SOSXR.EditorTools;
#endif


namespace SOSXR.SimpleHelpers
{
    public class FindGameObjectsWithTag : MonoBehaviour
    {
        #if SOSXR_EDITORTOOLS_INSTALLED
        [TagSelector]
        #endif
        [SerializeField] private string m_tagToSearchFor = "MainCamera";

        [SerializeField] private bool m_searchEveryFrameIfNull = true;
        [SerializeField] private bool m_checkForNull = true;

        private GameObject[] _foundGameObjects;


        public GameObject FirstOrOnlyGameObject
        {
            get
            {
                FindGameObjectsIfNull();

                return _foundGameObjects.Length > 0 ? _foundGameObjects[0] : null;
            }
        }

        public GameObject[] GameObjects
        {
            get
            {
                FindGameObjectsIfNull();

                return _foundGameObjects;
            }
        }


        public void FindGameObjectsIfNull()
        {
            if (_foundGameObjects != null && _foundGameObjects.Length != 0)
            {
                return;
            }

            FindGameObjects();
        }


        public void FindGameObjects()
        {
            _foundGameObjects = GameObject.FindGameObjectsWithTag(m_tagToSearchFor);
        }


        private bool InvalidatedCache()
        {
            if (_foundGameObjects == null || _foundGameObjects.Length == 0 || _foundGameObjects.All(go => go != null))
            {
                return false;
            }

            _foundGameObjects = null;

            Debug.Log("Invalidated cache because a GameObject with tag \"" + m_tagToSearchFor + "\" was destroyed.");

            return true;
        }


        private void Awake()
        {
            FindGameObjects();

            var targetReceiver = gameObject.GetComponent<ITargetReceiver>();

            if (targetReceiver != null)
            {
                targetReceiver.Target = FirstOrOnlyGameObject;
            }

            var targetsReceiver = gameObject.GetComponent<ITargetsReceiver>();

            if (targetsReceiver != null)
            {
                targetsReceiver.Targets = GameObjects;
            }
        }


        private void Update()
        {
            if (m_searchEveryFrameIfNull)
            {
                FindGameObjectsIfNull();

                if (_foundGameObjects == null || _foundGameObjects.Length == 0)
                {
                    Debug.LogWarning($"No GameObjects found with tag \"{m_tagToSearchFor}\".");
                }
            }

            if (m_checkForNull && InvalidatedCache())
            {
                FindGameObjects();
            }
        }
    }
}