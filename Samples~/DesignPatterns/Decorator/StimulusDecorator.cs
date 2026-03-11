using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Base decorator that wraps an <see cref="IStimulus"/> for perception-study stimulus composition.
    ///     Subclasses append masking, timing, and visibility behaviors while delegating baseline presentation.
    /// </summary>
    public abstract class StimulusDecorator : IStimulus
    {
        /// <summary>
        ///     The wrapped stimulus instance this decorator augments.
        /// </summary>
        protected IStimulus _wrapped;


        /// <summary>
        ///     Initializes a new decorator around an existing stimulus.
        /// </summary>
        /// <param name="wrappedStimulus">The stimulus instance to wrap.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="wrappedStimulus"/> is null.</exception>
        protected StimulusDecorator(IStimulus wrappedStimulus)
        {
            _wrapped = wrappedStimulus ?? throw new System.ArgumentNullException(nameof(wrappedStimulus));
        }


        /// <summary>
        ///     Gets duration delegated to the wrapped stimulus by default.
        /// </summary>
        public virtual float Duration => _wrapped.Duration;


        /// <summary>
        ///     Gets intensity delegated to the wrapped stimulus by default.
        /// </summary>
        public virtual float Intensity => _wrapped.Intensity;


        /// <summary>
        ///     Gets description delegated to the wrapped stimulus by default.
        /// </summary>
        public virtual string Description => _wrapped.Description;


        /// <summary>
        ///     Presents via the wrapped stimulus by default.
        /// </summary>
        /// <param name="anchor">The spatial anchor used for stimulus presentation.</param>
        public virtual void Present(Transform anchor)
        {
            _wrapped.Present(anchor);
        }
    }
}
