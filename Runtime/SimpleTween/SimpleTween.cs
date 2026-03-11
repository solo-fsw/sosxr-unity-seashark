namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Lightweight tweening API for Unity. Provides a DOTween-style fluent interface
    ///     for animating transforms, materials, and arbitrary values without external dependencies.
    /// </summary>
    /// <example>
    ///     <code>
    /// SimpleTween.Sequence()
    ///     .Insert(0f, transform.DOLocalMove(Vector3.up, 1f))
    ///     .Insert(0f, transform.DOScale(Vector3.one * 2f, 1f))
    ///     .SetLoops(2, LoopType.Yoyo);
    /// </code>
    /// </example>
    public static class SimpleTween
    {
        /// <summary>
        ///     Creates a new <see cref="TweenSequence" /> registered with the global <see cref="TweenRunner" />.
        ///     The sequence starts playing automatically on the next frame.
        /// </summary>
        /// <returns>A new, empty <see cref="TweenSequence" /> ready for tween insertion.</returns>
        public static TweenSequence Sequence()
        {
            return TweenRunner.Instance.CreateSequence();
        }
    }
}
