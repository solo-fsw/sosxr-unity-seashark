namespace SOSXR.SeaShark.Patterns.Command
{
    public interface ICommand
    {
        void Execute();


        void Undo();


        void Redo();
    }
}