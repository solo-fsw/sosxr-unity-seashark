using System.Collections.Generic;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates a minimal experiment protocol editor workflow powered by the Command pattern.
    ///     Trial edits are wrapped as commands and executed through a <see cref="CommandStack"/>,
    ///     enabling deterministic undo and redo of protocol changes using Ctrl+Z and Ctrl+Y.
    ///     This keeps input handling simple while delegating reversible protocol edit logic to commands.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Protocol Editor (Command)")]
    public class ProtocolEditorExample : MonoBehaviour
    {
        /// <summary>
        ///     Optional condition labels used as trial templates while building an experiment protocol.
        /// </summary>
        [SerializeField] private List<string> m_conditionTemplates = new List<string>();

        /// <summary>
        ///     Fallback condition label used when no templates are configured.
        /// </summary>
        [SerializeField] private string m_defaultConditionLabel = "Attention_Baseline";

        /// <summary>
        ///     Backing collection that represents the current ordered trial list of the experiment protocol.
        /// </summary>
        [SerializeField] private List<string> m_protocolTrials = new List<string>();

        /// <summary>
        ///     Current selection index used when deleting a trial from the protocol.
        /// </summary>
        [SerializeField] private int m_selectedTrialIndex;

        private CommandStack _commandStack;
        private int _templateCursor;


        /// <summary>
        ///     Initializes the command stack used for execute/undo/redo protocol edits.
        /// </summary>
        private void Awake()
        {
            _commandStack = new CommandStack();
        }


        /// <summary>
        ///     Polls keyboard shortcuts for trial add/delete and undo/redo protocol operations.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddTrial();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                DeleteSelectedTrial();
            }

            bool controlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (controlPressed && Input.GetKeyDown(KeyCode.Z))
            {
                _commandStack.Undo();
                Debug.Log("Protocol edit undo requested.");
            }

            if (controlPressed && Input.GetKeyDown(KeyCode.Y))
            {
                _commandStack.Redo();
                Debug.Log("Protocol edit redo requested.");
            }
        }


        /// <summary>
        ///     Adds one trial to the experiment protocol using an <see cref="AddTrialCommand"/>.
        /// </summary>
        private void AddTrial()
        {
            string conditionLabel = GetNextConditionLabel();
            AddTrialCommand addTrialCommand = new AddTrialCommand(m_protocolTrials, conditionLabel);

            _commandStack.Execute(addTrialCommand);

            m_selectedTrialIndex = m_protocolTrials.Count - 1;
            Debug.Log("Added trial '" + conditionLabel + "' at index " + m_selectedTrialIndex + ".");
        }


        /// <summary>
        ///     Deletes the currently selected trial through a command so protocol changes remain undoable.
        /// </summary>
        private void DeleteSelectedTrial()
        {
            if (m_protocolTrials == null || m_protocolTrials.Count == 0)
            {
                Debug.Log("No trials available to delete from the experiment protocol.");
                return;
            }

            int clampedIndex = Mathf.Clamp(m_selectedTrialIndex, 0, m_protocolTrials.Count - 1);
            DeleteTrialCommand deleteTrialCommand = new DeleteTrialCommand(m_protocolTrials, clampedIndex);

            _commandStack.Execute(deleteTrialCommand);

            m_selectedTrialIndex = Mathf.Clamp(clampedIndex, 0, m_protocolTrials.Count - 1);
            Debug.Log("Deleted trial at index " + clampedIndex + ".");
        }


        /// <summary>
        ///     Resolves the next condition label used for a new trial in the protocol sequence.
        /// </summary>
        /// <returns>Condition label for the new trial definition.</returns>
        private string GetNextConditionLabel()
        {
            if (m_conditionTemplates != null && m_conditionTemplates.Count > 0)
            {
                int templateIndex = _templateCursor % m_conditionTemplates.Count;
                string templateLabel = m_conditionTemplates[templateIndex];
                _templateCursor++;

                if (!string.IsNullOrWhiteSpace(templateLabel))
                {
                    return templateLabel;
                }
            }

            return string.IsNullOrWhiteSpace(m_defaultConditionLabel)
                ? "Condition" : m_defaultConditionLabel;
        }


        /// <summary>
        ///     Encapsulates trial deletion from an experiment protocol so it can be undone and redone.
        /// </summary>
        private class DeleteTrialCommand : ICommand
        {
            private readonly IList<string> _trials;
            private readonly int _index;

            private string _removedCondition;


            /// <summary>
            ///     Initializes a deletion command for a selected trial index in the protocol.
            /// </summary>
            /// <param name="trials">Mutable trial collection representing the experiment protocol.</param>
            /// <param name="index">Selected trial index that should be removed.</param>
            public DeleteTrialCommand(IList<string> trials, int index)
            {
                _trials = trials;
                _index = index;
            }


            /// <summary>
            ///     Executes the protocol edit by removing the selected trial condition.
            /// </summary>
            public void Execute()
            {
                if (_trials == null)
                {
                    Debug.LogWarning($"{nameof(DeleteTrialCommand)}: Trial list is null, cannot execute delete.");
                    return;
                }

                if (_index < 0 || _index >= _trials.Count)
                {
                    Debug.LogWarning($"{nameof(DeleteTrialCommand)}: Index {_index} is out of range, cannot execute delete.");
                    return;
                }

                _removedCondition = _trials[_index];
                _trials.RemoveAt(_index);
            }


            /// <summary>
            ///     Reverts the protocol edit by inserting the removed trial condition back at its original index.
            /// </summary>
            public void Undo()
            {
                if (_trials == null)
                {
                    return;
                }

                int insertIndex = Mathf.Clamp(_index, 0, _trials.Count);
                _trials.Insert(insertIndex, _removedCondition ?? string.Empty);
            }


            /// <summary>
            ///     Re-applies the trial deletion for redo protocol edits.
            /// </summary>
            public void Redo()
            {
                Execute();
            }
        }
    }
}
