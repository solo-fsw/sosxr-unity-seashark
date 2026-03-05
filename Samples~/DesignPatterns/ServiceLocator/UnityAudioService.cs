using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     <b>What:</b> Concrete Unity implementation of <see cref="IAudioService"/> backed by <see cref="AudioSource"/> components.
    ///     <b>Why:</b> Encapsulates scene audio playback details so experiment systems depend only on <see cref="IAudioService"/>.
    ///     <b>How:</b> Registers itself with <see cref="ServiceLocator"/> in <see cref="Awake"/>, routes calls to dedicated music/SFX sources, and unregisters in <see cref="OnDestroy"/>.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Unity Audio Service")]
    public class UnityAudioService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource m_musicSource;
        [SerializeField] private AudioSource m_sfxSource;


        private void Awake()
        {
            if (m_musicSource == null)
            {
                m_musicSource = gameObject.AddComponent<AudioSource>();
                m_musicSource.playOnAwake = false;
                m_musicSource.loop = true;
            }

            if (m_sfxSource == null)
            {
                m_sfxSource = gameObject.AddComponent<AudioSource>();
                m_sfxSource.playOnAwake = false;
                m_sfxSource.loop = false;
            }

            ServiceLocator.Register<IAudioService>(this);
        }


        private void OnDestroy()
        {
            if (ServiceLocator.TryGet(out IAudioService registeredService) && ReferenceEquals(registeredService, this))
            {
                ServiceLocator.Unregister<IAudioService>();
            }
        }


        /// <summary>
        ///     Plays a one-shot sound effect through the SFX source.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        /// <param name="volume">The playback volume multiplier.</param>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
            {
                return;
            }

            m_sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }


        /// <summary>
        ///     Starts music playback through the music source.
        /// </summary>
        /// <param name="clip">The music clip to play.</param>
        /// <param name="loop">Whether the clip should loop.</param>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null)
            {
                return;
            }

            m_musicSource.clip = clip;
            m_musicSource.loop = loop;
            m_musicSource.Play();
        }


        /// <summary>
        ///     Stops currently playing music.
        /// </summary>
        public void StopMusic()
        {
            m_musicSource.Stop();
            m_musicSource.clip = null;
        }


        /// <summary>
        ///     Sets music volume on the music source.
        /// </summary>
        /// <param name="volume">The target music volume in the range 0 to 1.</param>
        public void SetMusicVolume(float volume)
        {
            m_musicSource.volume = Mathf.Clamp01(volume);
        }


        /// <summary>
        ///     Sets SFX volume on the SFX source.
        /// </summary>
        /// <param name="volume">The target SFX volume in the range 0 to 1.</param>
        public void SetSFXVolume(float volume)
        {
            m_sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}
