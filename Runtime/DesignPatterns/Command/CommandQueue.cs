using System.Collections.Generic;


namespace SOSXR.SeaShark
{
    public class CommandQueue : Command
    {
        private readonly Queue<ICommand> _queue = new();
        private readonly Stack<ICommand> _redoStack = new();


        public override void Execute(ICommand command)
        {
            command.Execute();
            _queue.Enqueue(command); // Puts at the end of the 'list'
            _redoStack.Clear();
        }


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
    }
}