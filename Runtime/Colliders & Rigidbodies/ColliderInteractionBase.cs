using System.Linq;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(Collider))]
    public abstract class ColliderInteractionBase : MonoBehaviour, ITargetsReceiver
    {
        public UnityEvent<Collider> OnEnter;
        public UnityEvent<Collider> OnStay;
        public UnityEvent<Collider> OnExit;

        [SerializeField] protected Collider m_thisCollider;

        [SerializeField] protected Collider[] m_targetColliders;

        private bool _initialized;


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


        protected abstract bool ValidateColliders();


        protected bool FindOtherCollider()
        {
            if (m_targetColliders == null && Targets != null)
            {
                m_targetColliders = Targets
                                    .Where(go => go != null)
                                    .SelectMany(go => go.GetComponents<Collider>())
                                    .ToArray();
            }

            return m_thisCollider != null && m_targetColliders?.Length > 0;
        }


        protected void Enter(Collider other)
        {
            if (!m_targetColliders.Contains(other))
            {
                return;
            }

            OnEnter?.Invoke(other);
        }


        protected void Stay(Collider other)
        {
            if (!m_targetColliders.Contains(other))
            {
                return;
            }

            OnStay?.Invoke(other);
        }


        protected void Exit(Collider other)
        {
            if (!m_targetColliders.Contains(other))
            {
                return;
            }

            OnExit?.Invoke(other);
        }
    }
}