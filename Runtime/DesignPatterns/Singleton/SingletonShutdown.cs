using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Tracks whether the application is quitting, so the singletons do not create a new GameObject when their
    ///     <c>Instance</c> is read from <c>OnDisable</c> or <c>OnDestroy</c> during shutdown (or when leaving Play mode).
    /// </summary>
    internal static class SingletonShutdown
    {
        /// <summary>True from <see cref="Application.quitting" /> until the next play session starts.</summary>
        public static bool IsQuitting { get; private set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            // Reset here: with Domain Reload disabled, static state survives between play sessions.
            IsQuitting = false;
            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
        }


        private static void OnQuitting()
        {
            IsQuitting = true;
        }
    }
}
