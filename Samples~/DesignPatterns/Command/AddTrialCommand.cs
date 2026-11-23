using System.Collections.Generic;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Encapsulates adding a trial definition to an experiment protocol as an undoable command.
    ///     In the Command pattern, protocol edits are represented as command objects so callers can execute,
    ///     undo, and redo changes without coupling input handling to trial list mutation details.
    ///     This command stores the insertion index and condition data needed for deterministic undo/redo.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Add Trial Command")]
    public class AddTrialCommand : ICommand
    {
        private readonly IList<string> _trials;
        private readonly string _conditionLabel;

        private int _insertedIndex;
        private bool _hasExecuted;


        /// <summary>
        ///     Initializes a new command that adds one trial condition to an experiment protocol list.
        /// </summary>
        /// <param name="trials">Mutable trial collection representing the experiment protocol.</param>
        /// <param name="conditionLabel">Condition label assigned to the added trial.</param>
        public AddTrialCommand(IList<string> trials, string conditionLabel)
        {
            _trials = trials;
            _conditionLabel = conditionLabel;
            _insertedIndex = -1;
        }


        /// <summary>
        ///     Executes the protocol edit by appending a new trial condition to the protocol list.
        /// </summary>
        public void Execute()
        {
            if (_trials == null)
            {
                Debug.LogWarning($"{nameof(AddTrialCommand)}: Trial list is null, cannot execute add.");
                return;
            }

            _insertedIndex = _trials.Count;
            _trials.Insert(_insertedIndex, _conditionLabel ?? string.Empty);
            _hasExecuted = true;
        }


        /// <summary>
        ///     Reverts the protocol edit by removing the trial that was inserted during execution.
        /// </summary>
        public void Undo()
        {
            if (_trials == null || !_hasExecuted)
            {
                return;
            }

            if (_insertedIndex < 0 || _insertedIndex >= _trials.Count)
            {
                return;
            }

            _trials.RemoveAt(_insertedIndex);
        }


        /// <summary>
        ///     Re-applies the protocol edit by re-inserting the trial at its original index.
        /// </summary>
        public void Redo()
        {
            if (_trials == null || !_hasExecuted)
            {
                return;
            }

            int clampedIndex = Mathf.Clamp(_insertedIndex, 0, _trials.Count);
            _trials.Insert(clampedIndex, _conditionLabel ?? string.Empty);
            _insertedIndex = clampedIndex;
        }
    }
}
