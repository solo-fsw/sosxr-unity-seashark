namespace SOSXR.SeaShark.Patterns.Command
{
    public abstract class Command
    {
        public abstract void Execute(ICommand command);


        public abstract void Undo();


        public abstract void Redo();
    }
}