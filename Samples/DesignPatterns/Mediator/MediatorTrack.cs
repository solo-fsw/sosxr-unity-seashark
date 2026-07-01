using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Defines the custom Timeline track that hosts <see cref="MediatorClip"/> instances.
    ///     The track coordinates clip playable creation so each clip behaviour receives its owning
    ///     <see cref="TimelineClip"/> reference, enabling precise start/end time checks used by the
    ///     mediator event dispatch flow.
    /// </summary>
    [TrackClipType(typeof(MediatorClip))] // Tell the track that it can create clips from this binding
    [Serializable]
    public class MediatorTrack : TrackAsset
    {
        /// <summary>
        ///     Creates a playable for the provided clip and forwards that <see cref="TimelineClip"/> into
        ///     the clip template so <see cref="MediatorBehaviour"/> can evaluate Timeline boundaries.
        ///     This override is required because the default track creation path does not automatically
        ///     expose the source clip reference to the behaviour template.
        /// </summary>
        /// <param name="graph">Playable graph that will own the generated clip playable.</param>
        /// <param name="gameObject">Track binding owner passed by Timeline during playable creation.</param>
        /// <param name="clip">Timeline clip currently being converted to a playable.</param>
        /// <returns>The created playable handle for the clip, or <see cref="Playable.Null"/> when unsupported.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="graph"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="clip"/> is null.</exception>
        protected override Playable CreatePlayable(
            PlayableGraph graph,
            GameObject gameObject,
            TimelineClip clip
        )
        {
            if (!graph.IsValid())
            {
                throw new ArgumentException("graph must be a valid PlayableGraph");
            }

            if (clip == null)
            {
                throw new ArgumentNullException(nameof(clip));
            }

            if (clip.asset is IPlayableAsset asset)
            {
                var handle = asset.CreatePlayable(graph, gameObject);

                if (handle.IsValid())
                {
                    handle.SetAnimatedProperties(clip.curves);
                    handle.SetSpeed(clip.timeScale);

                    var currentClip = (MediatorClip)clip.asset;
                    currentClip.Template.TimelineClip = clip;
                }

                return handle;
            }

            return Playable.Null;
        }
    }
}
