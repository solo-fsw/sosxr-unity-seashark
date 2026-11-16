using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DitzelGames.FastIK
{
    /// <summary>
    /// Solves a transform chain with a FABRIK-style inverse kinematics pass.
    /// </summary>
    public class FastIKFabric : MonoBehaviour
    {
        /// <summary>
        ///     Number of parent bones above this transform that are solved. This transform is the end effector.
        /// </summary>
        public int ChainLength = 2;
        /// <summary>
        ///     Transform the end effector aims for. If null, a FastIK Target GameObject is created at this transform's position during initialization.
        /// </summary>
        public Transform Target;
        /// <summary>
        ///     Optional transform that sets the bend direction of the middle bones. Ignored when null.
        /// </summary>
        public Transform Pole;

        /// <summary>
        ///     Maximum number of solver iterations per solve when the target is within reach.
        /// </summary>
        [Header("Solver Parameters")] public int Iterations = 10;
        /// <summary>
        ///     Distance from the target at which the iterative solve stops early.
        /// </summary>
        public float Delta = 0.001f;
        /// <summary>
        ///     Blend factor (0 to 1) that pulls the chain toward its rest pose before iterating, when the target is within reach.
        /// </summary>
        [Range(0, 1)] public float SnapBackStrength = 1f;

        /// <summary>
        ///     Blend (0 to 1) between the animated bone positions (0) and the solved positions (1).
        /// </summary>
        [Header("IK Weights")] [Range(0, 1)] public float PositionWeight = 1f;
        /// <summary>
        ///     Blend (0 to 1) between the current bone rotations and the solved rotations, applied with Slerp.
        /// </summary>
        [Range(0, 1)] public float RotationWeight = 1f;

        /// <summary>
        ///     Length of each bone segment. Index i is the distance from Bones[i] to Bones[i + 1].
        /// </summary>
        protected float[] BonesLength;
        /// <summary>
        ///     Total length of the chain. The target is out of reach when its distance from the root exceeds this.
        /// </summary>
        protected float CompleteLength;
        /// <summary>
        ///     Transforms in the chain, ordered from the topmost ancestor (index 0) to this transform (last index).
        /// </summary>
        protected Transform[] Bones;
        /// <summary>
        ///     Working bone positions in root space, recalculated on every solve.
        /// </summary>
        protected Vector3[] Positions;
        /// <summary>
        ///     Rest-pose vector from each bone to the next bone in root space. The last entry points from the end transform to the target.
        /// </summary>
        protected Vector3[] StartDirectionSucc;
        /// <summary>
        ///     Rest-pose rotation of each bone in root space, captured during initialization.
        /// </summary>
        protected Quaternion[] StartRotationBone;
        /// <summary>
        ///     Rest-pose rotation of the target in root space, captured during initialization.
        /// </summary>
        protected Quaternion StartRotationTarget;
        /// <summary>
        ///     Ancestor transform the solve is performed relative to, ChainLength + 1 levels above this transform. Null when the chain reaches the scene root.
        /// </summary>
        protected Transform Root;
        private Vector3[] _animatedPositionsBuffer;


        private void Awake()
        {
            Init();
        }


        /// <summary>
        /// Builds solver state and one-time working buffers for the current chain length.
        /// </summary>
        private void Init()
        {
            Bones = new Transform[ChainLength + 1];
            Positions = new Vector3[ChainLength + 1];
            BonesLength = new float[ChainLength];
            StartDirectionSucc = new Vector3[ChainLength + 1];
            StartRotationBone = new Quaternion[ChainLength + 1];

            Root = transform;

            for (var i = 0; i <= ChainLength; i++)
            {
                if (Root == null)
                {
                    throw new UnityException("Chain value exceeds hierarchy length");
                }

                Root = Root.parent;
            }

            if (Target == null)
            {
                Target = new GameObject("FastIK Target").transform;
                SetPositionRootSpace(Target, GetPositionRootSpace(transform));
            }

            StartRotationTarget = GetRotationRootSpace(Target);

            var current = transform;
            CompleteLength = 0;

            for (var i = Bones.Length - 1; i >= 0; i--)
            {
                Bones[i] = current;
                StartRotationBone[i] = GetRotationRootSpace(current);

                if (i == Bones.Length - 1)
                {
                    StartDirectionSucc[i] = GetPositionRootSpace(Target) - GetPositionRootSpace(current);
                }
                else
                {
                    StartDirectionSucc[i] = GetPositionRootSpace(Bones[i + 1]) - GetPositionRootSpace(current);
                    BonesLength[i] = StartDirectionSucc[i].magnitude;
                    CompleteLength += BonesLength[i];
                }

                current = current.parent;
            }
        }


        /// <summary>
        /// Runs the IK solve after animation has updated the source transforms.
        /// </summary>
        private void LateUpdate()
        {
            ResolveIK();
        }


        /// <summary>
        /// Resolves the chain toward the target and blends the result with the animated pose.
        /// </summary>
        private void ResolveIK()
        {
            if (Target == null)
            {
                return;
            }

            if (BonesLength.Length != ChainLength)
            {
                Init();
            }

            if (_animatedPositionsBuffer == null || _animatedPositionsBuffer.Length != Positions.Length)
            {
                // Reallocate only when chain size changes so LateUpdate remains allocation-free.
                _animatedPositionsBuffer = new Vector3[Positions.Length];
            }

            for (var i = 0; i < Bones.Length; i++)
            {
                Positions[i] = GetPositionRootSpace(Bones[i]);
                _animatedPositionsBuffer[i] = Positions[i];
            }

            var targetPosition = GetPositionRootSpace(Target);
            var targetRotation = GetRotationRootSpace(Target);

            if ((targetPosition - Positions[0]).sqrMagnitude >= CompleteLength * CompleteLength)
            {
                var direction = (targetPosition - Positions[0]).normalized;

                for (var i = 1; i < Positions.Length; i++)
                {
                    Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
                }
            }
            else
            {
                for (var i = 0; i < Positions.Length - 1; i++)
                {
                    Positions[i + 1] = Vector3.Lerp(Positions[i + 1], Positions[i] + StartDirectionSucc[i], SnapBackStrength);
                }

                for (var iteration = 0; iteration < Iterations; iteration++)
                {
                    for (var i = Positions.Length - 1; i > 0; i--)
                    {
                        Positions[i] = i == Positions.Length - 1
                            ? targetPosition
                            : Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }

                    for (var i = 1; i < Positions.Length; i++)
                    {
                        Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
                    }

                    if ((Positions[Positions.Length - 1] - targetPosition).sqrMagnitude < Delta * Delta)
                    {
                        break;
                    }
                }
            }

            if (Pole != null)
            {
                var polePosition = GetPositionRootSpace(Pole);

                for (var i = 1; i < Positions.Length - 1; i++)
                {
                    var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                    var projectedPole = plane.ClosestPointOnPlane(polePosition);
                    var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                    var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                    Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
                }
            }

            for (var i = 0; i < Positions.Length; i++)
            {
                var blendedPos = Vector3.Lerp(_animatedPositionsBuffer[i], Positions[i], PositionWeight);
                SetPositionRootSpace(Bones[i], blendedPos);

                if (i == Positions.Length - 1)
                {
                    // Order matches upstream DitzelGames FastIK: Inverse(target) * startTarget * Inverse(startBone), in root space.
                    var finalRot = Quaternion.Inverse(targetRotation) * StartRotationTarget * Quaternion.Inverse(StartRotationBone[i]);
                    Bones[i].rotation = Quaternion.Slerp(Bones[i].rotation, ToWorldRotation(finalRot), RotationWeight);
                }
                else
                {
                    var rot = Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * Quaternion.Inverse(StartRotationBone[i]);
                    Bones[i].rotation = ToWorldRotation(rot);
                }
            }
        }


        private Vector3 GetPositionRootSpace(Transform current)
        {
            return Root == null ? current.position : Quaternion.Inverse(Root.rotation) * (current.position - Root.position);
        }


        private void SetPositionRootSpace(Transform current, Vector3 position)
        {
            if (Root == null)
            {
                current.position = position;
            }
            else
            {
                current.position = Root.rotation * position + Root.position;
            }
        }


        private Quaternion GetRotationRootSpace(Transform current)
        {
            return Root == null ? current.rotation : Quaternion.Inverse(current.rotation) * Root.rotation;
        }


        // Mirrors upstream SetRotationRootSpace: Root is null when the chain reaches the scene root.
        private Quaternion ToWorldRotation(Quaternion rootSpaceRotation)
        {
            return Root == null ? rootSpaceRotation : Root.rotation * rootSpaceRotation;
        }


        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            var current = transform;

            for (var i = 0; i < ChainLength && current != null && current.parent != null; i++)
            {
                var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
                Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
                Handles.color = Color.green;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                current = current.parent;
            }
            #endif
        }
    }
}
