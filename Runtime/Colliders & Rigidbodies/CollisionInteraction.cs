using System.Linq;
using UnityEngine;


namespace SOSXR.SeaShark.Colliders___Rigidbodies
{
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionInteraction : ColliderInteractionBase
    {
        [SerializeField] protected Rigidbody m_thisRigidbody;
        [SerializeField] protected Rigidbody[] m_targetRigidbodies;


        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_thisRigidbody == null)
            {
                m_thisRigidbody = GetComponent<Rigidbody>();
            }

            if (m_thisRigidbody == null)
            {
                m_thisRigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }


        protected override bool ValidateColliders()
        {
            if (!FindOtherCollider())
            {
                return false;
            }

            foreach (var otherCollider in m_targetColliders)
            {
                if (otherCollider.isTrigger)
                {
                    Debug.LogError("Collision interactions require non-trigger colliders.");

                    return false;
                }
            }

            // Ensure no trigger colliders on any of the arrays 
            if (m_thisCollider.isTrigger)
            {
                Debug.LogError("Collision interactions require non-trigger colliders.");

                return false;
            }

            // Ensure at least one non-kinematic Rigidbody
            // If this Rigidbody is null or kinematic
            if (m_thisRigidbody == null || m_thisRigidbody.isKinematic)
            {
                // Check if any of the other Rigidbodies are non-kinematic
                if (m_targetRigidbodies == null || !m_targetRigidbodies.Any(rb => rb != null && !rb.isKinematic))
                {
                    Debug.LogError("Collision interactions require at least one non-kinematic Rigidbody.");

                    return false;
                }

                return true;
            }

            return true;
        }


        private void OnCollisionEnter(Collision other)
        {
            Enter(other.collider);
        }


        private void OnCollisionStay(Collision other)
        {
            Stay(other.collider);
        }


        private void OnCollisionExit(Collision other)
        {
            Exit(other.collider);
        }
    }
}