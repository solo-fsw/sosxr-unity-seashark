using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Service interface for audio. Concrete implementations can be UnityAudioService (hardware audio), NullAudioService (silent mode for testing), or custom lab audio service.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        ///     Plays a one-shot sound effect.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        /// <param name="volume">The playback volume multiplier.</param>
        void PlaySFX(AudioClip clip, float volume = 1f);


        /// <summary>
        ///     Starts music playback.
        /// </summary>
        /// <param name="clip">The music clip to play.</param>
        /// <param name="loop">Whether the clip should loop.</param>
        void PlayMusic(AudioClip clip, bool loop = true);


        /// <summary>
        ///     Stops currently playing music.
        /// </summary>
        void StopMusic();


        /// <summary>
        ///     Sets global music volume.
        /// </summary>
        /// <param name="volume">The target music volume in the range 0 to 1.</param>
        void SetMusicVolume(float volume);


        /// <summary>
        ///     Sets global SFX volume.
        /// </summary>
        /// <param name="volume">The target SFX volume in the range 0 to 1.</param>
        void SetSFXVolume(float volume);
    }
}
