using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(BoneRenderer))]
    public class ChainIKInfo : RigInfo
    {
        [SerializeField] private string m_constrainedObjectName;
        [SerializeField] private string m_tipName;
        [DisableEditing] private Transform m_constrainedObject;
        [DisableEditing] private Transform m_tip;

        private ChainIKConstraint _constraint;


        protected override void Awake()
        {
            base.Awake();

            _constraint = (ChainIKConstraint) _iConstraint;

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
            _constraint.data.root = m_constrainedObject;

            m_tip = avatar.transform.FindChildByName(m_tipName);
            _constraint.data.tip = m_tip;

            RenderBones(_constraint.data.root, _constraint.data.tip);

            return _constraint.data.root != null && _constraint.data.tip != null;
        }
    }
}