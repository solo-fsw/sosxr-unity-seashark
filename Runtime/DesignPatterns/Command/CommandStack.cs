using System.Collections.Generic;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Implements the Command pattern using a LIFO stack for undo/redo history.
    /// This is the counterpart to CommandQueue, which uses FIFO behavior.
    /// Lifecycle:
    /// - Execute adds the command to the stack and clears the redo history.
    /// - Undo pops the most recent command, undoes it, and stores it for redo.
    /// - Redo re-executes the most recently undone command and restores it on the stack.
    /// </summary>
    public class CommandStack : Command
    {
        /// <summary>
        /// Stores executed commands in LIFO order for undo operations.
        /// </summary>
        private readonly Stack<ICommand> _stack = new();

        /// <summary>
        /// Stores undone commands for potential redo operations.
        /// </summary>
        private readonly Queue<ICommand> _redoQueue = new();


        /// <summary>
        /// Executes the command, then pushes it onto the history stack
        /// and clears any redo history.
        /// </summary>
        public override void Execute(ICommand command)
        {
            command.Execute();
            _stack.Push(command); // Puts at the 0 position of the 'list'
            _redoQueue.Clear();
        }


        /// <summary>
        /// Undoes the most recently executed command.
        /// The undone command is moved to the redo history.
        /// If there is no command to undo, the call has no effect.
        /// </summary>
        public override void Undo()
        {
            if (_stack.Count <= 0)
            {
                return;
            }

            var latestCommand = _stack.Pop();
            latestCommand.Undo();

            _redoQueue.Enqueue(latestCommand);
        }


        /// <summary>
        /// Redoes the most recently undone command.
        /// The redone command is moved back onto the main stack.
        /// If there is no command to redo, the call has no effect.
        /// </summary>
        public override void Redo()
        {
            if (_redoQueue.Count <= 0)
            {
                return;
            }

            var latestCommand = _redoQueue.Dequeue();
            latestCommand.Execute();

            _stack.Push(latestCommand);
        }


        /// <summary>
        /// Clears the command history.
        /// Note: This does not clear the redo history.
        /// </summary>
        public override void Clear()
        {
            _stack.Clear();
        }
    }
}
