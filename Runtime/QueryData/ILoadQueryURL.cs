namespace SOSXR.SeaShark
{
    public interface ILoadQueryURL
    {
        public bool IsReady { get; }


        void LoadURL(string url);
    }
}