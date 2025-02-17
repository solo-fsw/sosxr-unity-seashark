using System.Collections;
using UnityEngine;


/// <summary>
///     Periodically unloads unused assets to free memory.
/// </summary>
public class UnloadUnusedAssets : MonoBehaviour
{
    [SerializeField] [Range(0, 300)] private int m_autoUnloadInterval = 0;
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