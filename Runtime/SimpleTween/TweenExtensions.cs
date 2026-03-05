using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Extension methods that create <see cref="Tween" /> instances for common Unity types.
    ///     These are the primary API for constructing tweens to insert into a <see cref="TweenSequence" />.
    /// </summary>
    /// <example>
    ///     <code>
    /// SimpleTween.Sequence()
    ///     .Insert(0f, transform.DOLocalMove(new Vector3(0, 5, 0), 0.5f))
    ///     .Insert(0f, meshRenderer.material.DOColor(Color.red, _colorPropertyId, 0.5f));
    /// </code>
    /// </example>
    public static class TweenExtensions
    {
        /// <summary>
        ///     Creates a <see cref="Tween" /> that animates the <see cref="Transform.localPosition" />
        ///     of <paramref name="t" /> to <paramref name="target" /> over <paramref name="duration" /> seconds.
        /// </summary>
        /// <param name="t">The transform to animate.</param>
        /// <param name="target">The target local position.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <returns>A new <see cref="Tween" /> ready for insertion into a <see cref="TweenSequence" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="t" /> is <c>null</c>.</exception>
        public static Tween DOLocalMove(this Transform t, Vector3 target, float duration)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return new Tween(() => t.localPosition, value => t.localPosition = (Vector3) value,
                target, duration,
                (start, end, eased) =>
                    Vector3.LerpUnclamped((Vector3) start, (Vector3) end, eased));
        }


        /// <summary>
        ///     Creates a <see cref="Tween" /> that animates the <see cref="Transform.localEulerAngles" />
        ///     of <paramref name="t" /> to <paramref name="eulerTarget" /> over <paramref name="duration" /> seconds.
        /// </summary>
        /// <param name="t">The transform to animate.</param>
        /// <param name="eulerTarget">The target local Euler angles in degrees.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <returns>A new <see cref="Tween" /> ready for insertion into a <see cref="TweenSequence" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="t" /> is <c>null</c>.</exception>
        public static Tween DOLocalRotate(this Transform t, Vector3 eulerTarget, float duration)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return new Tween(() => t.localEulerAngles,
                value => t.localEulerAngles = (Vector3) value, eulerTarget,
                duration,
                (start, end, eased) =>
                    Vector3.LerpUnclamped((Vector3) start, (Vector3) end, eased));
        }


        /// <summary>
        ///     Creates a <see cref="Tween" /> that animates the <see cref="Transform.localScale" />
        ///     of <paramref name="t" /> to <paramref name="target" /> over <paramref name="duration" /> seconds.
        /// </summary>
        /// <param name="t">The transform to animate.</param>
        /// <param name="target">The target local scale.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <returns>A new <see cref="Tween" /> ready for insertion into a <see cref="TweenSequence" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="t" /> is <c>null</c>.</exception>
        public static Tween DOScale(this Transform t, Vector3 target, float duration)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return new Tween(() => t.localScale, value => t.localScale = (Vector3) value,
                target, duration,
                (start, end, eased) =>
                    Vector3.LerpUnclamped((Vector3) start, (Vector3) end, eased));
        }


        /// <summary>
        ///     Creates a <see cref="Tween" /> that animates a <see cref="Material" /> color property
        ///     identified by <paramref name="propertyId" /> to <paramref name="target" />
        ///     over <paramref name="duration" /> seconds.
        /// </summary>
        /// <param name="m">The material to animate.</param>
        /// <param name="target">The target color.</param>
        /// <param name="propertyId">
        ///     The shader property ID obtained via <see cref="Shader.PropertyToID" />.
        /// </param>
        /// <param name="duration">Duration in seconds.</param>
        /// <returns>A new <see cref="Tween" /> ready for insertion into a <see cref="TweenSequence" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="m" /> is <c>null</c>.</exception>
        public static Tween DOColor(this Material m, Color target, int propertyId,
                                    float duration)
        {
            if (m == null)
            {
                throw new ArgumentNullException(nameof(m));
            }

            return new Tween(() => m.GetColor(propertyId),
                value => m.SetColor(propertyId, (Color) value), target,
                duration,
                (start, end, eased) =>
                    Color.LerpUnclamped((Color) start, (Color) end, eased));
        }
    }
}
