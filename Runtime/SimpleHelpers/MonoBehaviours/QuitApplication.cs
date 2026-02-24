using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Provides methods to quit the application with optional delay.
    /// In the editor, quits play mode; in builds, quits the application.
    /// </summary>
    public class QuitApplication : MonoBehaviour
    {
        [SerializeField] [Range(0, 60)] private float m_quitDelay = 7.5f;


        /// <summary>
        /// Quits the application after a configurable delay (in seconds).
        /// The delay is set via the m_quitDelay field (default: 7.5 seconds).
        /// Available as a context menu item in the Inspector.
        /// </summary>
        [ContextMenu(nameof(DelayedQuit))]
        public void DelayedQuit()
        {
            Invoke(nameof(Quit), m_quitDelay);
        }


        /// <summary>
        /// Immediately quits the application.
        /// In the editor, exits play mode; in builds, closes the application.
        /// Available as a context menu item in the Inspector.
        /// </summary>
        [ContextMenu(nameof(Quit))]
        public void Quit()
        {
            #if UNITY_EDITOR
            if (Application.isEditor)
            {
                // exit playmode
                UnityEditor.EditorApplication.isPlaying = false;

                return;
            }
            #endif

            Application.Quit();
        }
    }
}