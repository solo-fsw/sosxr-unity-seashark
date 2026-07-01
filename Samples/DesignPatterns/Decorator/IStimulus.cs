using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     The Decorator pattern composes stimulus presentation behavior at runtime for perception and attention experiments.
    ///     This enables layered manipulations such as masking, timing jitter, and contrast/noise changes without altering the base stimulus implementation.
    /// </summary>
    public interface IStimulus
    {
        /// <summary>
        ///     Gets the total presentation duration in seconds after all psychophysics modifiers are applied.
        /// </summary>
        float Duration { get; }


        /// <summary>
        ///     Gets the effective stimulus intensity (normalized 0-1) after perceptual visibility modifiers are applied.
        /// </summary>
        float Intensity { get; }


        /// <summary>
        ///     Gets a human-readable summary of the current stimulus chain for perception study logging.
        /// </summary>
        string Description { get; }


        /// <summary>
        ///     Presents the stimulus at the given anchor, supporting visual search, masking, and threshold procedures.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        void Present(Transform anchor);
    }
}
