using System;
using UnityEngine.Playables;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Serves as the Timeline mixer behaviour for tracks containing <see cref="MediatorClip"/> instances.
    ///     In this sample the mediator clips publish discrete events, so no numeric blending or state interpolation
    ///     is required between clips. The mixer is intentionally empty to satisfy Timeline's track architecture
    ///     while keeping all event-driven logic inside each clip behaviour.
    /// </summary>
    [Serializable]
    public class MediatorMixer : PlayableBehaviour
    {
    }
}
