using System;
using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     A timeline of <see cref="Tween" /> instances and callbacks that plays automatically
    ///     via the global <see cref="TweenRunner" />. Tweens are positioned at absolute times
    ///     within the sequence, allowing overlapping animations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Create sequences through <see cref="SimpleTween.Sequence" /> rather than
    ///         instantiating this class directly — the factory method registers the sequence
    ///         with the runner so it receives frame updates.
    ///     </para>
    ///     <para>
    ///         Supports looping via <see cref="SetLoops" /> with <see cref="LoopType.Restart" />
    ///         and <see cref="LoopType.Yoyo" /> strategies. Pass <c>-1</c> for infinite loops.
    ///     </para>
    /// </remarks>
    public class TweenSequence
    {
        private readonly List<TweenEntry> _tweens = new();
        private readonly List<CallbackEntry> _callbacks = new();

        private bool _isKilled;
        private bool _isCompleted;
        private int _loops = 1;
        private LoopType _loopType = LoopType.Restart;
        private float _duration;
        private int _completedLoops;
        private float _elapsedTotal;

        /// <summary>
        ///     Current playback position within the active loop iteration, in seconds.
        ///     Ranges from 0 to the total sequence duration.
        /// </summary>
        public float position { get; private set; }

        /// <summary>
        ///     Raised once when the entire sequence (including all loop iterations) finishes playing.
        /// </summary>
        public event Action onComplete;

        /// <summary>
        ///     Raised at the end of each individual loop iteration.
        /// </summary>
        public event Action onStepComplete;


        /// <summary>
        ///     Inserts a <see cref="Tween" /> into the sequence at the specified time offset.
        ///     Multiple tweens can overlap by sharing the same <paramref name="atTime" />.
        /// </summary>
        /// <param name="atTime">
        ///     The time in seconds at which the tween starts playing within this sequence.
        ///     Clamped to a minimum of 0.
        /// </param>
        /// <param name="tween">
        ///     The tween to insert. Passing <c>null</c> is a no-op and returns this sequence unchanged.
        /// </param>
        /// <returns>This <see cref="TweenSequence" /> instance for fluent chaining.</returns>
        public TweenSequence Insert(float atTime, Tween tween)
        {
            if (tween == null)
            {
                return this;
            }

            var entry = new TweenEntry(atTime, tween);
            _tweens.Add(entry);
            _duration = Mathf.Max(_duration, entry.AtTime + tween.duration);

            return this;
        }


        /// <summary>
        ///     Inserts a callback to be invoked at the specified time offset during playback.
        ///     Callbacks fire once per loop iteration when the playback position crosses
        ///     <paramref name="atTime" />.
        /// </summary>
        /// <param name="atTime">
        ///     The time in seconds at which to invoke the callback.
        ///     Clamped to a minimum of 0.
        /// </param>
        /// <param name="callback">
        ///     The action to invoke. Passing <c>null</c> is a no-op and returns this sequence unchanged.
        /// </param>
        /// <returns>This <see cref="TweenSequence" /> instance for fluent chaining.</returns>
        public TweenSequence InsertCallback(float atTime, Action callback)
        {
            if (callback == null)
            {
                return this;
            }

            _callbacks.Add(new CallbackEntry(atTime, callback));
            _duration = Mathf.Max(_duration, Mathf.Max(0f, atTime));

            return this;
        }


        /// <summary>
        ///     Configures the sequence to repeat.
        /// </summary>
        /// <param name="loops">
        ///     Number of times to play the sequence. Use <c>-1</c> for infinite looping.
        ///     A value of <c>0</c> completes the sequence immediately.
        /// </param>
        /// <param name="loopType">
        ///     The looping strategy: <see cref="LoopType.Restart" /> replays from the start;
        ///     <see cref="LoopType.Yoyo" /> alternates direction each iteration.
        /// </param>
        /// <returns>This <see cref="TweenSequence" /> instance for fluent chaining.</returns>
        public TweenSequence SetLoops(int loops, LoopType loopType)
        {
            _loops = loops;
            _loopType = loopType;

            return this;
        }


        /// <summary>
        ///     Registers a callback invoked at the end of each loop iteration.
        ///     Replaces any previously registered step-complete callback.
        /// </summary>
        /// <param name="action">
        ///     The action to invoke on each loop iteration completion.
        ///     Passing <c>null</c> clears the callback.
        /// </param>
        /// <returns>This <see cref="TweenSequence" /> instance for fluent chaining.</returns>
        public TweenSequence OnStepComplete(Action action)
        {
            onStepComplete = null;

            if (action != null)
            {
                onStepComplete += action;
            }

            return this;
        }


        /// <summary>
        ///     Returns <c>true</c> if the sequence is still playing (neither killed nor completed).
        /// </summary>
        public bool IsActive()
        {
            return !_isKilled && !_isCompleted;
        }


        /// <summary>
        ///     Stops the sequence immediately and unregisters it from the <see cref="TweenRunner" />.
        ///     Once killed, the sequence cannot be restarted. Has no effect if already killed or completed.
        /// </summary>
        public void Kill()
        {
            if (_isKilled || _isCompleted)
            {
                return;
            }

            _isKilled = true;
            TweenRunner.Instance.Unregister(this);
        }


        /// <summary>
        ///     Returns the number of loop iterations that have fully completed so far.
        /// </summary>
        public int CompletedLoops()
        {
            return _completedLoops;
        }


        /// <summary>
        ///     Advances the sequence by <paramref name="deltaTime" /> seconds.
        ///     Called each frame by <see cref="TweenRunner" />.
        /// </summary>
        /// <param name="deltaTime">Time in seconds since the last frame.</param>
        internal void Advance(float deltaTime)
        {
            if (_isKilled || _isCompleted)
            {
                return;
            }

            if (_loops == 0)
            {
                _isCompleted = true;
                onComplete?.Invoke();

                return;
            }

            if (deltaTime < 0f)
            {
                deltaTime = 0f;
            }

            if (_duration <= 0f)
            {
                CompleteImmediately();

                return;
            }

            var previousElapsedTotal = _elapsedTotal;
            _elapsedTotal += deltaTime;

            var previousLoopIndex = (int) Mathf.Floor(previousElapsedTotal / _duration);
            var currentLoopIndex = (int) Mathf.Floor(_elapsedTotal / _duration);

            if (currentLoopIndex > previousLoopIndex)
            {
                var maxLoopIndex = _loops < 0 ? currentLoopIndex : Mathf.Min(currentLoopIndex, _loops);

                for (var loop = previousLoopIndex + 1; loop <= maxLoopIndex; loop++)
                {
                    _completedLoops++;
                    onStepComplete?.Invoke();
                }
            }

            FireCallbacks(previousElapsedTotal, _elapsedTotal);

            if (_loops > 0 && _elapsedTotal >= _loops * _duration)
            {
                position = _duration;
                ApplyAtEffectiveTime(GetEffectiveTime(_loops - 1, _duration));
                _isCompleted = true;
                onComplete?.Invoke();

                return;
            }

            var loopIndex = Mathf.Max(0, (int) Mathf.Floor(_elapsedTotal / _duration));
            var timeInLoop = _elapsedTotal - loopIndex * _duration;
            position = Mathf.Clamp(timeInLoop, 0f, _duration);
            var effectiveTime = GetEffectiveTime(loopIndex, timeInLoop);
            ApplyAtEffectiveTime(effectiveTime);
        }


        /// <summary>
        ///     Resets all playback state so the sequence can be replayed from the beginning.
        ///     Also resets the start-value capture on every contained tween.
        /// </summary>
        internal void ResetPlayback()
        {
            _isKilled = false;
            _isCompleted = false;
            _completedLoops = 0;
            position = 0f;
            _elapsedTotal = 0f;

            for (var i = 0; i < _tweens.Count; i++)
            {
                _tweens[i].Tween.ResetState();
            }
        }


        private void CompleteImmediately()
        {
            ApplyAtEffectiveTime(0f);
            _isCompleted = true;

            if (_loops > 0)
            {
                _completedLoops = _loops;
            }

            onComplete?.Invoke();
        }


        private float GetEffectiveTime(int loopIndex, float timeInLoop)
        {
            if (_loopType != LoopType.Yoyo)
            {
                return Mathf.Clamp(timeInLoop, 0f, _duration);
            }

            var oddLoop = (loopIndex & 1) == 1;

            if (!oddLoop)
            {
                return Mathf.Clamp(timeInLoop, 0f, _duration);
            }

            return Mathf.Clamp(_duration - timeInLoop, 0f, _duration);
        }


        private void ApplyAtEffectiveTime(float effectiveTime)
        {
            for (var i = 0; i < _tweens.Count; i++)
            {
                var entry = _tweens[i];
                var elapsed = Mathf.Clamp(effectiveTime - entry.AtTime, 0f, entry.Tween.duration);
                entry.Tween.Apply(elapsed);
            }
        }


        private void FireCallbacks(float previousPosition, float currentPosition)
        {
            if (_callbacks.Count == 0 || currentPosition <= previousPosition || _duration <= 0f)
            {
                return;
            }

            var startLoop = Mathf.Max(0, (int) Mathf.Floor(previousPosition / _duration));
            var endLoop = Mathf.Max(0, (int) Mathf.Floor(currentPosition / _duration));

            if (_loops > 0)
            {
                var maxLoop = Mathf.Max(0, _loops - 1);
                startLoop = Mathf.Min(startLoop, maxLoop);
                endLoop = Mathf.Min(endLoop, maxLoop);
            }

            for (var loop = startLoop; loop <= endLoop; loop++)
            {
                for (var i = 0; i < _callbacks.Count; i++)
                {
                    var callback = _callbacks[i];
                    var triggerInLoop = GetCallbackTriggerTimeInLoop(loop, callback.AtTime);
                    var triggerAbsolute = loop * _duration + triggerInLoop;

                    var isStartTrigger = Mathf.Approximately(previousPosition, 0f) &&
                                         Mathf.Approximately(triggerAbsolute, 0f);

                    if ((triggerAbsolute > previousPosition || isStartTrigger) &&
                        triggerAbsolute <= currentPosition)
                    {
                        callback.Callback?.Invoke();
                    }
                }
            }
        }


        private float GetCallbackTriggerTimeInLoop(int loopIndex, float callbackTime)
        {
            callbackTime = Mathf.Clamp(callbackTime, 0f, _duration);

            if (_loopType != LoopType.Yoyo)
            {
                return callbackTime;
            }

            return (loopIndex & 1) == 1 ? _duration - callbackTime : callbackTime;
        }


        private readonly struct TweenEntry
        {
            public readonly float AtTime;
            public readonly Tween Tween;


            public TweenEntry(float atTime, Tween tween)
            {
                AtTime = Mathf.Max(0f, atTime);
                Tween = tween;
            }
        }


        private readonly struct CallbackEntry
        {
            public readonly float AtTime;
            public readonly Action Callback;


            public CallbackEntry(float atTime, Action callback)
            {
                AtTime = Mathf.Max(0f, atTime);
                Callback = callback;
            }
        }
    }
}
