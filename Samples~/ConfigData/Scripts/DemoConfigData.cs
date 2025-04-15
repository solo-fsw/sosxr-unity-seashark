using UnityEngine;


namespace SOSXR.SeaShark
{
    [CreateAssetMenu(fileName = "Demo Config Data", menuName = "SOSXR/Config Data/Demo Config Data")]
    public class DemoConfigData : BaseConfigData
    {
        public enum Ordering
        {
            InOrder,
            Random,
            Permutation,
            Counterbalanced
        }


        [SerializeField] private string m_baseURL = "https://youtu.be/xvFZjo5PgG0?si=F3cJFXtwofUAeA";
        [SerializeField] [TextArea] private string m_queryStringURL = "";
        [SerializeField] private string m_taskName = "TaskToDo";
        [SerializeField] private string m_videoName = "VideoName";
        [SerializeField] private int m_PPN = -1;
        [SerializeField] private bool m_showDebug = false;
        [SerializeField] [Range(0, 30)] private int m_debugUpdateInterval = 1;
        [SerializeField] private string m_clipDirectory = "/Users/Mine/Videos";
        [SerializeField] private string[] m_extensions = {".mp4"};
        [SerializeField] private Ordering m_order = Ordering.Counterbalanced;
        [SerializeField] private bool m_showKeyboard = false;
        [SerializeField] private bool m_repeat = false;
        [SerializeField] private Vector3 m_position = new(0, 0, 0);


        public string BaseURL
        {
            get => m_baseURL;
            set => SetValue(ref m_baseURL, value, nameof(BaseURL));
        }

        public string QueryStringURL
        {
            get => m_queryStringURL;
            set => SetValue(ref m_queryStringURL, value, nameof(QueryStringURL));
        }

        public string TaskName
        {
            get => m_taskName;
            set => SetValue(ref m_taskName, value, nameof(TaskName));
        }

        public bool ShowDebug
        {
            get => m_showDebug;
            set => SetValue(ref m_showDebug, value, nameof(ShowDebug));
        }

        public int DebugUpdateInterval
        {
            get => m_debugUpdateInterval;
            set => SetValue(ref m_debugUpdateInterval, value, nameof(DebugUpdateInterval));
        }

        public string VideoName
        {
            get => m_videoName;
            set => SetValue(ref m_videoName, value, nameof(VideoName));
        }

        public int PPN
        {
            get => m_PPN;
            set => SetValue(ref m_PPN, value, nameof(PPN));
        }

        public string PPNStr
        {
            set
            {
                if (int.TryParse(value, out var result))
                {
                    PPN = result;
                }
                else
                {
                    Debug.LogError("Failed to parse PPN from string. Is it an int?");
                }
            }
        }

        public string ClipDirectory
        {
            get => m_clipDirectory;
            set => SetValue(ref m_clipDirectory, value, nameof(ClipDirectory));
        }

        public Ordering Order
        {
            get => m_order;
            set => SetValue(ref m_order, value, nameof(Order));
        }

        public bool ShowKeyboard
        {
            get => m_showKeyboard;
            set => SetValue(ref m_showKeyboard, value, nameof(ShowKeyboard));
        }

        public bool Repeat
        {
            get => m_repeat;
            set => SetValue(ref m_repeat, value, nameof(Repeat));
        }

        public Vector3 Position
        {
            get => m_position;
            set => SetValue(ref m_position, value, nameof(Position));
        }

        public string[] Extensions
        {
            get => m_extensions;
            set => SetValue(ref m_extensions, value, nameof(Extensions));
        }
    }
}