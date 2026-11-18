using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates runtime composition of stimulus-presentation modifiers for perception and attention experiments.
    ///     Press 1=mask, 2=noise, 3=jitter, 4=contrast filter, R=reset chain, Space=present stimulus at this transform.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Stimulus Builder (Decorator Example)")]
    public class StimulusBuilderExample : MonoBehaviour
    {
        [SerializeField] private BaseStimulus m_baseStimulus;

        private IStimulus _currentStimulus;


        private void Start()
        {
            ResetStimulus();
        }


        private void Update()
        {
            if (_currentStimulus == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _currentStimulus = new BackwardMaskDecorator(_currentStimulus, 0.05f);
                LogStimulusState();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _currentStimulus = new NoiseOverlayDecorator(_currentStimulus, 0.85f);
                LogStimulusState();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _currentStimulus = new TimingJitterDecorator(_currentStimulus, 0.01f, 0.05f);
                LogStimulusState();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _currentStimulus = new ContrastFilterDecorator(_currentStimulus, 1.1f);
                LogStimulusState();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetStimulus();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currentStimulus.Present(transform);
            }
        }


        /// <summary>
        ///     Resets the chain back to the configured baseline stimulus asset.
        /// </summary>
        public void ResetStimulus()
        {
            if (m_baseStimulus == null)
            {
                Debug.LogWarning($"{nameof(StimulusBuilderExample)} requires a BaseStimulus asset.", this);
                _currentStimulus = null;
                return;
            }

            _currentStimulus = m_baseStimulus;
            LogStimulusState();
        }


        private void LogStimulusState()
        {
            Debug.Log(
                "Current Stimulus: " + _currentStimulus.Description + " | Duration: " + _currentStimulus.Duration.ToString("F3") + "s | Intensity: " + _currentStimulus.Intensity.ToString("F2"),
                this);
        }
    }
}
