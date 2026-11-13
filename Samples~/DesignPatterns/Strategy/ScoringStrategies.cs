using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Accuracy-only scoring used in paradigms where correctness is the sole performance metric.
    ///     Returns 1.0 for correct responses and 0.0 for incorrect responses.
    /// </summary>
    public sealed class AccuracyOnlyScoring : IScoringStrategy
    {
        /// <summary>
        ///     Gets the display name of this psychophysics scoring strategy.
        /// </summary>
        public string StrategyName => "Accuracy Only";


        /// <summary>
        ///     Scores responses by correctness only for psychophysics tasks where threshold analysis uses accuracy proportions.
        /// </summary>
        ///     <param name="reactionTimeMs">Unused reaction time value.</param>
        ///     <param name="wasCorrect">True if response was correct.</param>
        ///     <param name="deadlineMs">Unused deadline value.</param>
        ///     <returns>1.0 when correct, otherwise 0.0.</returns>
        public float ScoreResponse(float reactionTimeMs, bool wasCorrect, float deadlineMs)
        {
            return wasCorrect ? 1.0f : 0.0f;
        }
    }


    /// <summary>
    ///     Speed-accuracy scoring that rewards correct and faster responses.
    ///     Incorrect responses receive zero, while correct responses are scaled by proximity to the deadline.
    /// </summary>
    public sealed class SpeedAccuracyScoring : IScoringStrategy
    {
        /// <summary>
        ///     Gets the display name of this reaction-time and accuracy scoring strategy.
        /// </summary>
        public string StrategyName => "Speed-Accuracy";


        /// <summary>
        ///     Scores responses by combining correctness and response speed.
        /// </summary>
        ///     <param name="reactionTimeMs">Response latency in milliseconds.</param>
        ///     <param name="wasCorrect">True if response was correct.</param>
        ///     <param name="deadlineMs">Response deadline in milliseconds.</param>
        ///     <returns>A value from 0.0 to 1.0 where faster correct responses score higher.</returns>
        public float ScoreResponse(float reactionTimeMs, bool wasCorrect, float deadlineMs)
        {
            if (!wasCorrect)
            {
                return 0.0f;
            }

            if (deadlineMs <= 0f)
            {
                return 1.0f;
            }

            return 1.0f - Mathf.Clamp01(reactionTimeMs / deadlineMs);
        }
    }


    /// <summary>
    ///     Inverse efficiency style scoring used in attention and cognitive control research.
    ///     Correct responses return raw RT, while incorrect responses return a penalized RT estimate.
    /// </summary>
    public sealed class InverseEfficiencyScoring : IScoringStrategy
    {
        /// <summary>
        ///     Gets the display name of this attention-research scoring strategy.
        /// </summary>
        public string StrategyName => "Inverse Efficiency";


        /// <summary>
        ///     Scores responses by returning RT for correct and penalized RT for incorrect.
        /// </summary>
        ///     <param name="reactionTimeMs">Response latency in milliseconds.</param>
        ///     <param name="wasCorrect">True if response was correct.</param>
        ///     <param name="deadlineMs">Fallback RT used when supplied RT is non-positive.</param>
        ///     <returns>RT-like score where lower is better.</returns>
        public float ScoreResponse(float reactionTimeMs, bool wasCorrect, float deadlineMs)
        {
            var baseRt = reactionTimeMs > 0f ? reactionTimeMs : Mathf.Max(deadlineMs, 1f);
            return wasCorrect ? baseRt : baseRt * 2.0f;
        }
    }


    /// <summary>
    ///     Deadline-based scoring for time-constrained tasks.
    ///     Correct responses within deadline score full credit, late correct responses score partial credit, and errors score zero.
    /// </summary>
    public sealed class DeadlineScoring : IScoringStrategy
    {
        /// <summary>
        ///     Gets the display name of this reaction-time deadline scoring strategy.
        /// </summary>
        public string StrategyName => "Deadline";


        /// <summary>
        ///     Scores responses against correctness and timing deadline.
        /// </summary>
        ///     <param name="reactionTimeMs">Response latency in milliseconds.</param>
        ///     <param name="wasCorrect">True if response was correct.</param>
        ///     <param name="deadlineMs">Response deadline in milliseconds.</param>
        ///     <returns>1.0 if correct and on time, 0.5 if correct but late, otherwise 0.0.</returns>
        public float ScoreResponse(float reactionTimeMs, bool wasCorrect, float deadlineMs)
        {
            if (!wasCorrect)
            {
                return 0.0f;
            }

            if (reactionTimeMs <= deadlineMs)
            {
                return 1.0f;
            }

            return 0.5f;
        }
    }
}
