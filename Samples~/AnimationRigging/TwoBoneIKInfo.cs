using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(BoneRenderer))]
    public class TwoBoneIKInfo : RigInfo
    {
        [SerializeField] private string m_rootName;
        [SerializeField] private string m_midName;
        [SerializeField] private string m_tipName;
        [DisableEditing] private Transform m_root;
        [DisableEditing] private Transform m_mid;
        [DisableEditing] private Transform m_tip;

        private TwoBoneIKConstraint _constraint;


        protected override void Awake()
        {
            base.Awake();

            _constraint = (TwoBoneIKConstraint) _iConstraint;

            if (_constraint == null)
            {
                this.Error($"Cannot find constraint of type {_constraint.GetType()}");
            }
        }


        [Button]
        public override bool GetTransforms()
        {
            var avatar = Rigger.Animator.transform;
            m_root = avatar.FindChildByName(m_rootName);
            _constraint.data.root = m_root;

            m_mid = avatar.FindChildByName(m_midName);
            _constraint.data.mid = m_mid;

            m_tip = avatar.FindChildByName(m_tipName);
            _constraint.data.tip = m_tip;

            RenderBones(_constraint.data.root, _constraint.data.tip);

            return _constraint.data.root != null && _constraint.data.mid != null && _constraint.data.tip != null;
        }
    }
}