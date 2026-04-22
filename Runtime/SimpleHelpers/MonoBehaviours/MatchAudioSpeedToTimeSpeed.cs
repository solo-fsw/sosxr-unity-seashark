using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Somehow Unity doesn't adjust the pitch of audio sources when changing the time scale.
    ///     This will do that for you.
    ///     It finds all audio sources in the scene and adjusts their pitch to match the time scale.
    ///     You just need to hook up the AdjustPitchToTimeScale method to some time scale change event.
    /// </summary>
    public class MatchAudioSpeedToTimeSpeed : MonoBehaviour
    {
        private AudioSource[] _sources;


        private void Awake()
        {
            _sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude);
        }


        /// <summary>
        /// Adjusts the pitch of all audio sources in the scene to match the current Time.timeScale.
        /// Available as a context menu item in the Inspector.
        /// </summary>
        [ContextMenu(nameof(AdjustPitchToTimeScale))]
        public void AdjustPitchToTimeScale()
        {
            AdjustPitchToTimeScale(Time.timeScale);
        }


        /// <summary>
        /// Adjusts the pitch of all audio sources in the scene to the specified time scale value.
        /// </summary>
        /// <param name="timeScale">The time scale value to apply as pitch to all audio sources.</param>
        public void AdjustPitchToTimeScale(float timeScale)
        {
            if (_sources == null)
            {
                return;
            }

            foreach (var audioSource in _sources)
            {
                audioSource.pitch = timeScale;
            }
        }
    }
}