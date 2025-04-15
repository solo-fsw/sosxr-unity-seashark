namespace SOSXR.SeaShark
{
    public abstract class Command
    {
        public abstract void Execute(ICommand command);


        public abstract void Undo();


        public abstract void Redo();
    }
}