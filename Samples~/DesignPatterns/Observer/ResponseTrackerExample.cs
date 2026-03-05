using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates the Observer pattern for experiment data flow.
    ///     The response tracker raises events without knowing what listens — data loggers, adaptive difficulty controllers, live accuracy displays, and block transition managers all subscribe independently via <see cref="GameEventListener"/>.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Response Tracker (Observer Example)")]
    public class ResponseTrackerExample : MonoBehaviour
    {
        [SerializeField] private int m_totalTrials = 50;
        [SerializeField] private GameEvent m_onResponseRecorded;
        [SerializeField] private GameEvent m_onAccuracyUpdated;
        [SerializeField] private GameEvent m_onBlockCompleted;

        private int _correctResponses;
        private int _totalResponses;


        /// <summary>
        ///     Gets the number of correct responses recorded in the current block.
        /// </summary>
        public int CorrectResponses => _correctResponses;


        /// <summary>
        ///     Gets the number of responses recorded in the current block.
        /// </summary>
        public int TotalResponses => _totalResponses;


        /// <summary>
        ///     Gets the running response accuracy for the current block.
        /// </summary>
        public float Accuracy => _totalResponses > 0 ? (float)_correctResponses / _totalResponses : 0f;


        /// <summary>
        ///     Records one participant response, updates block accuracy, and raises completion when the block reaches its trial limit.
        /// </summary>
        /// <param name="isCorrect">Whether the recorded response is correct for the current trial.</param>
        public void RecordResponse(bool isCorrect)
        {
            _totalResponses++;

            if (isCorrect)
            {
                _correctResponses++;
            }

            m_onResponseRecorded?.Raise();
            m_onAccuracyUpdated?.Raise();

            if (_totalResponses >= m_totalTrials)
            {
                m_onBlockCompleted?.Raise();
            }
        }


        /// <summary>
        ///     Clears current response counters so a new experimental block can begin.
        /// </summary>
        public void ResetBlock()
        {
            _correctResponses = 0;
            _totalResponses = 0;
        }


        private void Awake()
        {
            m_totalTrials = Mathf.Max(1, m_totalTrials);
        }
    }
}
