using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Singleton that drives all active <see cref="TweenSequence" /> instances every frame.
    ///     Automatically creates a hidden, persistent <see cref="GameObject" /> with a
    ///     <c>MonoBehaviour</c> driver to pump the update loop.
    /// </summary>
    /// <remarks>
    ///     This class is <c>internal</c> — consumers should use <see cref="SimpleTween.Sequence" />
    ///     rather than interacting with the runner directly.
    /// </remarks>
    internal sealed class TweenRunner
    {
        private readonly List<TweenSequence> _activeSequences = new();
        private readonly List<TweenSequence> _pendingRemovals = new();


        private TweenRunner()
        {
            EnsureDriverExists();
        }


        /// <summary>
        ///     Global singleton instance. Created lazily on first access.
        /// </summary>
        public static TweenRunner Instance => _instance ??= new TweenRunner();
        private static TweenRunner _instance;


        /// <summary>
        ///     Creates a new <see cref="TweenSequence" />, resets its playback state,
        ///     and registers it for frame updates.
        /// </summary>
        /// <returns>A freshly created, playing <see cref="TweenSequence" />.</returns>
        public TweenSequence CreateSequence()
        {
            var sequence = new TweenSequence();
            sequence.ResetPlayback();
            _activeSequences.Add(sequence);

            return sequence;
        }


        /// <summary>
        ///     Removes a sequence from the active update list. Called by
        ///     <see cref="TweenSequence.Kill" /> when a sequence is stopped early.
        /// </summary>
        /// <param name="sequence">The sequence to remove. <c>null</c> is safely ignored.</param>
        public void Unregister(TweenSequence sequence)
        {
            if (sequence == null)
            {
                return;
            }

            _activeSequences.Remove(sequence);
        }


        /// <summary>
        ///     Advances all active sequences by <paramref name="deltaTime" /> seconds
        ///     and removes any that are no longer active.
        /// </summary>
        /// <param name="deltaTime">Frame delta time in seconds (typically <see cref="Time.deltaTime" />).</param>
        private void Update(float deltaTime)
        {
            if (_activeSequences.Count == 0)
            {
                return;
            }

            _pendingRemovals.Clear();

            for (var i = 0; i < _activeSequences.Count; i++)
            {
                var sequence = _activeSequences[i];
                sequence.Advance(deltaTime);

                if (!sequence.IsActive())
                {
                    _pendingRemovals.Add(sequence);
                }
            }

            for (var i = 0; i < _pendingRemovals.Count; i++)
            {
                _activeSequences.Remove(_pendingRemovals[i]);
            }
        }


        private static void EnsureDriverExists()
        {
            if (TweenRunnerDriver.Instance != null)
            {
                return;
            }

            TweenRunnerDriver.Create();
        }


        /// <summary>
        ///     Hidden <see cref="MonoBehaviour" /> that calls <see cref="TweenRunner.Update" />
        ///     every frame. Persists across scene loads via <see cref="Object.DontDestroyOnLoad" />.
        /// </summary>
        private sealed class TweenRunnerDriver : MonoBehaviour
        {
            public static TweenRunnerDriver Instance { get; private set; }


            public static void Create()
            {
                if (Instance != null)
                {
                    return;
                }

                var go = new GameObject("[SimpleTweenRunner]");
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);
                Instance = go.AddComponent<TweenRunnerDriver>();
            }


            private void Update()
            {
                TweenRunner.Instance.Update(Time.deltaTime);
            }
        }
    }
}
