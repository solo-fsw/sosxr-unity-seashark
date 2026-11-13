using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSXR.SeaShark
{
    public class DrawGizmo : MonoBehaviour
    {
        [SerializeField]
        private GizmoType m_type = GizmoType.Cube;

        [SerializeField]
        private Transform m_end;

        [SerializeField]
        private Color m_color = Color.blue;

        [SerializeField]
        [Range(0.01f, 5f)]
        private float m_size = 0.05f;

        [SerializeField]
        private TransformDirections m_direction = TransformDirections.Forward;

        [SerializeField]
        [HideInInspector]
        private Transform m_startTransform;

        private void OnValidate()
        {
            if (m_startTransform == null)
            {
                m_startTransform = transform;
                m_color = Random.ColorHSV();
            }
        }

        private void Reset()
        {
            if (m_startTransform == null)
            {
                m_startTransform = transform;
            }

            m_color = Random.ColorHSV();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_color;

            switch (m_type)
            {
                case GizmoType.Cube:
                    DrawCube();

                    break;

                case GizmoType.WireCube:
                    DrawWireCube();

                    break;

                case GizmoType.Sphere:
                    DrawSphere();

                    break;

                case GizmoType.WireSphere:
                    DrawWireSphere();

                    break;

                case GizmoType.LineBetween:
                    DrawLineBetweenTransforms();

                    break;

                case GizmoType.LinePointing:
                    DrawLineOnTransform();

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawCube()
        {
            Gizmos.matrix = m_startTransform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * m_size);
        }

        private void DrawWireCube()
        {
            Gizmos.matrix = m_startTransform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * m_size);
        }

        private void DrawSphere()
        {
            if (!IsUniformScale(m_startTransform.localScale))
            {
                Debug.LogWarning("Non-uniform scale detected; sphere size may not render correctly.");
            }

            Gizmos.DrawSphere(m_startTransform.position, m_size * m_startTransform.localScale.x);
        }

        private void DrawWireSphere()
        {
            if (!IsUniformScale(m_startTransform.localScale))
            {
                Debug.LogWarning("Non-uniform scale detected; sphere size may not render correctly.");
            }

            Gizmos.DrawWireSphere(m_startTransform.position, m_size * m_startTransform.localScale.x);
        }

        private bool IsUniformScale(Vector3 scale) => Mathf.Approximately(scale.x, scale.y) && Mathf.Approximately(scale.y, scale.z);

        private void DrawLineBetweenTransforms()
        {
            if (m_end == null)
            {
                Debug.LogWarning("Cannot draw gizmos; EndTransform is null!");

                return;
            }

            Gizmos.DrawLine(m_startTransform.position, m_end.position);
        }

        private void DrawLineOnTransform()
        {
            Vector3 position = m_startTransform.position;

            Vector3 directionVector = m_direction switch
            {
                TransformDirections.Forward => m_startTransform.forward,
                TransformDirections.Back => -m_startTransform.forward,
                TransformDirections.Up => m_startTransform.up,
                TransformDirections.Down => -m_startTransform.up,
                TransformDirections.Right => m_startTransform.right,
                TransformDirections.Left => -m_startTransform.right,
                _ => Vector3.zero,
            };

            Gizmos.DrawLine(position, position + (directionVector * m_size));
        }
    }

    public enum GizmoType
    {
        Cube,
        WireCube,
        Sphere,
        WireSphere,
        LinePointing,
        LineBetween,
    }

    public enum TransformDirections
    {
        Forward,
        Back,
        Up,
        Down,
        Right,
        Left,
    }
}
