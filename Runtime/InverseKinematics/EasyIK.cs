using System.Linq;
using UnityEngine;


namespace joaen
{
    /// <summary>
    ///     Based on joaen: https://github.com/joaen/EasyIK
    /// </summary>
    public class EasyIK : MonoBehaviour
    {
        [Header("IK properties")]
        [Range(0f, 1f)] public float weight = 1f;
        public bool solveIK = true; // Toggle IK solving on/off
        public bool dynamicTargetRotation = false; // Update target rotation each frame

        public int numberOfJoints = 2;
        public Transform ikTarget;
        public int iterations = 10;
        public float tolerance = 0.05f;

        [Header("Pole target (3 joint chain)")]
        public Transform poleTarget;

        private Transform[] jointTransforms;
        private Vector3 startPosition;
        private Vector3[] jointPositions;
        private float[] boneLength;
        private float jointChainLength;
        private float distanceToTarget;
        private Quaternion[] startRotation;
        private Vector3[] jointStartDirection;
        private Quaternion ikTargetStartRot;
        private Quaternion lastJointStartRot;


        private void Awake()
        {
            // Allocate arrays and cache initial rotations and bone directions
            jointChainLength = 0;
            jointTransforms = new Transform[numberOfJoints];
            jointPositions = new Vector3[numberOfJoints];
            boneLength = new float[numberOfJoints];
            jointStartDirection = new Vector3[numberOfJoints];
            startRotation = new Quaternion[numberOfJoints];
            ikTargetStartRot = ikTarget.rotation;

            var current = transform;

            for (var i = 0; i < jointTransforms.Length; i++)
            {
                jointTransforms[i] = current;

                if (i == jointTransforms.Length - 1)
                {
                    lastJointStartRot = current.rotation;

                    return;
                }

                boneLength[i] = Vector3.Distance(current.position, current.GetChild(0).position);
                jointChainLength += boneLength[i];
                jointStartDirection[i] = current.GetChild(0).position - current.position;
                startRotation[i] = current.rotation;

                current = current.GetChild(0);
            }
        }


        private void PoleConstraint()
        {
            if (poleTarget == null)
            {
                return;
            }

            if (numberOfJoints != 3)
            {
                Debug.LogWarning("Pole target constraint requires exactly 3 joints.");

                return;
            }

            var limbAxis = (jointPositions[2] - jointPositions[0]).normalized;
            var poleDirection = (poleTarget.position - jointPositions[0]).normalized;
            var boneDirection = (jointPositions[1] - jointPositions[0]).normalized;

            Vector3.OrthoNormalize(ref limbAxis, ref poleDirection);
            Vector3.OrthoNormalize(ref limbAxis, ref boneDirection);

            var angle = Quaternion.FromToRotation(boneDirection, poleDirection);
            jointPositions[1] = angle * (jointPositions[1] - jointPositions[0]) + jointPositions[0];
        }


        private void Backward()
        {
            // Move chain from tip to root toward the target
            for (var i = jointPositions.Length - 1; i >= 0; i--)
            {
                jointPositions[i] = i == jointPositions.Length - 1
                    ? ikTarget.position
                    : jointPositions[i + 1] + (jointPositions[i] - jointPositions[i + 1]).normalized * boneLength[i];
            }
        }


        private void Forward()
        {
            // Move chain from root to tip preserving bone lengths
            for (var i = 0; i < jointPositions.Length; i++)
            {
                jointPositions[i] = i == 0
                    ? startPosition
                    : jointPositions[i - 1] + (jointPositions[i] - jointPositions[i - 1]).normalized * boneLength[i - 1];
            }
        }


        private void SolveIK()
        {
            // Cache joint positions
            for (var i = 0; i < jointTransforms.Length; i++)
            {
                jointPositions[i] = jointTransforms[i].position;
            }

            // Optionally update starting target rotation
            if (dynamicTargetRotation)
            {
                ikTargetStartRot = ikTarget.rotation;
            }

            distanceToTarget = Vector3.Distance(jointPositions[0], ikTarget.position);

            if (distanceToTarget > jointChainLength)
            {
                // Target is out of reach — fully extend chain toward it
                var direction = (ikTarget.position - jointPositions[0]).normalized;

                for (var i = 1; i < jointPositions.Length; i++)
                {
                    jointPositions[i] = jointPositions[i - 1] + direction * boneLength[i - 1];
                }
            }
            else
            {
                // Iteratively converge on the target within tolerance
                var distToTarget = Vector3.Distance(jointPositions.Last(), ikTarget.position);
                var counter = 0;

                while (distToTarget > tolerance && counter < iterations)
                {
                    startPosition = jointPositions[0];
                    Backward();
                    Forward();
                    distToTarget = Vector3.Distance(jointPositions.Last(), ikTarget.position);
                    counter++;
                }
            }

            // Apply pole constraint for limb bending
            PoleConstraint();

            // Apply solved joint positions and rotations
            for (var i = 0; i < jointPositions.Length - 1; i++)
            {
                var jt = jointTransforms[i];
                jt.position = Vector3.Lerp(jt.position, jointPositions[i], weight);

                var targetRot = Quaternion.FromToRotation(jointStartDirection[i], jointPositions[i + 1] - jointPositions[i]);
                jt.rotation = Quaternion.Slerp(jt.rotation, targetRot * startRotation[i], weight);
            }

            // Apply rotation to the last joint
            var offset = lastJointStartRot * Quaternion.Inverse(ikTargetStartRot);
            var lastRot = ikTarget.rotation * offset;
            jointTransforms.Last().rotation = Quaternion.Slerp(jointTransforms.Last().rotation, lastRot, weight);
        }


        private void LateUpdate()
        {
            if (solveIK)
            {
                SolveIK();
            }
        }
    }
}