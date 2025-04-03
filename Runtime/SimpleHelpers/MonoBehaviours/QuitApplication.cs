using UnityEngine;


namespace SOSXR.SeaShark
{
    public class QuitApplication : MonoBehaviour
    {
        [SerializeField] [Range(0, 60)] private float m_quitDelay = 7.5f;


        [ContextMenu(nameof(DelayedQuit))]
        public void DelayedQuit()
        {
            Invoke(nameof(Quit), m_quitDelay);
        }


        [ContextMenu(nameof(Quit))]
        public void Quit()
        {
            Application.Quit();
        }
    }
}