using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SOSXR.SeaShark
{
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
            var currenSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currenSceneIndex + 1;

            if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                nextSceneIndex = 0;
            }

            SceneManager.LoadScene(nextSceneIndex);

            yield return new WaitForSeconds(m_switchingTime);
        }
    }
}