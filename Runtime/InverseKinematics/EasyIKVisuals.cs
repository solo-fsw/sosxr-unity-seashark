using UnityEditor;
using UnityEngine;


namespace joaen
{
    /// <summary>
    ///     Visualization helper for EasyIK that draws gizmos for the IK chain
    ///     Based on: https://github.com/joaen/EasyIK
    /// </summary>
    [RequireComponent(typeof(EasyIK))]
    public class EasyIKVisuals : MonoBehaviour
    {
        #if UNITY_EDITOR


        [Header("Visualization Settings")]
        [Tooltip("Draw local rotation axes for joints")]
        [SerializeField] private bool m_showLocalRotationAxis = false;

        [Tooltip("Size of visual gizmos")]
        [Range(0.0f, 1.0f)] [SerializeField] private float m_gizmoSize = 0.05f;

        [Tooltip("Draw lines to pole target")]
        [SerializeField] private bool m_showPoleDirection = false;

        [Tooltip("Draw axis from start to end joint")]
        [SerializeField] private bool m_showPoleRotationAxis = false;

        [SerializeField] private EasyIK m_easyIK;

        private readonly Color boneColor = Color.cyan;


        private void OnValidate()
        {
            if (m_easyIK != null)
            {
                return;
            }

            m_easyIK = GetComponent<EasyIK>();
        }


        private void OnDrawGizmos()
        {
            if (m_easyIK == null)
            {
                return;
            }

            DrawBoneChain();

            if (m_showLocalRotationAxis)
            {
                DrawJointAxes();
            }

            if (m_easyIK.poleTarget != null && m_easyIK.numberOfJoints < 4)
            {
                if (m_showPoleRotationAxis)
                {
                    DrawPoleRotationAxis();
                }

                if (m_showPoleDirection)
                {
                    DrawPoleDirectionLines();
                }
            }
        }


        private void DrawBoneChain()
        {
            var current = transform;
            var child = current.GetChild(0);

            for (var i = 0; i < m_easyIK.numberOfJoints - 1; i++)
            {
                var length = Vector3.Distance(current.position, child.position);
                var midpoint = current.position + (child.position - current.position).normalized * length / 2;
                var rotation = Quaternion.FromToRotation(Vector3.up, (child.position - current.position).normalized);

                DrawWireCapsule(midpoint, rotation, m_gizmoSize, length, boneColor);

                // Move to next bone in chain
                if (i < m_easyIK.numberOfJoints - 2)
                {
                    current = child;
                    child = current.GetChild(0);
                }
            }
        }


        private void DrawJointAxes()
        {
            var current = transform;

            for (var i = 0; i < m_easyIK.numberOfJoints; i++)
            {
                DrawLocalAxes(current);

                if (i < m_easyIK.numberOfJoints - 1)
                {
                    current = current.GetChild(0);
                }
            }
        }


        private void DrawPoleRotationAxis()
        {
            var start = transform;
            var end = GetEndJoint(start);

            Handles.color = Color.white;
            Handles.DrawLine(start.position, end.position);
        }


        private void DrawPoleDirectionLines()
        {
            var start = transform;
            var end = GetEndJoint(start);

            Handles.color = Color.grey;
            Handles.DrawLine(start.position, m_easyIK.poleTarget.position);
            Handles.DrawLine(end.position, m_easyIK.poleTarget.position);
        }


        private Transform GetEndJoint(Transform start)
        {
            var current = start;

            // Navigate to the end joint based on the number of joints
            for (var i = 0; i < m_easyIK.numberOfJoints - 1; i++)
            {
                current = current.GetChild(0);
            }

            return current;
        }


        private void DrawLocalAxes(Transform joint)
        {
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(0, joint.position, joint.rotation * Quaternion.LookRotation(Vector3.right), m_gizmoSize, EventType.Repaint);

            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(0, joint.position, joint.rotation * Quaternion.LookRotation(Vector3.up), m_gizmoSize, EventType.Repaint);

            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(0, joint.position, joint.rotation * Quaternion.LookRotation(Vector3.forward), m_gizmoSize, EventType.Repaint);
        }


        /// <summary>
        ///     Draws a wireframe capsule.
        /// </summary>
        /// <param name="position">Center position of the capsule</param>
        /// <param name="rotation">Rotation of the capsule</param>
        /// <param name="radius">Radius of the capsule</param>
        /// <param name="height">Total height of the capsule</param>
        /// <param name="color">Color of the wireframe</param>
        public static void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height, Color color)
        {
            Handles.color = color;
            var angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (height - radius * 2) / 2;

                // Draw the side arcs
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);

                // Draw the connecting lines
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));

                // Draw the end caps
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
        }


        #endif
    }
}