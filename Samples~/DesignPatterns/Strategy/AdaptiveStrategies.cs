using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Fixed-difficulty control condition used in psychophysics and behavioral experiments.
    ///     The stimulus level never changes across responses, making it useful as a baseline against adaptive staircases.
    /// </summary>
    public sealed class FixedDifficultyStrategy : IAdaptiveStrategy
    {
        private readonly float _fixedLevel;


        /// <summary>
        ///     Initializes a fixed-difficulty strategy with one constant stimulus level.
        /// </summary>
        /// <param name="fixedLevel">The constant difficulty level to keep throughout the session.</param>
        public FixedDifficultyStrategy(float fixedLevel)
        {
            _fixedLevel = fixedLevel;
        }


        /// <summary>
        ///     Gets the display name of this adaptive strategy.
        /// </summary>
        public string StrategyName => "Fixed Difficulty";


        /// <summary>
        ///     Gets the current difficulty level.
        /// </summary>
        public float CurrentLevel => _fixedLevel;


        /// <summary>
        ///     Ignores participant responses because level is intentionally held constant.
        /// </summary>
        /// <param name="wasCorrect">Unused response correctness flag.</param>
        public void UpdateAfterResponse(bool wasCorrect)
        {
        }
    }


    /// <summary>
    ///     One-up/one-down staircase procedure for threshold estimation in psychophysics.
    ///     Increases difficulty after each correct response and decreases after each incorrect response.
    ///     This simple staircase converges around the 50% correct performance level.
    /// </summary>
    public sealed class OneUpOneDownStaircase : IAdaptiveStrategy
    {
        private readonly float _stepSize;
        private readonly float _minLevel;
        private readonly float _maxLevel;
        private float _currentLevel;


        /// <summary>
        ///     Initializes a 1-up/1-down staircase.
        /// </summary>
        /// <param name="startingLevel">Initial stimulus level.</param>
        /// <param name="stepSize">Step size used after each response.</param>
        /// <param name="minLevel">Lower bound for the stimulus level.</param>
        /// <param name="maxLevel">Upper bound for the stimulus level.</param>
        public OneUpOneDownStaircase(float startingLevel, float stepSize, float minLevel, float maxLevel)
        {
            _stepSize = Mathf.Abs(stepSize);
            _minLevel = Mathf.Min(minLevel, maxLevel);
            _maxLevel = Mathf.Max(minLevel, maxLevel);
            _currentLevel = Mathf.Clamp(startingLevel, _minLevel, _maxLevel);
        }


        /// <summary>
        ///     Gets the display name of this adaptive strategy.
        /// </summary>
        public string StrategyName => "1-Up / 1-Down Staircase";


        /// <summary>
        ///     Gets the current staircase level.
        /// </summary>
        public float CurrentLevel => _currentLevel;


        /// <summary>
        ///     Updates difficulty according to 1-up/1-down rules.
        /// </summary>
        /// <param name="wasCorrect">True when the participant response was correct.</param>
        public void UpdateAfterResponse(bool wasCorrect)
        {
            if (wasCorrect)
            {
                _currentLevel += _stepSize;
            }
            else
            {
                _currentLevel -= _stepSize;
            }

            _currentLevel = Mathf.Clamp(_currentLevel, _minLevel, _maxLevel);
        }
    }


    /// <summary>
    ///     Two-down/one-up staircase procedure commonly used in psychophysical threshold studies.
    ///     Requires two consecutive correct responses to increase difficulty, but only one incorrect response to decrease.
    ///     This rule converges near the 70.7% correct performance threshold.
    /// </summary>
    public sealed class TwoDownOneUpStaircase : IAdaptiveStrategy
    {
        private readonly float _stepSize;
        private readonly float _minLevel;
        private readonly float _maxLevel;
        private float _currentLevel;
        private int _consecutiveCorrectCount;


        /// <summary>
        ///     Initializes a 2-down/1-up staircase.
        /// </summary>
        /// <param name="startingLevel">Initial stimulus level.</param>
        /// <param name="stepSize">Step size used when criterion is met.</param>
        /// <param name="minLevel">Lower bound for the stimulus level.</param>
        /// <param name="maxLevel">Upper bound for the stimulus level.</param>
        public TwoDownOneUpStaircase(float startingLevel, float stepSize, float minLevel, float maxLevel)
        {
            _stepSize = Mathf.Abs(stepSize);
            _minLevel = Mathf.Min(minLevel, maxLevel);
            _maxLevel = Mathf.Max(minLevel, maxLevel);
            _currentLevel = Mathf.Clamp(startingLevel, _minLevel, _maxLevel);
            _consecutiveCorrectCount = 0;
        }


        /// <summary>
        ///     Gets the display name of this adaptive strategy.
        /// </summary>
        public string StrategyName => "2-Down / 1-Up Staircase";


        /// <summary>
        ///     Gets the current staircase level.
        /// </summary>
        public float CurrentLevel => _currentLevel;


        /// <summary>
        ///     Updates difficulty according to 2-down/1-up rules.
        /// </summary>
        /// <param name="wasCorrect">True when the participant response was correct.</param>
        public void UpdateAfterResponse(bool wasCorrect)
        {
            if (wasCorrect)
            {
                _consecutiveCorrectCount++;

                if (_consecutiveCorrectCount >= 2)
                {
                    _currentLevel += _stepSize;
                    _consecutiveCorrectCount = 0;
                }
            }
            else
            {
                _currentLevel -= _stepSize;
                _consecutiveCorrectCount = 0;
            }

            _currentLevel = Mathf.Clamp(_currentLevel, _minLevel, _maxLevel);
        }
    }
}
