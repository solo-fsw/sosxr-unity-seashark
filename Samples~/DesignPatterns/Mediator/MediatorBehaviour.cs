using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Header = SOSXR.SeaShark.HeaderAttribute;
using Object = UnityEngine.Object;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Provides the per-clip payload and Timeline lifecycle hooks for the Mediator pattern sample.
    ///     This behaviour acts as a compact data container that publishes strongly-typed messages through
    ///     the SeaShark <see cref="Mediator.Publish(Medium)"/> channel when a clip starts, while it is playing,
    ///     and when it ends. Use this when Timeline clips should broadcast events without tightly coupling
    ///     gameplay systems to the Timeline track itself.
    /// </summary>
    [Serializable]
    public class MediatorBehaviour : PlayableBehaviour
    {
        /// <summary>
        ///     Object payload sent through <see cref="m_onClipPlay"/> when the clip first begins playback.
        /// </summary>
        [SOSXR.SeaShark.Header("On Clip Play")]
        [SerializeField]
        private Object m_thing;

        /// <summary>
        ///     Medium published once from <see cref="OnBehaviourPlay(Playable, FrameData)"/> when the clip starts.
        ///     Receivers should subscribe to this medium for one-time "clip entered" reactions.
        /// </summary>
        [Mediator(true)]
        [SerializeField]
        private Medium m_onClipPlay;

        /// <summary>
        ///     String payload continuously sent through <see cref="m_whileClipPlaying"/> while the director time
        ///     remains within this clip's start/end range.
        /// </summary>
        [SOSXR.SeaShark.Header("While Clip Playing")]
        [SerializeField]
        private string m_thingWhileClipPlaying;

        /// <summary>
        ///     Medium published every frame from <see cref="ProcessFrame(Playable, FrameData, object)"/> while
        ///     the clip is actively within its playable window.
        /// </summary>
        [Mediator(true)]
        [SerializeField]
        private Medium m_whileClipPlaying;

        /// <summary>
        ///     Boolean payload sent through <see cref="m_onClipEnd"/> when playback exits the clip after it has started.
        /// </summary>
        [SOSXR.SeaShark.Header("On Clip End")]
        [SerializeField]
        private bool m_thingOnClipEnd;

        /// <summary>
        ///     Medium published once from <see cref="OnBehaviourPause(Playable, FrameData)"/> after the clip has played.
        ///     Receivers should subscribe to this medium for one-time "clip exited" reactions.
        /// </summary>
        [Mediator(true)]
        [SerializeField]
        private Medium m_onClipEnd;

        private PlayableDirector _director;

        private double _startTime = -1;
        private double _endTime;

        private bool _hasStarted;

        /// <summary>
        ///     Gets or sets the backing Timeline clip that owns this behaviour instance.
        ///     The track assigns this so the behaviour can read clip start/end times.
        /// </summary>
        public TimelineClip TimelineClip { get; set; }

        /// <summary>
        ///     Called when the playable is created in the graph.
        ///     Caches the <see cref="PlayableDirector"/> resolver used to query Timeline time during playback.
        /// </summary>
        /// <param name="playable">The playable instance representing this clip behaviour.</param>
        public override void OnPlayableCreate(Playable playable)
        {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        /// <summary>
        ///     Called when the Timeline graph starts evaluation.
        ///     Resets clip state and snapshots start/end times from <see cref="TimelineClip"/>.
        /// </summary>
        /// <param name="playable">The playable instance representing this clip behaviour.</param>
        public override void OnGraphStart(Playable playable)
        {
            _hasStarted = false;

            if (TimelineClip == null)
            {
                return;
            }

            _startTime = TimelineClip.start;
            _endTime = TimelineClip.end;
        }

        /// <summary>
        ///     Called once when the clip transitions into the playing state.
        ///     Writes the start payload into <see cref="m_onClipPlay"/> and publishes it through SeaShark Mediator.
        /// </summary>
        /// <param name="playable">The playable instance representing this clip behaviour.</param>
        /// <param name="info">Frame timing and evaluation metadata for this play transition.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_onClipPlay.Data = m_thing;
            Mediator.Publish(m_onClipPlay);
            _hasStarted = true;
        }

        /// <summary>
        ///     Called every frame while the graph evaluates.
        ///     Publishes <see cref="m_whileClipPlaying"/> only while the director time is inside this clip window,
        ///     allowing continuous event-based mediation during active playback.
        /// </summary>
        /// <param name="playable">The playable instance representing this clip behaviour.</param>
        /// <param name="info">Frame timing and evaluation metadata for this frame.</param>
        /// <param name="playerData">Optional bound track data supplied by Timeline.</param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_director == null)
            {
                return;
            }

            if (_director.time < _startTime || _director.time > _endTime)
            {
                return;
            }

            m_whileClipPlaying.Data = m_thingWhileClipPlaying;

            Mediator.Publish(m_whileClipPlaying);
        }

        /// <summary>
        ///     Called when the clip leaves the playing state.
        ///     Guards against Timeline's initial pause callback, then publishes <see cref="m_onClipEnd"/> through
        ///     SeaShark Mediator after the clip has truly played.
        /// </summary>
        /// <param name="playable">The playable instance representing this clip behaviour.</param>
        /// <param name="info">Frame timing and evaluation metadata for this pause transition.</param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_director == null)
            {
                return;
            }

            if (_director.time <= _startTime) // REQUIRED CHECK! OnBehaviourPause also runs right after OnGraphStart, so before this clip has actually played.
            {
                return;
            }

            if (!_hasStarted)
            {
                return;
            }

            m_onClipEnd.Data = m_thingOnClipEnd;
            Mediator.Publish(m_onClipEnd);
        }
    }
}
