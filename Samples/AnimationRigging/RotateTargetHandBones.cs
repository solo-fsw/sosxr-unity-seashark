using System;
using System.Collections.Generic;
// using BasteRainGames;
using ScriptableObjectArchitecture;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From: https://discussions.unity.com/t/oculus-handtracking-with-custom-hand-model/772738/4
    ///     And their Github: https://github.com/AlexChesser/VR_ARMS_DEMO/blob/master/Assets/Scripts/FingerIK.cs
    /// </summary>
    public class RotateTargetHandBones : MonoBehaviour
    {
        [Tooltip("We're using the animator as a root gameobject")]
        [SerializeField] private Animator m_animator;
        [SerializeField] [Suffix("ms")] private IntVariable m_delay;

        [Info("Each of the fingers are optional")]
        [SerializeField] private HandTrackingRotationMap m_left;
        [SerializeField] private HandTrackingRotationMap m_right;

        [SerializeField] private bool m_initialised;


        private readonly Queue<RotationFrame> _leftFrames = new();
        private readonly Queue<RotationFrame> _rightFrames = new();


        private void Start()
        {
            FindFingers();
        }


        [Button]
        public void FindFingers()
        {
            foreach (var map in new[] {m_left, m_right})
            {
                foreach (var finger in map.AllFingers())
                {
                    if (finger.Target == null && !string.IsNullOrEmpty(finger.TargetName))
                    {
                        finger.Target = m_animator?.transform.FindChildByName(finger.TargetName);
                    }

                    if (finger.Origin == null && !string.IsNullOrEmpty(finger.OriginName))
                    {
                        finger.Origin = m_animator?.transform.FindChildByName(finger.OriginName);
                    }
                }
            }

            m_initialised = true;
        }


        private void LateUpdate()
        {
            if (!m_initialised)
            {
                return;
            }

            var now = Time.time;
            var delaySec = m_delay.Value / 1000f;
            var targetTime = now + delaySec;

            // Capture and enqueue current rotations
            _leftFrames.Enqueue(new RotationFrame {Time = targetTime, Rotations = CaptureAllRotations(m_left)});
            _rightFrames.Enqueue(new RotationFrame {Time = targetTime, Rotations = CaptureAllRotations(m_right)});

            // Clean up old frames
            while (_leftFrames.Count > 100)
            {
                _leftFrames.Dequeue();
            }

            while (_rightFrames.Count > 100)
            {
                _rightFrames.Dequeue();
            }

            // Apply interpolated rotations
            ApplyInterpolated(_leftFrames, m_left, now);
            ApplyInterpolated(_rightFrames, m_right, now);
        }


        private Dictionary<AdjustRotation, Quaternion> CaptureAllRotations(HandTrackingRotationMap map)
        {
            var rotations = new Dictionary<AdjustRotation, Quaternion>();

            foreach (var finger in map.AllFingers())
            {
                if (finger != null && finger.Origin != null && finger.Enabled)
                {
                    rotations[finger] = finger.Origin.rotation;
                }
            }

            return rotations;
        }


        private void ApplyInterpolated(Queue<RotationFrame> frames, HandTrackingRotationMap map, float now)
        {
            if (frames.Count < 2)
            {
                return;
            }

            var frameArray = frames.ToArray();
            RotationFrame prev = frameArray[0], next = frameArray[0];

            for (var i = 0; i < frameArray.Length - 1; i++)
            {
                if (frameArray[i].Time <= now && frameArray[i + 1].Time >= now)
                {
                    prev = frameArray[i];
                    next = frameArray[i + 1];

                    break;
                }
            }

            var t = GetLerpFactor(now, next, prev);

            RotateFinger(map, prev, next, t);
        }


        private static float GetLerpFactor(float now, RotationFrame next, RotationFrame prev)
        {
            return next.Time > prev.Time ? Mathf.Clamp01((now - prev.Time) / (next.Time - prev.Time)) : 1f;
        }


        private void RotateFinger(HandTrackingRotationMap map, RotationFrame prev, RotationFrame next, float t)
        {
            foreach (var finger in map.AllFingers())
            {
                if (finger == null || finger.Target == null || !finger.Enabled)
                {
                    continue;
                }

                if (prev.Rotations.TryGetValue(finger, out var prevRot) && next.Rotations.TryGetValue(finger, out var nextRot))
                {
                    finger.Target.rotation = Quaternion.Lerp(prevRot, nextRot, t);
                    finger.Target.Rotate(finger.Offset);
                }
            }
        }
    }


    [Serializable]
    public struct RotationFrame
    {
        public float Time;
        [NonSerialized] public Dictionary<AdjustRotation, Quaternion> Rotations;
    }


    [Serializable]
    public class AdjustRotation
    {
        [Tooltip("Add either a name or a Transform")]
        [Optional] public Transform Origin;
        [HideIfNotNull(nameof(Origin))] public string OriginName;

        [Space(10)]
        [Tooltip("Add either a name or a Transform")]
        [Optional] public Transform Target;
        [HideIfNotNull(nameof(Target))] public string TargetName;

        [Space(10)]
        [Optional] public Vector3 Offset;
        public bool Enabled = true;
    }


    [Serializable]
    public class HandTrackingRotationMap
    {
        [Header("Others")]
        [NoFoldOut] public List<AdjustRotation> Others;
        [Header("Fingers")]
        [NoFoldOut] public List<AdjustRotation> Thumb;
        [NoFoldOut] public List<AdjustRotation> Index;
        [NoFoldOut] public List<AdjustRotation> Middle;
        [NoFoldOut] public List<AdjustRotation> Ring;
        [NoFoldOut] public List<AdjustRotation> Pinky;


        public IEnumerable<AdjustRotation> AllFingers()
        {
            foreach (var f in Others)
            {
                yield return f;
            }

            foreach (var f in Thumb)
            {
                yield return f;
            }

            foreach (var f in Index)
            {
                yield return f;
            }

            foreach (var f in Middle)
            {
                yield return f;
            }

            foreach (var f in Ring)
            {
                yield return f;
            }

            foreach (var f in Pinky)
            {
                yield return f;
            }
        }
    }
}