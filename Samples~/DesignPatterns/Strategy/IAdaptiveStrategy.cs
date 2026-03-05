namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Defines a strategy interface for adaptive psychophysical procedures.
    ///     Implementations adjust a current difficulty or threshold level after each participant response.
    /// </summary>
    public interface IAdaptiveStrategy
    {
        /// <summary>
        ///     Gets the display name of the adaptive procedure.
        /// </summary>
        string StrategyName { get; }


        /// <summary>
        ///     Gets the current difficulty or threshold level.
        /// </summary>
        float CurrentLevel { get; }


        /// <summary>
        ///     Updates the adaptive procedure after a participant response.
        /// </summary>
        /// <param name="wasCorrect">True if the participant response was correct; otherwise false.</param>
        void UpdateAfterResponse(bool wasCorrect);
    }
}
