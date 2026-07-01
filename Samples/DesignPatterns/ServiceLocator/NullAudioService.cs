using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Null Object pattern combined with Service Locator. Use this when audio should be silent (silent testing, classroom demonstrations, or hardware-free development).
    ///     Register it in place of UnityAudioService — no conditional checks needed anywhere.
    /// </summary>
    public class NullAudioService : IAudioService
    {
        /// <summary>
        ///     Performs no operation for SFX playback.
        /// </summary>
        /// <param name="clip">Ignored clip parameter.</param>
        /// <param name="volume">Ignored volume parameter.</param>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
        }


        /// <summary>
        ///     Performs no operation for music playback.
        /// </summary>
        /// <param name="clip">Ignored clip parameter.</param>
        /// <param name="loop">Ignored loop parameter.</param>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
        }


        /// <summary>
        ///     Performs no operation for stopping music.
        /// </summary>
        public void StopMusic()
        {
        }


        /// <summary>
        ///     Performs no operation for music volume changes.
        /// </summary>
        /// <param name="volume">Ignored volume parameter.</param>
        public void SetMusicVolume(float volume)
        {
        }


        /// <summary>
        ///     Performs no operation for SFX volume changes.
        /// </summary>
        /// <param name="volume">Ignored volume parameter.</param>
        public void SetSFXVolume(float volume)
        {
        }
    }
}
