using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     A lightweight finite state machine (FSM) for Unity.
    ///     The FSM delegates Update and FixedUpdate to the active state, ensuring clean separation of behavior per state.
    ///     Use this for experiment flow control, task phase management, adaptive procedure states.
    ///     For more complex needs (hierarchical states, state history), extend this base or consider a pushdown automaton.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        private IState _currentState;

        /// <summary>
        ///     Gets the currently active state.
        /// </summary>
        public IState CurrentState => _currentState;


        /// <summary>
        ///     Changes the active state by exiting the current state and entering the new state.
        ///     If <paramref name="newState"/> is null or already active, no transition is performed.
        /// </summary>
        /// <param name="newState">The state to activate.</param>
        public void ChangeState(IState newState)
        {
            if (newState == null)
            {
                return;
            }

            if (ReferenceEquals(_currentState, newState))
            {
                return;
            }

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }


        private void Update()
        {
            _currentState?.Update();
        }


        private void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }
    }
}
