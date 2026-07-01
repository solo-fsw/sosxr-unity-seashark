using System;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     A ScriptableObject-based trial factory. Create different factory assets for each condition (for example, congruent vs incongruent)
    ///     with distinct stimulus timing and difficulty settings. The sequencer does not need to know condition details — it only calls Create().
    /// </summary>
    [CreateAssetMenu(fileName = "New Trial Factory", menuName = "SOSXR/Design Patterns/Trial Factory")]
    public class TrialFactory : ScriptableObject, IFactory<GameObject>
    {
        [SerializeField] private GameObject m_trialPrefab;
        [SerializeField] private string m_conditionLabel = "Congruent";
        [SerializeField] private float m_stimulusDuration = 0.5f;
        [SerializeField] private float m_interTrialInterval = 1f;
        [SerializeField] [Range(1, 5)] private int m_difficultyLevel = 1;
        [SerializeField] private Color m_highlightColor = Color.cyan;


        /// <summary>
        ///     Creates a trial instance at the world origin with identity rotation.
        /// </summary>
        /// <returns>A configured trial GameObject instance.</returns>
        public GameObject Create()
        {
            return Create(Vector3.zero, Quaternion.identity);
        }


        /// <summary>
        ///     Creates a trial instance at the specified position and rotation.
        /// </summary>
        /// <param name="position">The world position for the generated trial object.</param>
        /// <param name="rotation">The world rotation for the generated trial object.</param>
        /// <returns>A configured trial GameObject instance.</returns>
        public GameObject Create(Vector3 position, Quaternion rotation)
        {
            if (m_trialPrefab == null)
            {
                throw new InvalidOperationException($"{nameof(TrialFactory)} '{name}' requires a trial prefab reference.");
            }

            GameObject instance = Instantiate(m_trialPrefab, position, rotation);
            TrialDefinition trialDefinition = instance.GetComponent<TrialDefinition>();

            if (trialDefinition == null)
            {
                trialDefinition = instance.AddComponent<TrialDefinition>();
            }

            trialDefinition.Initialize(m_conditionLabel, m_stimulusDuration, m_interTrialInterval, m_difficultyLevel, m_highlightColor);
            return instance;
        }
    }
}
