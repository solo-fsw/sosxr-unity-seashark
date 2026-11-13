using System.Collections.Generic;
using UnityEngine;

namespace SOSXR.SeaShark
{
    public class CopyTransform : MonoBehaviour
    {
        [Tooltip("Only one of these is going to be used: the one that's active and enabled in Hierarchy")]
        [SerializeField]
        private List<Transform> m_followTargets;

        [SerializeField]
        [DisableEditing]
        private Transform _followTarget;

        [Tooltip("I think this needs be root")]
        [SerializeField]
        [Optional]
        private GameObject m_rotationOffset;

        [SerializeField]
        [Optional]
        [Postfix("ms")]
        private int m_delay;

        [SerializeField]
        [Optional]
        private Vector3 m_positionOffset;

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
                Debug.LogError("Follow targets list is null or empty");

                return;
            }

            /// Don't set this to Linq, which could be stripped in build
            foreach (var target in m_followTargets)
            {
                if (target != null && target.gameObject.activeInHierarchy)
                {
                    _followTarget = target;

                    break;
                }
            }
        }

        private void Update()
        {
            if (m_followTargets == null)
            {
                Debug.LogError("m_followTargets is NULL");
                return;
            }

            if (m_followTargets != null && m_followTargets.Count == 0)
            {
                Debug.LogError("m_followTargets is EMPTY");
                return;
            }

            /// Don't set this to Linq, which could be stripped in build
            foreach (var t in m_followTargets)
            {
                if (t == null)
                {
                    Debug.LogError("A follow target element is NULL");
                    return;
                }
            }

            GetFirstActiveTargetFromList();

            if (_followTarget == null)
            {
                Debug.Log($"We should never have no {nameof(_followTarget)} for {gameObject.name}");

                return;
            }

            if (!_followTarget.gameObject.activeSelf)
            {
                Debug.LogWarning($"The chosen {nameof(_followTarget)} is not active, this may be a problem for tracking!");
            }

            var delay = 0;

            if (m_delay != null)
            {
                delay = m_delay;
            }

            var delaySec = delay / 1000f;

            var positionOffset = Vector3.zero;

            if (m_positionOffset != null)
            {
                positionOffset = m_positionOffset;
            }

            var newPosition = _followTarget.position + m_rotationOffset.transform.rotation * positionOffset; // To make sure a parented object behaves when the parent rotates.

            var newRotation = _followTarget.rotation;

            if (_invoker == null)
            {
                Debug.LogError("No invoker provided");

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
