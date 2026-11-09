using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From: https://github.com/adammyhre/Unity-Utils
    ///     Persistent Regulator singleton, will destroy any other older components of the same type it finds on awake
    /// </summary>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static bool HasInstance => instance != null;

        public float InitializationTime { get; private set; }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();

                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        go.hideFlags = HideFlags.HideAndDontSave;
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }


        /// <summary>
        ///     Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }


        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);

            var oldInstances = FindObjectsByType<T>(FindObjectsInactive.Exclude);

            // Newest wins: every other instance was initialised before (or in the same frame as) this one.
            // Destroying all others also resolves same-frame ties, so exactly one survives.
            foreach (var old in oldInstances)
            {
                if (old != null && old.gameObject != gameObject)
                {
                    Destroy(old.gameObject);
                }
            }

            instance = this as T;
        }
    }
}