using System.Linq;
using SOSXR.SeaShark;
using UnityEngine;


namespace joaen
{
    /// <summary>
    ///     Based on joaen: https://github.com/joaen/EasyIK
    /// </summary>
    public class EasyIK : MonoBehaviour
    {
        public Transform Target;

        [Range(0, 6)] public int NumberOfJoints = 2;
        [Range(0f, 1f)] public float Weight = 1.0f; // Set this value between 0 (animation) and 1 (full IK)
        [Range(0, 20)] public int Iterations = 10;
        [Range(0f, 0.25f)] public float Tolerance = 0.0025f;

        [Header("Pole target (3 joint chain)")]
        [Optional] public Transform PoleTarget;


        private Transform[] _jointTransforms;
        private Vector3 _startPosition;
        private Vector3[] _jointPositions;
        private float[] _boneLength;
        private float _jointChainLength;
        private float _distanceToTarget;
        private Quaternion[] _startRotation;
        private Vector3[] _jointStartDirection;
        private Quaternion _ikTargetStartRot;
        private Quaternion _lastJointStartRot;


        private void Reset()
        {
            if (gameObject.GetComponent<EasyIKVisuals>() == null)
            {
                gameObject.AddComponent<EasyIKVisuals>();
            }
        }


        private void Awake()
        {
            // Let's set some properties
            _jointChainLength = 0;
            _jointTransforms = new Transform[NumberOfJoints];
            _jointPositions = new Vector3[NumberOfJoints];
            _boneLength = new float[NumberOfJoints];
            _jointStartDirection = new Vector3[NumberOfJoints];
            _startRotation = new Quaternion[NumberOfJoints];
            _ikTargetStartRot = Target.rotation;

            var current = transform;

            // For each bone calculate and store the lenght of the bone
            for (var i = 0; i < _jointTransforms.Length; i += 1)
            {
                _jointTransforms[i] = current;

                // If the bones lenght equals the max lenght, we are on the last joint in the chain
                if (i == _jointTransforms.Length - 1)
                {
                    _lastJointStartRot = current.rotation;

                    return;
                }
                // Store length and add the sum of the bone lengths

                _boneLength[i] = Vector3.Distance(current.position, current.GetChild(0).position);
                _jointChainLength += _boneLength[i];

                _jointStartDirection[i] = current.GetChild(0).position - current.position;
                _startRotation[i] = current.rotation;
                // Move the iteration to next joint in the chain
                current = current.GetChild(0);
            }
        }


        private void PoleConstraint()
        {
            if (PoleTarget != null && NumberOfJoints < 4)
            {
                // Get the limb axis direction
                var limbAxis = (_jointPositions[2] - _jointPositions[0]).normalized;

                // Get the direction from the root joint to the pole target and mid joint
                var poleDirection = (PoleTarget.position - _jointPositions[0]).normalized;
                var boneDirection = (_jointPositions[1] - _jointPositions[0]).normalized;

                // Ortho-normalize the vectors
                Vector3.OrthoNormalize(ref limbAxis, ref poleDirection);
                Vector3.OrthoNormalize(ref limbAxis, ref boneDirection);

                // Calculate the angle between the boneDirection vector and poleDirection vector
                var angle = Quaternion.FromToRotation(boneDirection, poleDirection);

                // Rotate the middle bone using the angle
                _jointPositions[1] = angle * (_jointPositions[1] - _jointPositions[0]) + _jointPositions[0];
            }
        }


        private void LateUpdate()
        {
            SolveIK();
        }


        private void SolveIK()
        {
            // Get the jointPositions from the joints
            for (var i = 0; i < _jointTransforms.Length; i += 1)
            {
                _jointPositions[i] = _jointTransforms[i].position;
            }

            // Distance from the root to the ikTarget
            _distanceToTarget = Vector3.Distance(_jointPositions[0], Target.position);

            // IF THE TARGET IS NOT REACHABLE
            if (_distanceToTarget > _jointChainLength)
            {
                // Direction from root to ikTarget
                var direction = Target.position - _jointPositions[0];

                // Get the jointPositions
                for (var i = 1; i < _jointPositions.Length; i += 1)
                {
                    _jointPositions[i] = _jointPositions[i - 1] + direction.normalized * _boneLength[i - 1];
                }
            }
            // IF THE TARGET IS REACHABLE
            else
            {
                // Get the distance from the leaf bone to the ikTarget
                var distToTarget = Vector3.Distance(_jointPositions[_jointPositions.Length - 1], Target.position);
                float counter = 0;

                // While the distance to target is greater than the tolerance let's iterate until we are close enough
                while (distToTarget > Tolerance)
                {
                    _startPosition = _jointPositions[0];
                    Backward();
                    Forward();
                    counter += 1;

                    // After x iterations break the loop to avoid an infinite loop
                    if (counter > Iterations)
                    {
                        break;
                    }
                }
            }

            // Apply the pole constraint
            PoleConstraint();

            // Apply the jointPositions and rotations to the joints
            for (var i = 0; i < _jointPositions.Length - 1; i += 1)
            {
                _jointTransforms[i].position = _jointPositions[i];
                var targetRotation = Quaternion.FromToRotation(_jointStartDirection[i], _jointPositions[i + 1] - _jointPositions[i]);
                _jointTransforms[i].rotation = targetRotation * _startRotation[i];
            }

            // Let's constrain the rotation of the last joint to the IK target and maintain the offset.
            var offset = _lastJointStartRot * Quaternion.Inverse(_ikTargetStartRot);
            _jointTransforms.Last().rotation = Target.rotation * offset;
        }


        private void Backward()
        {
            // Iterate through every position in the list until we reach the start of the chain
            for (var i = _jointPositions.Length - 1; i >= 0; i -= 1)
            {
                Vector3 targetPosition;

                // The last bone position should have the same position as the ikTarget
                if (i == _jointPositions.Length - 1)
                {
                    targetPosition = Target.transform.position;
                }
                else
                {
                    targetPosition = _jointPositions[i + 1] + (_jointPositions[i] - _jointPositions[i + 1]).normalized * _boneLength[i];
                }

                // Blend between the current position and the target position (IK target)
                _jointPositions[i] = Vector3.Lerp(_jointPositions[i], targetPosition, Weight / 2);
            }
        }


        private void Forward()
        {
            // Iterate through every position in the list until we reach the end of the chain
            for (var i = 0; i < _jointPositions.Length; i += 1)
            {
                Vector3 targetPosition;

                // The first bone position should have the same position as the startPosition
                if (i == 0)
                {
                    targetPosition = _startPosition;
                }
                else
                {
                    targetPosition = _jointPositions[i - 1] + (_jointPositions[i] - _jointPositions[i - 1]).normalized * _boneLength[i - 1];
                }

                // Blend between the current position and the target position (IK solution)
                _jointPositions[i] = Vector3.Lerp(_jointPositions[i], targetPosition, Weight / 2);
            }
        }
    }
}