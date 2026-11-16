using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Moves, rotates or parents its GameObject toward or relative to Target, using the mode selected in m_actionToTake.
    ///     Runs in LateUpdate every frame.
    /// </summary>
    public class Move : MonoBehaviour, ITargetReceiver
    {
        [SerializeField] private ActionToTake m_actionToTake = ActionToTake.MoveTowards;
        [SerializeField] private GameObject m_target;

        // Movement Properties
        [SerializeField] [Range(0f, 25f)] private float m_velocity = 5f;
        [SerializeField] [Range(0f, 360f)] private float m_rotationSpeed = 180f;
        [SerializeField] [Range(0f, 10f)] private float m_springForce = 1f;
        [SerializeField] [Range(0f, 1f)] private float m_damping = 0.98f;

        // Offset Movement Properties
        [SerializeField] private Vector3 m_offset = Vector3.zero;
        /// <summary>
        ///     Control points of the cubic Bezier path. Each segment uses four points and shares endpoints with the next, so the array length is 3n + 1.
        /// </summary>
        [SerializeField] public Vector3[] m_bezierPoints = new Vector3[4];
        [SerializeField] private Vector3 m_direction = Vector3.forward;

        [SerializeField] [Range(0f, 100f)] private float m_orbitRadius = 5f;
        [SerializeField] [Range(0f, 100f)] private float m_orbitSpeed = 50f;
        [SerializeField] [Range(0f, 1f)] private float m_amplitude = 0.01f;
        [SerializeField] [Range(0f, 1f)] private float m_frequency = 0.01f;
        [SerializeField] private Vector3 m_bounceStartPoint = Vector3.zero;
        [SerializeField] private Vector3 m_bounceEndPoint = Vector3.zero;
        [SerializeField] [Range(0f, 10f)] private float m_bounceSpeed = 2f;

        // Height Constraints
        [SerializeField] private Vector2 m_heightConstraints = new(-10f, 10f);
        [SerializeField] [Range(0f, 5f)] private float m_smoothTime = 1.5f;

        /// <summary>
        ///     Whether the Bezier path restarts from the beginning after reaching the end. If false, it stops at the last point.
        /// </summary>
        public bool m_bezierLoop = true;

        private float m_bezierT;
        private float m_orbitAngle;
        private float m_time;
        private bool m_bouncingTowardsEnd = true;
        private Vector3 velocity;

        // Runtime-only elastic velocity, so MoveWithElasticity never changes the serialized m_velocity used by the other modes.
        private float _elasticVelocity;
        private bool _elasticVelocityInitialised;

        /// <summary>
        ///     GameObject that the target-based movement modes move toward, follow or parent to.
        /// </summary>
        public GameObject Target
        {
            get => m_target;
            set => m_target = value;
        }


        private void OnEnable()
        {
            _elasticVelocityInitialised = false;
        }


        private void LateUpdate()
        {
            if (m_actionToTake != ActionToTake.MoveWithElasticity)
            {
                _elasticVelocityInitialised = false;
            }

            switch (m_actionToTake)
            {
                case ActionToTake.ParentTo:
                    ParentTo();

                    break;
                case ActionToTake.SyncTransform:
                    SyncTransform();

                    break;
                case ActionToTake.MoveTowards:
                    MoveTowards();

                    break;
                case ActionToTake.LookAt:
                    LookAtTarget();

                    break;
                case ActionToTake.SmoothLookAtTarget:
                    SmoothLookAtTarget();

                    break;
                case ActionToTake.SmoothMoveTowards:
                    SmoothMoveTowards();

                    break;
                case ActionToTake.MoveByOffset:
                    MoveByOffset();

                    break;
                case ActionToTake.MoveAlongPath:
                    MoveAlongPath();

                    break;

                case ActionToTake.MoveInLocalSpace:
                    MoveInLocalSpace();

                    break;
                case ActionToTake.MoveInCircle:
                    MoveInCircle();

                    break;
                case ActionToTake.MoveSinusoidally:
                    MoveSinusoidally();

                    break;
                case ActionToTake.SmoothBounceBackAndForth:
                    SmoothBounceBackAndForth();

                    break;
                case ActionToTake.MoveWithElasticity:
                    MoveWithElasticity();

                    break;
                case ActionToTake.SmoothFollow:
                    SmoothFollow();

                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        ///     Parents this transform to the target and resets its local position and rotation. Does nothing if no target is set or it is already the parent.
        /// </summary>
        public void ParentTo()
        {
            if (m_target == null)
            {
                return;
            }

            if (transform.parent == m_target.transform)
            {
                return;
            }

            transform.SetParent(m_target.transform);
            transform.rotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }


        /// <summary>
        ///     Copies the target's position and rotation. Does nothing if no target is set.
        /// </summary>
        public void SyncTransform()
        {
            if (m_target == null)
            {
                return;
            }

            transform.position = m_target.transform.position;
            transform.rotation = m_target.transform.rotation;
        }


        /// <summary>
        ///     Moves toward the target position at the configured velocity. Does nothing if no target is set.
        /// </summary>
        public void MoveTowards()
        {
            if (m_target == null)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, m_target.transform.position, m_velocity * Time.deltaTime);
        }


        /// <summary>
        ///     Rotates to face the target. Does nothing if no target is set.
        /// </summary>
        public void LookAtTarget()
        {
            if (m_target == null)
            {
                return;
            }

            transform.LookAt(m_target.transform);
        }


        /// <summary>
        ///     Rotates toward the target at the configured rotation speed. Does nothing if no target is set.
        /// </summary>
        public void SmoothLookAtTarget()
        {
            if (m_target == null)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(m_target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        }


        /// <summary>
        ///     Lerps toward the target position at a rate set by the configured velocity. Does nothing if no target is set.
        /// </summary>
        public void SmoothMoveTowards()
        {
            if (m_target == null)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, m_target.transform.position, m_velocity * Time.deltaTime);
        }


        /// <summary>
        ///     Moves by the offset vector, scaled by the configured velocity and frame time.
        /// </summary>
        public void MoveByOffset()
        {
            transform.position += m_offset * (m_velocity * Time.deltaTime);
        }


        /// <summary>
        ///     Moves along the cubic Bezier path in m_bezierPoints, advancing by the configured velocity. Does nothing if there are
        ///     fewer than four points or the velocity is zero or negative. Loops or clamps at the end depending on m_bezierLoop.
        /// </summary>
        public void MoveAlongPath()
        {
            if (m_bezierPoints.Length < 4 || m_velocity <= 0)
            {
                return;
            }

            // Calculate the total number of segments
            var segmentCount = (m_bezierPoints.Length - 1) / 3;

            if (segmentCount <= 0)
            {
                return;
            }

            // Determine the current segment based on m_bezierT
            var segmentT = m_bezierT * segmentCount;
            var currentSegment = Mathf.FloorToInt(segmentT);

            // Clamp to avoid out-of-bounds, then derive localT so t == 1 maps to the end of the last segment
            currentSegment = Mathf.Clamp(currentSegment, 0, segmentCount - 1);
            var localT = Mathf.Clamp01(segmentT - currentSegment);

            // Get the control points for the current segment
            var startPointIndex = currentSegment * 3;
            var segmentPoints = new Vector3[4];

            for (var i = 0; i < 4; i++)
            {
                segmentPoints[i] = m_bezierPoints[startPointIndex + i];
            }

            // Update the position based on the current segment
            transform.position = GetCubicBezierPoint(segmentPoints, localT);

            // Increment m_bezierT
            m_bezierT += m_velocity * Time.deltaTime / segmentCount;

            // Handle end of path
            if (m_bezierT > 1f)
            {
                if (m_bezierLoop)
                {
                    m_bezierT = 0f; // Loop back to the start
                }
                else
                {
                    m_bezierT = 1f; // Clamp to the end
                    transform.position = GetCubicBezierPoint(segmentPoints, 1f); // Ensure position is set to the last point
                }
            }
        }


        /// <summary>
        ///     Resets the Bezier path progress to the start. Available from the component context menu.
        /// </summary>
        [ContextMenu(nameof(ResetBezier))]
        public void ResetBezier()
        {
            m_bezierT = 0f;
        }


        /// <summary>
        ///     Translates along the direction vector in local space, scaled by the configured velocity.
        /// </summary>
        public void MoveInLocalSpace()
        {
            transform.Translate(m_direction * (m_velocity * Time.deltaTime), Space.Self);
        }


        /// <summary>
        ///     Orbits the target in the XZ plane at the configured radius and speed, keeping the current Y position. Does nothing if no target is set.
        /// </summary>
        public void MoveInCircle()
        {
            if (m_target == null)
            {
                return;
            }

            m_orbitAngle = (m_orbitAngle + m_orbitSpeed * Time.deltaTime) % 360f;
            var center = m_target.transform.position;
            transform.position = new Vector3(center.x + Mathf.Cos(Mathf.Deg2Rad * m_orbitAngle) * m_orbitRadius, transform.position.y, center.z + Mathf.Sin(Mathf.Deg2Rad * m_orbitAngle) * m_orbitRadius);
        }


        /// <summary>
        ///     Adds a sine-based offset to each axis of the position on every call, using the amplitude and frequency settings.
        /// </summary>
        public void MoveSinusoidally()
        {
            m_time += Time.deltaTime;

            // Different phase shifts for each axis to create interesting 3D motion
            var xOffset = Mathf.Sin(m_time * m_frequency) * m_amplitude;
            var yOffset = Mathf.Sin(m_time * m_frequency + Mathf.PI / 2) * m_amplitude;
            var zOffset = Mathf.Sin(m_time * m_frequency + Mathf.PI) * m_amplitude;

            transform.position = new Vector3(
                transform.position.x + xOffset,
                transform.position.y + yOffset,
                transform.position.z + zOffset);
        }


        /// <summary>
        ///     Lerps toward the current bounce destination and flips direction once within 0.1 units of it.
        /// </summary>
        public void SmoothBounceBackAndForth()
        {
            var destination = m_bouncingTowardsEnd ? m_bounceEndPoint : m_bounceStartPoint;
            transform.position = Vector3.Lerp(transform.position, destination, m_bounceSpeed * Time.deltaTime);

            // Only flip once the point we are heading to is reached, otherwise we toggle every frame near either end.
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                m_bouncingTowardsEnd = !m_bouncingTowardsEnd;
            }
        }


        /// <summary>
        ///     Pulls toward the target with a spring force and a decaying elastic velocity. Snaps to the target within 0.01 units.
        ///     Does nothing if no target is set.
        /// </summary>
        public void MoveWithElasticity()
        {
            if (m_target == null)
            {
                return;
            }

            var direction = m_target.transform.position - transform.position;
            var springForce = direction.normalized * m_springForce;
            transform.position += springForce * Time.deltaTime;

            if (!_elasticVelocityInitialised)
            {
                _elasticVelocity = m_velocity;
                _elasticVelocityInitialised = true;
            }

            // Frame-rate independent damping: m_damping is tuned as the per-frame factor at 60 fps.
            _elasticVelocity *= Mathf.Pow(m_damping, Time.deltaTime * 60f);
            transform.position += direction.normalized * (_elasticVelocity * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_target.transform.position) < 0.01f)
            {
                _elasticVelocity = 0f;
                transform.position = m_target.transform.position;
            }
        }


        private void SmoothFollow()
        {
            if (m_target == null)
            {
                return;
            }

            transform.position = ConstrainHeight(Vector3.SmoothDamp(transform.position, m_target.transform.position, ref velocity, m_smoothTime));
        }


        private Vector3 ConstrainHeight(Vector3 position)
        {
            if (position.y < m_heightConstraints.x)
            {
                position.y = m_heightConstraints.x;
            }

            if (position.y > m_heightConstraints.y)
            {
                position.y = m_heightConstraints.y;
            }

            return position;
        }


        /// <summary>
        ///     Evaluates a cubic Bezier curve at parameter t using four control points.
        /// </summary>
        /// <param name="points">Four control points: start, two handles and end.</param>
        /// <param name="t">Curve parameter, from 0 (start) to 1 (end).</param>
        /// <returns>The point on the curve at t.</returns>
        public static Vector3 GetCubicBezierPoint(Vector3[] points, float t)
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            var p = uuu * points[0]; // (1 - t)^3 * P0
            p += 3 * uu * t * points[1]; // 3 * (1 - t)^2 * t * P1
            p += 3 * u * tt * points[2]; // 3 * (1 - t) * t^2 * P2
            p += ttt * points[3]; // t^3 * P3

            return p;
        }
    }


    /// <summary>
    ///     Movement modes that Move runs each frame in LateUpdate.
    /// </summary>
    public enum ActionToTake
    {
        /// <summary>
        ///     Parents this transform to the target and resets its local position and rotation.
        /// </summary>
        ParentTo,
        /// <summary>
        ///     Copies the target's position and rotation every frame.
        /// </summary>
        SyncTransform,
        /// <summary>
        ///     Moves toward the target at the configured velocity.
        /// </summary>
        MoveTowards,
        /// <summary>
        ///     Lerps toward the target at a rate set by the configured velocity.
        /// </summary>
        SmoothMoveTowards,
        /// <summary>
        ///     Rotates to face the target.
        /// </summary>
        LookAt,
        /// <summary>
        ///     Rotates toward the target at the configured rotation speed.
        /// </summary>
        SmoothLookAtTarget,
        /// <summary>
        ///     Moves by the offset vector, scaled by the configured velocity.
        /// </summary>
        MoveByOffset,
        /// <summary>
        ///     Moves along the cubic Bezier path defined by the bezier points.
        /// </summary>
        MoveAlongPath,
        /// <summary>
        ///     Translates along the direction vector in local space.
        /// </summary>
        MoveInLocalSpace,
        /// <summary>
        ///     Orbits the target in the XZ plane at the configured radius and speed.
        /// </summary>
        MoveInCircle,
        /// <summary>
        ///     Adds a sine-based offset to the position on each frame.
        /// </summary>
        MoveSinusoidally,
        /// <summary>
        ///     Lerps back and forth between the bounce start and end points.
        /// </summary>
        SmoothBounceBackAndForth,
        /// <summary>
        ///     Pulls toward the target with a spring force and a decaying elastic velocity.
        /// </summary>
        MoveWithElasticity,
        /// <summary>
        ///     Follows the target with SmoothDamp, with height clamped to the height constraints.
        /// </summary>
        SmoothFollow
    }
}