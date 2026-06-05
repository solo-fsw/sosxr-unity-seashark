using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Base class for trigger/collision interactions that filter incoming colliders against a target set.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class ColliderInteractionBase : MonoBehaviour, ITargetsReceiver
    {
        public UnityEvent<Collider> OnEnter;
        public UnityEvent<Collider> OnStay;
        public UnityEvent<Collider> OnExit;

        [SerializeField] protected Collider m_thisCollider;

        [SerializeField] protected Collider[] m_targetColliders;

        private HashSet<Collider> _targetColliderSet;

        private bool _initialized;


        /// <summary>
        /// Optional source objects from which target colliders are discovered.
        /// </summary>
        public GameObject[] Targets { get; set; }


        protected virtual void OnValidate()
        {
            Initialize();
        }


        private void Awake()
        {
            Initialize();
        }


        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            m_thisCollider ??= GetComponent<Collider>();

            if (ValidateColliders())
            {
                _initialized = true;
            }
        }


        /// <summary>
        /// Validates component state and required collider references.
        /// </summary>
        protected abstract bool ValidateColliders();


        /// <summary>
        /// Finds target colliders and caches them for fast membership checks.
        /// </summary>
        /// <returns><c>true</c> when this component and at least one target collider are available.</returns>
        protected bool FindOtherCollider()
        {
            if (m_targetColliders == null && Targets != null)
            {
                var colliders = new List<Collider>();

                for (var i = 0; i < Targets.Length; i++)
                {
                    var target = Targets[i];

                    if (target == null)
                    {
                        continue;
                    }

                    // Init path only, but manual loops avoid LINQ iterator and closure allocations.
                    colliders.AddRange(target.GetComponents<Collider>());
                }

                m_targetColliders = colliders.ToArray();
                _targetColliderSet = m_targetColliders.Length > 0 ? new HashSet<Collider>(m_targetColliders) : null;
            }

            return m_thisCollider != null && m_targetColliders?.Length > 0;
        }


        protected void Enter(Collider other)
        {
            if (_targetColliderSet != null && !_targetColliderSet.Contains(other))
            {
                return;
            }

            OnEnter?.Invoke(other);
        }


        protected void Stay(Collider other)
        {
            if (_targetColliderSet != null && !_targetColliderSet.Contains(other))
            {
                return;
            }

            OnStay?.Invoke(other);
        }


        protected void Exit(Collider other)
        {
            if (_targetColliderSet != null && !_targetColliderSet.Contains(other))
            {
                return;
            }

            OnExit?.Invoke(other);
        }
    }
}
