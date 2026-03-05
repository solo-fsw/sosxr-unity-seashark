using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Defines a Timeline PlayableAsset that stores a <see cref="MediatorBehaviour"/> template for each clip instance.
    ///     This is the authoring entry point for the Mediator sample, letting designers configure payload data in the
    ///     Timeline inspector while keeping runtime message dispatch in the behaviour itself.
    ///     It implements <see cref="ITimelineClipAsset"/> to describe clip capabilities through <see cref="clipCaps"/>.
    /// </summary>
    [Serializable]
    public class MediatorClip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        ///     Template behaviour copied into the playable instance when this clip is evaluated.
        /// </summary>
        public MediatorBehaviour Template = new();

        /// <summary>
        ///     Gets the capabilities supported by this clip type.
        ///     Returning <see cref="ClipCaps.None"/> indicates the clip does not expose additional Timeline clip operations.
        /// </summary>
        public ClipCaps clipCaps => ClipCaps.None;


        /// <summary>
        ///     Creates the playable for this asset by cloning the configured <see cref="Template"/> behaviour.
        /// </summary>
        /// <param name="graph">Playable graph that owns the created playable.</param>
        /// <param name="owner">GameObject that owns this playable instance.</param>
        /// <returns>A script playable containing a copy of <see cref="Template"/>.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<MediatorBehaviour>.Create(graph, Template);

            return playable;
        }
    }
}
