using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DitzelGames.FastIK
{
    public class FastIKFabric : MonoBehaviour
    {
        public int ChainLength = 2;
        public Transform Target;
        public Transform Pole;

        [Header("Solver Parameters")]
        public int Iterations = 10;
        public float Delta = 0.001f;
        [Range(0, 1)] public float SnapBackStrength = 1f;

        [Header("IK Weights")]
        [Range(0, 1)] public float PositionWeight = 1f;
        [Range(0, 1)] public float RotationWeight = 1f;

        protected float[] BonesLength;
        protected float CompleteLength;
        protected Transform[] Bones;
        protected Vector3[] Positions;
        protected Vector3[] StartDirectionSucc;
        protected Quaternion[] StartRotationBone;
        protected Quaternion StartRotationTarget;
        protected Transform Root;


        private void Awake()
        {
            Init();
        }


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
                Target = new GameObject(gameObject.name + " Target").transform;
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


        private void LateUpdate()
        {
            ResolveIK();
        }


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

            var animatedPositions = new Vector3[Positions.Length];

            for (var i = 0; i < Bones.Length; i++)
            {
                Positions[i] = GetPositionRootSpace(Bones[i]);
                animatedPositions[i] = Positions[i];
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
                var blendedPos = Vector3.Lerp(animatedPositions[i], Positions[i], PositionWeight);
                SetPositionRootSpace(Bones[i], blendedPos);

                if (i == Positions.Length - 1)
                {
                    var finalRot = Quaternion.Inverse(StartRotationBone[i]) * StartRotationTarget * Quaternion.Inverse(targetRotation);
                    Bones[i].rotation = Quaternion.Slerp(Bones[i].rotation, Root.rotation * finalRot, RotationWeight);
                }
                else
                {
                    var rot = Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * Quaternion.Inverse(StartRotationBone[i]);
                    Bones[i].rotation = Root.rotation * rot;
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