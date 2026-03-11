namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Represents one state in a finite state machine where each state encapsulates behavior for a single behavioral mode of operation.
    ///     Each state owns its own Enter/Update/Exit lifecycle, enabling clean separation of experiment phases, task conditions, or interaction modes.
    ///     Each state owns its own Enter/Update/Exit lifecycle, preventing tangled if-else chains and making behavior easier to extend.
    /// </summary>
    public interface IState
    {
        /// <summary>
        ///     Called once when the state becomes active.
        ///     Use this to initialize per-state data or perform one-time setup.
        /// </summary>
        void Enter();


        /// <summary>
        ///     Called every rendered frame while this state is active.
        ///     Use this for frame-based behavior such as input polling, timers, or non-physics movement.
        /// </summary>
        void Update();


        /// <summary>
        ///     Called every physics step while this state is active.
        ///     Use this for physics-related logic that should run in sync with Unity's fixed timestep.
        /// </summary>
        void FixedUpdate();


        /// <summary>
        ///     Called once when the state is about to be replaced by another state.
        ///     Use this to clean up state-specific resources or reset temporary values.
        /// </summary>
        void Exit();
    }
}
