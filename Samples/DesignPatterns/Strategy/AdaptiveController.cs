using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates runtime switching between adaptive psychophysical procedures for threshold estimation.
    ///     This controller routes participant correctness feedback to interchangeable staircase rules that update stimulus level in behavioral research.
    ///     Press Q or E to switch adaptive rules, Y to record a correct response, and N to record an incorrect response.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Adaptive Controller (Strategy Example)")]
    public class AdaptiveController : MonoBehaviour
    {
        [SerializeField] private float m_startingLevel = 0.5f;
        [SerializeField] private float m_stepSize = 0.05f;
        [SerializeField] private float m_minLevel = 0f;
        [SerializeField] private float m_maxLevel = 1f;

        private IAdaptiveStrategy[] _strategies;
        private int _currentStrategyIndex;


        private void Awake()
        {
            _strategies = new IAdaptiveStrategy[]
            {
                new FixedDifficultyStrategy(m_startingLevel),
                new OneUpOneDownStaircase(m_startingLevel, m_stepSize, m_minLevel, m_maxLevel),
                new TwoDownOneUpStaircase(m_startingLevel, m_stepSize, m_minLevel, m_maxLevel)
            };

            _currentStrategyIndex = 0;
            LogCurrentState();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CycleStrategy(-1);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CycleStrategy(1);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                RecordResponse(true);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                RecordResponse(false);
            }
        }


        /// <summary>
        ///     Records a participant response and applies the active adaptive procedure used in psychophysics threshold estimation.
        /// </summary>
        ///     <param name="wasCorrect">True if the participant response was correct; otherwise false.</param>
        public void RecordResponse(bool wasCorrect)
        {
            CurrentStrategy.UpdateAfterResponse(wasCorrect);
            Debug.Log("Response: " + (wasCorrect ? "Correct" : "Incorrect"), this);
            LogCurrentState();
        }


        private IAdaptiveStrategy CurrentStrategy => _strategies[_currentStrategyIndex];


        private void CycleStrategy(int direction)
        {
            _currentStrategyIndex += direction;

            if (_currentStrategyIndex < 0)
            {
                _currentStrategyIndex = _strategies.Length - 1;
            }
            else if (_currentStrategyIndex >= _strategies.Length)
            {
                _currentStrategyIndex = 0;
            }

            LogCurrentState();
        }


        private void LogCurrentState()
        {
            Debug.Log($"Adaptive Strategy: {CurrentStrategy.StrategyName} | Level: {CurrentStrategy.CurrentLevel:0.###}", this);
        }
    }
}
