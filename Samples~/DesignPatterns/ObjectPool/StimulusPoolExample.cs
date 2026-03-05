using System.Collections;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates the Object Pool pattern for rapid stimulus presentation.
    ///     In RSVP paradigms or visual search tasks, hundreds of stimuli may appear per block.
    ///     Pooling eliminates GC spikes that could distort precise timing measurements.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Stimulus Pool Example")]
    public class StimulusPoolExample : MonoBehaviour
    {
        [SerializeField] private GameObjectPool m_stimulusPool;
        [SerializeField] private Transform m_presentationAnchor;
        [SerializeField] private float m_presentationRate = 0.1f;
        [SerializeField] private float m_stimulusDuration = 0.08f;

        private float _nextPresentationTime;


        private void Update()
        {
            if (m_stimulusPool == null || m_presentationAnchor == null)
            {
                return;
            }

            if (!Input.GetKey(KeyCode.Space))
            {
                return;
            }

            if (Time.time < _nextPresentationTime)
            {
                return;
            }

            PresentStimulus();
            _nextPresentationTime = Time.time + Mathf.Max(0.0001f, m_presentationRate);
        }


        /// <summary>
        ///     Retrieves a stimulus display from the pool, places it at the presentation anchor, and schedules its return after the configured display duration.
        /// </summary>
        public void PresentStimulus()
        {
            var stimulus = m_stimulusPool.Get(m_presentationAnchor.position, m_presentationAnchor.rotation);

            if (stimulus.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.linearVelocity = Vector3.zero;
            }

            if (m_stimulusDuration <= 0f)
            {
                return;
            }

            StartCoroutine(ReturnStimulusAfterDelay(stimulus, m_stimulusDuration));
        }


        private IEnumerator ReturnStimulusAfterDelay(GameObject stimulus, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (stimulus == null || m_stimulusPool == null || !stimulus.activeInHierarchy)
            {
                yield break;
            }

            m_stimulusPool.Return(stimulus);
        }
    }
}
