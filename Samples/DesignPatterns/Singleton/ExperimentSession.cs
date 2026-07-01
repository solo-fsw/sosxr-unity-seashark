using System;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates the Singleton pattern with a global experiment session coordinator. Use this for truly shared,
    ///     cross-scene state such as participant metadata, session flow, and consent status. Inherits
    ///     PersistentSingleton so the session survives scene loads across experiment phases.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Experiment Session (Singleton)")]
    public class ExperimentSession : PersistentSingleton<ExperimentSession>
    {
        /// <summary>
        ///     Defines the current high-level experiment session flow state.
        /// </summary>
        public enum SessionState
        {
            /// <summary>Session is in setup before active tasks begin.</summary>
            Setup,

            /// <summary>Session tasks are currently active.</summary>
            Running,

            /// <summary>Session is temporarily paused.</summary>
            Paused,

            /// <summary>Session has been completed.</summary>
            Completed,
        }

        private string _participantId;


        /// <summary>
        ///     Invoked after the session state changes.
        /// </summary>
        public event Action<SessionState> OnSessionStateChanged;

        /// <summary>
        ///     Gets the active session state managed by this singleton.
        /// </summary>
        public SessionState CurrentState { get; private set; } = SessionState.Setup;

        /// <summary>
        ///     Gets or sets participant identifier metadata. This value can only be set once.
        /// </summary>
        public string ParticipantId
        {
            get => _participantId;
            set
            {
                if (!string.IsNullOrEmpty(_participantId))
                {
                    Debug.LogWarning($"{nameof(ExperimentSession)}: ParticipantId is already set and cannot be changed.", this);
                    return;
                }

                _participantId = value;
            }
        }

        /// <summary>
        ///     Gets or sets the current session number.
        /// </summary>
        public int SessionNumber { get; set; }


        /// <summary>
        ///     Transitions the experiment session into the running state and notifies listeners.
        /// </summary>
        public void BeginSession()
        {
            SetState(SessionState.Running);
        }


        /// <summary>
        ///     Transitions the experiment session into the paused state and notifies listeners.
        /// </summary>
        public void PauseSession()
        {
            SetState(SessionState.Paused);
        }


        /// <summary>
        ///     Resumes the experiment session from pause and notifies listeners.
        /// </summary>
        public void ResumeSession()
        {
            SetState(SessionState.Running);
        }


        /// <summary>
        ///     Marks the experiment session as completed and notifies listeners.
        /// </summary>
        public void CompleteSession()
        {
            SetState(SessionState.Completed);
        }


        /// <summary>
        ///     Applies a new state value and invokes <see cref="OnSessionStateChanged"/>.
        /// </summary>
        /// <param name="newState">State value to apply.</param>
        private void SetState(SessionState newState)
        {
            CurrentState = newState;
            OnSessionStateChanged?.Invoke(CurrentState);
        }
    }
}
