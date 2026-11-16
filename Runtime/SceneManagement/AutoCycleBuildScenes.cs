using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Loads the next scene in Build Settings every m_switchingTime seconds, wrapping back to the first scene after the last.
    ///     Calls DontDestroyOnLoad on its GameObject so the cycle keeps running across scene loads.
    /// </summary>
    public class AutoCycleBuildScenes : MonoBehaviour
    {
        [SerializeField] [Range(1, 180)] private int m_switchingTime = 10;


        private void Awake()
        {
            DontDestroyOnLoad(this);
        }


        private void Start()
        {
            StartCoroutine(SwitchScenesCR());
        }


        private IEnumerator SwitchScenesCR()
        {
            var wait = new WaitForSeconds(m_switchingTime);

            while (true)
            {
                yield return wait;

                var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
                {
                    nextSceneIndex = 0;
                }

                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}