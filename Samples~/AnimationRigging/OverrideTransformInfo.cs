using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.SeaShark
{
    public class OverrideTransformInfo : RigInfo
    {
        [SerializeField] private string m_constrainedObjectName;
        [DisableEditing] private Transform m_constrainedObject;

        private OverrideTransform _constraint;


        protected override void Awake()
        {
            base.Awake();

            _constraint = (OverrideTransform) _iConstraint;

            if (_constraint == null)
            {
                this.Error($"Cannot find constraint of type {_constraint.GetType()}");
            }
        }


        [Button]
        public override bool GetTransforms()
        {
            var avatar = Rigger.Animator.transform;
            m_constrainedObject = avatar.FindChildByName(m_constrainedObjectName);
            _constraint.data.constrainedObject = m_constrainedObject;

            return _constraint.data.constrainedObject != null;
        }
    }
}