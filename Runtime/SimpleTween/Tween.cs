using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Represents a single property animation that interpolates a value from its current state
    ///     to a target over a specified duration. Tweens are created via <see cref="TweenExtensions" />
    ///     and inserted into a <see cref="TweenSequence" />.
    /// </summary>
    public class Tween
    {
        private readonly Func<object> _getter;
        private readonly Action<object> _setter;
        private readonly Func<object, object, float, object> _lerp;
        private readonly object _endValue;
        private object _startValue;
        private bool _startCaptured;
        private AnimationCurve _easeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


        /// <summary>
        ///     Creates a new tween.
        /// </summary>
        /// <param name="getter">Function that returns the current value of the property being tweened.</param>
        /// <param name="setter">Action that applies an interpolated value to the property.</param>
        /// <param name="endValue">The target value to tween towards.</param>
        /// <param name="duration">Duration of the tween in seconds. Clamped to a minimum of 0.</param>
        /// <param name="lerp">
        ///     Interpolation function that blends <paramref name="getter" /> and <paramref name="endValue" />
        ///     by an eased factor in the range [0, 1].
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="getter" />, <paramref name="setter" />,
        ///     or <paramref name="lerp" /> is <c>null</c>.
        /// </exception>
        internal Tween(Func<object> getter, Action<object> setter, object endValue,
                       float duration, Func<object, object, float, object> lerp)
        {
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _endValue = endValue;
            _lerp = lerp ?? throw new ArgumentNullException(nameof(lerp));
            this.duration = Mathf.Max(0f, duration);
        }


        /// <summary>
        ///     The duration of this tween in seconds.
        /// </summary>
        internal float duration { get; }


        /// <summary>
        ///     Overrides the default linear easing with a custom <see cref="AnimationCurve" />.
        ///     The curve is evaluated with the normalised elapsed time (0–1) and its output
        ///     is used as the interpolation factor.
        /// </summary>
        /// <param name="curve">
        ///     An <see cref="AnimationCurve" /> defining the easing shape.
        ///     Passing <c>null</c> resets the ease to linear.
        /// </param>
        /// <returns>This <see cref="Tween" /> instance for fluent chaining.</returns>
        public Tween SetEase(AnimationCurve curve)
        {
            _easeCurve = curve ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);

            return this;
        }


        /// <summary>
        ///     Clears the captured start value so it will be re-sampled on the next <see cref="Apply" /> call.
        ///     Used internally when a sequence loops.
        /// </summary>
        internal void ResetState()
        {
            _startCaptured = false;
        }


        /// <summary>
        ///     Captures the property's current value as the start of the interpolation range.
        ///     Called once on the first <see cref="Apply" /> invocation (lazy capture).
        /// </summary>
        internal void CaptureStartValue()
        {
            if (_startCaptured)
            {
                return;
            }

            _startValue = _getter();
            _startCaptured = true;
        }


        /// <summary>
        ///     Evaluates the tween at the given elapsed time and applies the interpolated value
        ///     to the target property via the setter.
        /// </summary>
        /// <param name="elapsed">
        ///     Seconds elapsed since this tween's start time within its owning sequence.
        ///     Clamped internally to [0, <see cref="duration" />].
        /// </param>
        internal void Apply(float elapsed)
        {
            CaptureStartValue();

            if (duration <= 0f)
            {
                _setter(_endValue);

                return;
            }

            var normalized = Mathf.Clamp01(elapsed / duration);
            var eased = _easeCurve.Evaluate(normalized);
            var value = _lerp(_startValue, _endValue, eased);
            _setter(value);
        }
    }
}
