using System.Collections.Generic;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Implements the Command pattern using a FIFO queue to maintain undo/redo history.
    /// Lifecycle:
    /// - Execute adds the command to the queue and clears the redo stack.
    /// - Undo removes the command from the queue and pushes it onto the redo stack.
    /// - Redo pops a command from the redo stack and re-executes it.
    /// </summary>
    public class CommandQueue : Command
    {
        /// <summary>Stores executed commands in FIFO order.</summary>
        private readonly Queue<ICommand> _queue = new();
        /// <summary>Stores undone commands for redo.</summary>
        private readonly Stack<ICommand> _redoStack = new();


        /// <summary>Executes the given command, enqueues it, and clears the redo history.</summary>
        /// <param name="command">The command to execute and register for undo/redo.</param>
        public override void Execute(ICommand command)
        {
            command.Execute();
            _queue.Enqueue(command); // Puts at the end of the 'list'
            _redoStack.Clear();
        }


        /// <summary>Dequeues the next command, undoes it, and pushes it onto the redo stack. No operation if the queue is empty.</summary>
        public override void Undo()
        {
            if (_queue.Count <= 0)
            {
                return;
            }

            var latestCommand = _queue.Dequeue();
            latestCommand.Undo();

            _redoStack.Push(latestCommand);
        }


        /// <summary>Pops a command from the redo stack, re-executes it, and enqueues it.</summary>
        /// <remarks>Does nothing if the redo stack is empty.</remarks>
        public override void Redo()
        {
            if (_redoStack.Count <= 0)
            {
                return;
            }

            var latestCommand = _redoStack.Pop();
            latestCommand.Execute();

            _queue.Enqueue(latestCommand);
        }


        /// <summary>Clears the command queue (does not affect the redo stack).</summary>
        public override void Clear()
        {
            _queue.Clear();
        }
    }
}
