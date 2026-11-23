using System.Collections;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates the Factory pattern for generating experiment trial sequences. Each TrialFactory asset defines a condition configuration.
    ///     The sequencer picks factories to produce balanced blocks. To add a new condition, create a new TrialFactory asset — no code changes needed.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Trial Sequence (Factory Example)")]
    public class TrialSequenceExample : MonoBehaviour
    {
        [SerializeField] private TrialFactory[] m_trialFactories;
        [SerializeField] private Transform[] m_spawnPoints;
        [SerializeField] private float m_interBlockInterval = 2f;
        [SerializeField] private int m_trialsPerBlock = 12;


        private void Start()
        {
            StartCoroutine(GenerateBlocksLoop());
        }


        /// <summary>
        ///     Generates one balanced block of trials.
        /// </summary>
        public void GenerateBlock()
        {
            if (m_trialFactories == null || m_trialFactories.Length == 0)
            {
                Debug.LogWarning($"{nameof(TrialSequenceExample)} on '{name}' has no trial factories configured.", this);
                return;
            }

            if (m_spawnPoints == null || m_spawnPoints.Length == 0)
            {
                Debug.LogWarning($"{nameof(TrialSequenceExample)} on '{name}' has no spawn points configured.", this);
                return;
            }

            int validFactoryCount = 0;
            for (int i = 0; i < m_trialFactories.Length; i++)
            {
                if (m_trialFactories[i] != null)
                {
                    validFactoryCount++;
                }
            }

            if (validFactoryCount == 0)
            {
                Debug.LogWarning($"{nameof(TrialSequenceExample)} on '{name}' has only null trial factories.", this);
                return;
            }

            int targetTrialCount = Mathf.Max(1, m_trialsPerBlock);
            TrialFactory[] workingBlock = new TrialFactory[targetTrialCount];
            int index = 0;
            int assigned = 0;
            while (assigned < targetTrialCount)
            {
                TrialFactory candidate = m_trialFactories[index % m_trialFactories.Length];
                if (candidate != null)
                {
                    workingBlock[assigned] = candidate;
                    assigned++;
                }

                index++;
            }

            for (int i = 0; i < workingBlock.Length; i++)
            {
                int swapIndex = Random.Range(i, workingBlock.Length);
                TrialFactory temp = workingBlock[i];
                workingBlock[i] = workingBlock[swapIndex];
                workingBlock[swapIndex] = temp;
            }

            for (int i = 0; i < workingBlock.Length; i++)
            {
                TrialFactory selectedFactory = workingBlock[i];
                Transform selectedSpawnPoint = m_spawnPoints[i % m_spawnPoints.Length];

                if (selectedSpawnPoint == null)
                {
                    continue;
                }

                selectedFactory.Create(selectedSpawnPoint.position, selectedSpawnPoint.rotation);
            }
        }


        private IEnumerator GenerateBlocksLoop()
        {
            while (true)
            {
                GenerateBlock();
                yield return new WaitForSeconds(Mathf.Max(m_interBlockInterval, 0.1f));
            }
        }
    }
}
