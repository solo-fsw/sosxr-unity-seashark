using System.Collections;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Periodically unloads unused assets to free memory.
    /// If m_autoUnloadInterval is greater than 0, automatically unloads assets at the specified interval (in seconds).
    /// </summary>
    public class UnloadUnusedAssets : MonoBehaviour
    {
        [SerializeField] [Range(0, 300)] private int m_autoUnloadInterval;
        private Coroutine _unloadCoroutine;


        private void Start()
        {
            if (m_autoUnloadInterval > 0)
            {
                StartUnloadLoop();
            }
            else
            {
                Debug.Log("Auto-unload disabled due to interval <= 0.");
            }
        }


        private void StartUnloadLoop()
        {
            if (_unloadCoroutine != null)
            {
                StopCoroutine(_unloadCoroutine);
            }

            _unloadCoroutine = StartCoroutine(UnloadLoop());
        }


        private IEnumerator UnloadLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_autoUnloadInterval);
                UnloadNow();
            }
        }


        /// <summary>
        /// Immediately unloads all unused assets from memory.
        /// Available as a context menu item in the Inspector.
        /// </summary>
        [ContextMenu(nameof(UnloadNow))]
        public void UnloadNow()
        {
            Resources.UnloadUnusedAssets();
            Debug.Log("Unused assets unloaded.");
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}