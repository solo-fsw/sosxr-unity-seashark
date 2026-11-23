using System.Collections;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Loads the query URL from an <see cref="IHaveQueryData" /> into an <see cref="ILoadQueryURL" />.
    ///     Interfaces cannot be serialized by Unity, so both are stored as <see cref="Object" /> references and cast at runtime.
    /// </summary>
    public class LoadQueryURL : MonoBehaviour
    {
        [SerializeField] private bool m_startAutomatically;
        [Tooltip("Component implementing ILoadQueryURL. Defaults to one on this GameObject.")]
        [SerializeField] private Object m_webView;
        [Tooltip("Object (e.g. ScriptableObject) implementing IHaveQueryData.")]
        [SerializeField] private Object m_configData;
        [Tooltip("How long to wait for the WebView to report IsReady before giving up. 0 waits forever.")]
        [SerializeField] [Min(0f)] private float m_readyTimeout = 30f;

        private Coroutine _coroutine;

        protected ILoadQueryURL WebView => m_webView as ILoadQueryURL;
        private IHaveQueryData ConfigData => m_configData as IHaveQueryData;


        private void OnValidate()
        {
            ResolveWebView();

            if (m_webView != null && WebView == null)
            {
                Debug.LogWarning($"{m_webView.name} does not implement {nameof(ILoadQueryURL)}", this);
                m_webView = null;
            }

            if (m_configData != null && ConfigData == null)
            {
                Debug.LogWarning($"{m_configData.name} does not implement {nameof(IHaveQueryData)}", this);
                m_configData = null;
            }
        }


        private void Awake()
        {
            ResolveWebView();
        }


        private void ResolveWebView()
        {
            if (m_webView == null)
            {
                m_webView = GetComponent<ILoadQueryURL>() as Object;
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

            if (WebView == null)
            {
                Debug.LogError($"No {nameof(ILoadQueryURL)} assigned", this);

                yield break;
            }

            var waited = 0f;
            var wait = new WaitForSecondsRealtime(delay);

            if (!WebView.IsReady)
            {
                Debug.Log("WebView is not ready yet. Waiting for it before loading the URL.", this);
            }

            while (!WebView.IsReady)
            {
                if (m_readyTimeout > 0f && waited >= m_readyTimeout)
                {
                    Debug.LogError($"WebView was not ready after {m_readyTimeout} seconds. The URL is not loaded.", this);
                    _coroutine = null;

                    yield break;
                }

                yield return wait;
                waited += delay;
            }

            if (ConfigData == null || string.IsNullOrEmpty(ConfigData.QueryStringURL))
            {
                Debug.LogError("Cannot load URL since ConfigData is null or QueryStringURL is empty");

                yield break;
            }

            WebView.LoadURL(ConfigData.QueryStringURL);

            Debug.Log("Loaded URL " + ConfigData.QueryStringURL + " into our lovely WebView");
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}