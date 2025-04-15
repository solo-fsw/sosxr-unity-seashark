using System.Collections.Generic;


namespace SOSXR.SeaShark
{
    public class CommandStack : Command
    {
        private readonly Stack<ICommand> _stack = new();
        private readonly Queue<ICommand> _redoQueue = new();


        public override void Execute(ICommand command)
        {
            command.Execute();
            _stack.Push(command); // Puts at the 0 position of the 'list'
            _redoQueue.Clear();
        }


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
    }
}