using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates runtime swapping of psychophysics and attention response scoring rules using the Strategy pattern.
    ///     Press 1-4 to switch scoring strategies, Space to simulate a correct response, and X to simulate an incorrect response.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Response Scoring (Strategy Example)")]
    public class ScoringExample : MonoBehaviour
    {
        [SerializeField] private float m_responseDeadlineMs = 1500f;

        private IScoringStrategy[] _strategies;
        private int _currentStrategyIndex;


        private void Awake()
        {
            _strategies = new IScoringStrategy[]
            {
                new AccuracyOnlyScoring(),
                new SpeedAccuracyScoring(),
                new InverseEfficiencyScoring(),
                new DeadlineScoring()
            };

            _currentStrategyIndex = 0;
            Debug.Log($"Scoring Strategy: {CurrentStrategy.StrategyName}", this);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetStrategyIndex(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetStrategyIndex(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetStrategyIndex(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetStrategyIndex(3);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SimulateResponse(true);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                SimulateResponse(false);
            }
        }


        private IScoringStrategy CurrentStrategy => _strategies[_currentStrategyIndex];


        private void SetStrategyIndex(int index)
        {
            if (index < 0 || index >= _strategies.Length)
            {
                return;
            }

            if (_currentStrategyIndex == index)
            {
                return;
            }

            _currentStrategyIndex = index;
            Debug.Log($"Scoring Strategy: {CurrentStrategy.StrategyName}", this);
        }


        private void SimulateResponse(bool wasCorrect)
        {
            var reactionTimeMs = Random.Range(200f, 2000f);
            var score = CurrentStrategy.ScoreResponse(reactionTimeMs, wasCorrect, m_responseDeadlineMs);

            var sb = new System.Text.StringBuilder(128);
            sb.Append('[').Append(CurrentStrategy.StrategyName).Append("] Correct=").Append(wasCorrect)
              .Append(", RT=").Append(reactionTimeMs.ToString("0")).Append(" ms, Deadline=")
              .Append(m_responseDeadlineMs.ToString("0")).Append(" ms, Score=")
              .Append(score.ToString("0.###"));
            Debug.Log(sb.ToString(), this);
        }
    }
}
