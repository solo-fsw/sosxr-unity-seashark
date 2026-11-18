using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Represents a single trial instance with condition parameters assigned by a <see cref="TrialFactory"/>.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Trial Definition")]
    public class TrialDefinition : MonoBehaviour
    {
        [SerializeField] private Renderer m_renderer;


        /// <summary>
        ///     Gets the condition label assigned to this trial.
        /// </summary>
        public string ConditionLabel { get; private set; }


        /// <summary>
        ///     Gets the stimulus presentation duration in seconds.
        /// </summary>
        public float StimulusDuration { get; private set; }


        /// <summary>
        ///     Gets the interval in seconds before the next trial.
        /// </summary>
        public float InterTrialInterval { get; private set; }


        /// <summary>
        ///     Gets the difficulty level assigned to this trial.
        /// </summary>
        public int DifficultyLevel { get; private set; }


        /// <summary>
        ///     Initializes this trial definition with condition parameters and optional editor color-coding.
        /// </summary>
        /// <param name="conditionLabel">The trial condition label, for example congruent or incongruent.</param>
        /// <param name="stimulusDuration">The stimulus duration in seconds.</param>
        /// <param name="interTrialInterval">The interval in seconds before the next trial.</param>
        /// <param name="difficultyLevel">The condition difficulty level.</param>
        /// <param name="color">The visual color used to differentiate condition types in editor views.</param>
        public void Initialize(string conditionLabel, float stimulusDuration, float interTrialInterval, int difficultyLevel, Color color)
        {
            ConditionLabel = conditionLabel;
            StimulusDuration = stimulusDuration;
            InterTrialInterval = interTrialInterval;
            DifficultyLevel = difficultyLevel;

            if (m_renderer == null)
            {
                m_renderer = GetComponentInChildren<Renderer>();
            }

            if (m_renderer != null)
            {
                m_renderer.material.color = color;
            }
        }
    }
}
