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
            _sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        }


        [ContextMenu(nameof(AdjustPitchToTimeScale))]
        public void AdjustPitchToTimeScale()
        {
            AdjustPitchToTimeScale(Time.timeScale);
        }


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