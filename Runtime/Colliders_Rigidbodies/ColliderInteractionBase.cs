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
        /// <summary>
        /// Invoked with the other collider when a target enters, as forwarded by derived classes through <see cref="Enter"/>.
        /// </summary>
        public UnityEvent<Collider> OnEnter;
        /// <summary>
        /// Invoked with the other collider every frame a target stays, as forwarded by derived classes through <see cref="Stay"/>.
        /// </summary>
        public UnityEvent<Collider> OnStay;
        /// <summary>
        /// Invoked with the other collider when a target exits, as forwarded by derived classes through <see cref="Exit"/>.
        /// </summary>
        public UnityEvent<Collider> OnExit;

        /// <summary>
        /// Collider on this GameObject used for interactions. Assigned from GetComponent when left empty.
        /// </summary>
        [SerializeField] protected Collider m_thisCollider;

        /// <summary>
        /// Colliders that count as valid interaction targets. Filled from <see cref="Targets"/> when empty.
        /// </summary>
        [SerializeField] protected Collider[] m_targetColliders;

        private HashSet<Collider> _targetColliderSet;

        private bool _initialized;

        private GameObject[] _targets;


        /// <summary>
        /// Optional source objects from which target colliders are discovered.
        /// Setting this (re)builds the target filter, so it works regardless of whether the setter
        /// (e.g. <c>FindGameObjectsWithTag.Awake</c> on the same GameObject) runs before or after our Awake.
        /// </summary>
        public GameObject[] Targets
        {
            get => _targets;
            set
            {
                _targets = value;
                _targetColliderSet = null;
                _initialized = false;
                Initialize();
            }
        }


        /// <summary>
        /// Runs initialization in the Editor when values change.
        /// </summary>
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

            // Explicit == null check: ??= bypasses Unity's fake-null for destroyed/missing references.
            if (m_thisCollider == null)
            {
                m_thisCollider = GetComponent<Collider>();
            }

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
            // Unity deserializes arrays as empty (never null), so treat empty as "not yet found".
            if ((m_targetColliders == null || m_targetColliders.Length == 0) && Targets != null && Targets.Length > 0)
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
            }

            if (_targetColliderSet == null && m_targetColliders != null && m_targetColliders.Length > 0)
            {
                _targetColliderSet = new HashSet<Collider>(m_targetColliders);
            }

            return m_thisCollider != null && m_targetColliders?.Length > 0;
        }


        /// <summary>
        /// Invokes <see cref="OnEnter"/> for the other collider when it is a target, or for any collider if no target set has been built yet.
        /// </summary>
        /// <param name="other">The collider that entered.</param>
        protected void Enter(Collider other)
        {
            if (_targetColliderSet != null && !_targetColliderSet.Contains(other))
            {
                return;
            }

            OnEnter?.Invoke(other);
        }


        /// <summary>
        /// Invokes <see cref="OnStay"/> for the other collider when it is a target, or for any collider if no target set has been built yet.
        /// </summary>
        /// <param name="other">The collider that stays.</param>
        protected void Stay(Collider other)
        {
            if (_targetColliderSet != null && !_targetColliderSet.Contains(other))
            {
                return;
            }

            OnStay?.Invoke(other);
        }


        /// <summary>
        /// Invokes <see cref="OnExit"/> for the other collider when it is a target, or for any collider if no target set has been built yet.
        /// </summary>
        /// <param name="other">The collider that exited.</param>
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
