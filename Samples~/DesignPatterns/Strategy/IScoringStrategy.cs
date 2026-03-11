namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Defines a strategy interface for response scoring in psychophysics and attention research.
    ///     Implementations combine accuracy and reaction time into a numeric performance metric.
    /// </summary>
    public interface IScoringStrategy
    {
        /// <summary>
        ///     Gets the display name of the scoring strategy.
        /// </summary>
        string StrategyName { get; }


        /// <summary>
        ///     Scores a response based on reaction time, accuracy, and response deadline constraints.
        /// </summary>
        ///     <param name="reactionTimeMs">Response latency in milliseconds.</param>
        ///     <param name="wasCorrect">True if the response was correct; otherwise false.</param>
        ///     <param name="deadlineMs">Response deadline in milliseconds.</param>
        ///     <returns>A numeric score for the response.</returns>
        float ScoreResponse(float reactionTimeMs, bool wasCorrect, float deadlineMs);
    }
}
