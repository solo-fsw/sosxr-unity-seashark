using UnityEngine;


namespace SOSXR.SimpleHelpers
{
    /// <summary>
    ///     Make the audio speed match the time speed.
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