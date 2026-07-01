using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Adds backward masking to probe conscious perception thresholds in psychophysics tasks.
    /// </summary>
    public class BackwardMaskDecorator : StimulusDecorator
    {
        private readonly float _maskDuration;


        /// <summary>
        ///     Initializes a new backward masking decorator.
        /// </summary>
        /// <param name="wrappedStimulus">The stimulus to augment.</param>
        /// <param name="maskDuration">The mask presentation duration in seconds.</param>
        public BackwardMaskDecorator(IStimulus wrappedStimulus, float maskDuration) : base(wrappedStimulus)
        {
            _maskDuration = Mathf.Max(0f, maskDuration);
        }


        /// <summary>
        ///     Gets duration including the post-stimulus mask interval.
        /// </summary>
        public override float Duration => _wrapped.Duration + _maskDuration;


        /// <summary>
        ///     Gets description including backward masking details.
        /// </summary>
        public override string Description => $"{_wrapped.Description} + Backward Mask ({_maskDuration:F3}s)";


        /// <summary>
        ///     Presents the wrapped stimulus then logs immediate mask presentation.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        public override void Present(Transform anchor)
        {
            _wrapped.Present(anchor);
            Debug.Log($"Mask presented for {_maskDuration:F3}s at {anchor.position} to reduce post-target visibility.");
        }
    }


    /// <summary>
    ///     Applies visual noise to degrade stimulus clarity for discrimination and attention studies.
    /// </summary>
    public class NoiseOverlayDecorator : StimulusDecorator
    {
        private readonly float _noiseFactor;


        /// <summary>
        ///     Initializes a new noise overlay decorator.
        /// </summary>
        /// <param name="wrappedStimulus">The stimulus to augment.</param>
        /// <param name="noiseFactor">The multiplicative visibility factor (0-1) after noise is applied.</param>
        public NoiseOverlayDecorator(IStimulus wrappedStimulus, float noiseFactor) : base(wrappedStimulus)
        {
            _noiseFactor = Mathf.Clamp01(noiseFactor);
        }


        /// <summary>
        ///     Gets intensity after applying the noise visibility factor.
        /// </summary>
        public override float Intensity => Mathf.Clamp01(_wrapped.Intensity * _noiseFactor);


        /// <summary>
        ///     Gets description including noise overlay details.
        /// </summary>
        public override string Description => $"{_wrapped.Description} + Noise Overlay (x{_noiseFactor:F2})";


        /// <summary>
        ///     Presents the wrapped stimulus and logs Gaussian-style visual noise application.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        public override void Present(Transform anchor)
        {
            _wrapped.Present(anchor);
            Debug.Log($"Gaussian noise overlay applied at {anchor.position}; effective intensity {Intensity:F2}.");
        }
    }


    /// <summary>
    ///     Adds onset timing jitter to reduce anticipatory responding in attention paradigms.
    /// </summary>
    public class TimingJitterDecorator : StimulusDecorator
    {
        private readonly float _minJitterSeconds;
        private readonly float _maxJitterSeconds;


        /// <summary>
        ///     Initializes a new timing jitter decorator.
        /// </summary>
        /// <param name="wrappedStimulus">The stimulus to augment.</param>
        /// <param name="minJitterSeconds">The minimum jitter in seconds.</param>
        /// <param name="maxJitterSeconds">The maximum jitter in seconds.</param>
        public TimingJitterDecorator(IStimulus wrappedStimulus, float minJitterSeconds, float maxJitterSeconds) : base(wrappedStimulus)
        {
            _minJitterSeconds = Mathf.Min(minJitterSeconds, maxJitterSeconds);
            _maxJitterSeconds = Mathf.Max(minJitterSeconds, maxJitterSeconds);
        }


        /// <summary>
        ///     Gets duration including a sampled temporal jitter offset.
        /// </summary>
        public override float Duration => Mathf.Max(0f, _wrapped.Duration + Random.Range(_minJitterSeconds, _maxJitterSeconds));


        /// <summary>
        ///     Gets description including configured jitter range.
        /// </summary>
        public override string Description => $"{_wrapped.Description} + Timing Jitter ({_minJitterSeconds:F3}s to {_maxJitterSeconds:F3}s)";


        /// <summary>
        ///     Presents the wrapped stimulus and logs sampled onset jitter.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        public override void Present(Transform anchor)
        {
            float sampledJitter = Random.Range(_minJitterSeconds, _maxJitterSeconds);
            _wrapped.Present(anchor);
            Debug.Log($"Onset jitter sampled at {sampledJitter:F3}s for anticipatory-response control.");
        }
    }


    /// <summary>
    ///     Adjusts stimulus contrast/brightness to manipulate visibility thresholds.
    /// </summary>
    public class ContrastFilterDecorator : StimulusDecorator
    {
        private readonly float _contrastMultiplier;


        /// <summary>
        ///     Initializes a new contrast filter decorator.
        /// </summary>
        /// <param name="wrappedStimulus">The stimulus to augment.</param>
        /// <param name="contrastMultiplier">The multiplicative contrast/brightness factor.</param>
        public ContrastFilterDecorator(IStimulus wrappedStimulus, float contrastMultiplier) : base(wrappedStimulus)
        {
            _contrastMultiplier = Mathf.Max(0f, contrastMultiplier);
        }


        /// <summary>
        ///     Gets intensity after applying contrast/brightness scaling.
        /// </summary>
        public override float Intensity => Mathf.Clamp01(_wrapped.Intensity * _contrastMultiplier);


        /// <summary>
        ///     Gets description including contrast filter details.
        /// </summary>
        public override string Description => $"{_wrapped.Description} + Contrast Filter (x{_contrastMultiplier:F2})";


        /// <summary>
        ///     Presents the wrapped stimulus and logs the filtered visibility level.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        public override void Present(Transform anchor)
        {
            _wrapped.Present(anchor);
            Debug.Log($"Contrast filter applied; resulting intensity {Intensity:F2} at {anchor.position}.");
        }
    }
}
