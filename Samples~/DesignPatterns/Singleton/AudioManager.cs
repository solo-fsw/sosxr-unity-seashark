using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates a scene-scoped Singleton audio manager for auditory stimulus delivery and feedback tones.
    ///     Use <see cref="Singleton{T}"/> when the manager should reset each time a scene changes and should not
    ///     carry runtime state forward automatically. Use <see cref="PersistentSingleton{T}"/> instead for managers
    ///     that must persist across scenes, such as a global session controller.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Audio Manager (Singleton)")]
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        ///     Source used for looping or long-running instruction audio playback.
        /// </summary>
        [SerializeField] private AudioSource m_musicSource;

        /// <summary>
        ///     Source used for one-shot auditory stimuli and feedback tones.
        /// </summary>
        [SerializeField] private AudioSource m_sfxSource;


        /// <summary>
        ///     Starts playback of the supplied instruction or background audio clip.
        /// </summary>
        /// <param name="clip">Instruction or ambient clip to assign and play.</param>
        public void PlayMusic(AudioClip clip)
        {
            if (m_musicSource == null)
            {
                Debug.LogWarning($"{nameof(AudioManager)}: Music AudioSource is not assigned.", this);
                return;
            }

            m_musicSource.clip = clip;
            m_musicSource.Play();
        }


        /// <summary>
        ///     Stops the currently playing instruction or background audio clip.
        /// </summary>
        public void StopMusic()
        {
            if (m_musicSource == null)
            {
                return;
            }

            m_musicSource.Stop();
        }


        /// <summary>
        ///     Plays a one-shot auditory stimulus or feedback tone clip at the requested volume.
        /// </summary>
        /// <param name="clip">Auditory stimulus or feedback tone clip to play once.</param>
        /// <param name="volume">Playback volume multiplier from 0 to 1.</param>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (m_sfxSource == null)
            {
                Debug.LogWarning($"{nameof(AudioManager)}: SFX AudioSource is not assigned.", this);
                return;
            }

            m_sfxSource.PlayOneShot(clip, volume);
        }


        /// <summary>
        ///     Sets the instruction audio source volume.
        /// </summary>
        /// <param name="volume">Target instruction audio volume from 0 to 1.</param>
        public void SetMusicVolume(float volume)
        {
            if (m_musicSource == null)
            {
                return;
            }

            m_musicSource.volume = volume;
        }
    }
}
