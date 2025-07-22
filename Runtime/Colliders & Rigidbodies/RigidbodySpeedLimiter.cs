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

        private float _timer;


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

            if (velocity.magnitude > m_maxSpeed)
            {
                _timer = Mathf.Min(_timer + Time.fixedDeltaTime, 1f); // Clamping timer to prevent infinite growth
                m_rigidbody.linearVelocity = Vector3.Lerp(velocity, Vector3.zero, _timer);

                return;
            }

            _timer = 0f;
        }
    }
}