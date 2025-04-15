namespace SOSXR.SeaShark
{
    public interface ICommand
    {
        void Execute();


        void Undo();


        void Redo();
    }
}