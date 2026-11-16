using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Limits the speed of a Rigidbody by clamping its velocity.
    /// </summary>
    public class RigidbodySpeedLimiter : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private bool m_limitSpeed = true;
        [SerializeField] private float m_maxSpeed = 1f;


        private void OnValidate()
        {
            if (m_rigidbody == null)
            {
                m_rigidbody = GetComponent<Rigidbody>();
            }
        }


        private void FixedUpdate()
        {
            if (m_rigidbody == null || !m_limitSpeed)
            {
                return;
            }

            var velocity = m_rigidbody.linearVelocity;

            // Clamp to max speed; never brake below it.
            if (velocity.sqrMagnitude > m_maxSpeed * m_maxSpeed)
            {
                m_rigidbody.linearVelocity = Vector3.ClampMagnitude(velocity, m_maxSpeed);
            }
        }
    }
}