using System.Collections.Generic;
using ScriptableObjectArchitecture;
using SOSXR.EnhancedLogger;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class CopyTransform : MonoBehaviour
    {
        [Tooltip("Only one of these is going to be used: the one that's active and enabled in Hierarchy")]
        [SerializeField] private List<Transform> m_followTargets;
        [SerializeField] [DisableEditing] private Transform _followTarget;

        [Tooltip("I think this needs be root")]
        [SerializeField] [Optional] private GameObject m_rotationOffset;

        [SerializeField] [Optional] [Postfix("ms")] private IntVariable m_delay;
        [SerializeField] [Optional] private Vector3Variable m_positionOffset;


        private Transform _thisTransform;
        private Command _invoker;


        private void Awake()
        {
            _thisTransform = gameObject.transform;

            _invoker = new CommandStack();
        }


        private void Start()
        {
            GetFirstActiveTargetFromList();

            if (m_rotationOffset == null)
            {
                m_rotationOffset = gameObject.transform.root.gameObject;
            }
        }


        private void GetFirstActiveTargetFromList()
        {
            if (_followTarget != null && _followTarget.gameObject.activeInHierarchy)
            {
                return;
            }

            if (m_followTargets == null || m_followTargets.Count == 0)
            {
                this.Error("Follow targets list is null or empty");

                return;
            }

            /// Don't set this to Linq, which could be stripped in build
            foreach (var target in m_followTargets)
            {
                if (target != null && target.gameObject.activeInHierarchy)
                {
                    _followTarget = target;
                    this.Verbose($"Current {nameof(_followTarget)}: {_followTarget.name}");

                    break;
                }
            }
        }


        private void Update()
        {
            if (m_followTargets == null)
            {
                this.Error("m_followTargets is NULL");
            }

            if (m_followTargets != null && m_followTargets.Count == 0)
            {
                this.Error("m_followTargets is EMPTY");
            }

            /// Don't set this to Linq, which could be stripped in build
            foreach (var t in m_followTargets)
            {
                if (t == null)
                {
                    this.Error("A follow target element is NULL");
                }
            }

            GetFirstActiveTargetFromList();

            if (_followTarget == null)
            {
                // this.Verbose($"We should never have no {nameof(_followTarget)} for {gameObject.name}");

                return;
            }

            if (!_followTarget.gameObject.activeSelf)
            {
                this.Warning($"The chosen {nameof(_followTarget)}: {_followTarget.name}, is not active, this may be a problem for tracking!");
            }

            var delay = 0;

            if (m_delay != null)
            {
                delay = m_delay.Value;
            }

            var delaySec = delay / 1000f;

            var positionOffset = Vector3.zero;

            if (m_positionOffset != null)
            {
                positionOffset = m_positionOffset.Value;
            }

            var newPosition = _followTarget.position + m_rotationOffset.transform.rotation * positionOffset; // To make sure a parented object behaves when the parent rotates.

            var newRotation = _followTarget.rotation;

            if (_invoker == null)
            {
                this.Error("No invoker provided");

                return;
            }

            _invoker.Execute(new CopyTransformCommand(_thisTransform, newPosition, newRotation, delaySec));
        }


        private void OnDisable()
        {
            _invoker.Clear();
        }
    }
}