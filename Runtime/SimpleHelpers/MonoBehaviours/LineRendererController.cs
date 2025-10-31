using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererController : MonoBehaviour
    {
        [SerializeField] private List<Transform> m_points;
        [SerializeField] [HideInInspector] private LineRenderer _lineRenderer;


        private void OnValidate()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
        }


        private void Update()
        {
            if (m_points == null || m_points.Count == 0)
            {
                return;
            }

            if (_lineRenderer.positionCount != m_points.Count)
            {
                _lineRenderer.positionCount = m_points.Count;
            }

            for (var i = 0; i < m_points.Count; i++)
            {
                _lineRenderer.SetPosition(i, m_points[i].position);
            }
        }
    }
}