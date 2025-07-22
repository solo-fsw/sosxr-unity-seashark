using System.Collections;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(ILoadQueryURL))]
    public class LoadQueryURL : MonoBehaviour
    {
        [SerializeField] private bool m_startAutomatically = false;
        [SerializeField] protected ILoadQueryURL m_webViewPrefab;
        [SerializeField] private IHaveQueryData m_configData;

        private Coroutine _coroutine;


        private void OnValidate()
        {
            if (m_webViewPrefab == null)
            {
                m_webViewPrefab = GetComponent<ILoadQueryURL>();
            }

            if (m_configData == null)
            {
                Debug.LogWarning("ConfigData is null. Please assign a ConfigData scriptable object to this component");
            }
        }


        private void OnEnable()
        {
            if (m_startAutomatically)
            {
                LoadURL();
            }
        }


        [ContextMenu(nameof(LoadURL))]
        private void LoadURL()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(LoadUrlCR());
        }


        private IEnumerator LoadUrlCR()
        {
            var delay = 0.1f;

            while (!m_webViewPrefab.IsReady)
            {
                Debug.LogWarning("WebView is not ready yet. Waiting for " + delay + " seconds before checking again");

                yield return new WaitForSeconds(delay);
            }

            if (m_configData == null || string.IsNullOrEmpty(m_configData.QueryStringURL))
            {
                Debug.LogError("Cannot load URL since ConfigData is null or QueryStringURL is empty");

                yield break;
            }

            m_webViewPrefab.LoadURL(m_configData.QueryStringURL);

            Debug.Log("Loaded URL " + m_configData.QueryStringURL + " into our lovely WebView");
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}