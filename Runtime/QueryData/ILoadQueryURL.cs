namespace SOSXR.SeaShark.QueryData
{
    public interface ILoadQueryURL
    {
        public bool IsReady { get; }


        void LoadURL(string url);
    }
}