using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     <b>What:</b> Concrete unmodified stimulus definition stored as a ScriptableObject.
    ///     <b>Why:</b> Provides a reusable baseline stimulus asset for perception and psychophysics protocols.
    ///     <b>How:</b> Implements <see cref="IStimulus"/> with baseline duration/intensity values and logs each presentation.
    /// </summary>
    [CreateAssetMenu(fileName = "New Stimulus", menuName = "SOSXR/Design Patterns/Stimulus")]
    public class BaseStimulus : ScriptableObject, IStimulus
    {
        [SerializeField] private string m_stimulusName = "Gabor Patch";
        [SerializeField] private float m_baseDuration = 0.2f;
        [SerializeField] private float m_baseIntensity = 0.8f;


        /// <summary>
        ///     Gets the base presentation duration before temporal decorators modify it.
        /// </summary>
        public float Duration => m_baseDuration;


        /// <summary>
        ///     Gets the base stimulus intensity before contrast/noise decorators modify visibility.
        /// </summary>
        public float Intensity => m_baseIntensity;


        /// <summary>
        ///     Gets the baseline stimulus description used in research logs.
        /// </summary>
        public string Description => m_stimulusName;


        /// <summary>
        ///     Presents the base stimulus and logs duration/intensity for perception experiment traceability.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        public void Present(Transform anchor)
        {
            if (anchor == null)
            {
                Debug.LogWarning($"{nameof(BaseStimulus)} presentation skipped because anchor is null.");
                return;
            }

            Debug.Log($"Presenting {Description} at {anchor.position} for {Duration:F3}s with intensity {Mathf.Clamp01(Intensity):F2}.");
        }
    }
}
