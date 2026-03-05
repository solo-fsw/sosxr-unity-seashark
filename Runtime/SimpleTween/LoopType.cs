namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Determines how a <see cref="TweenSequence" /> repeats when looping.
    /// </summary>
    public enum LoopType
    {
        /// <summary>
        ///     Replays the sequence from the beginning on each loop iteration.
        /// </summary>
        Restart,

        /// <summary>
        ///     Alternates playback direction on each loop iteration (forward, then backward, etc.).
        /// </summary>
        Yoyo
    }
}
