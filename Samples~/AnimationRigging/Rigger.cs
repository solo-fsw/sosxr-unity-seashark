using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.SeaShark
{
    public class Rigger : MonoBehaviour
    {
        public Animator Animator;
        private RigInfo[] m_rigs = { };


        private void Awake()
        {
            m_rigs = GetComponentsInChildren<RigInfo>();

            foreach (var rig in m_rigs)
            {
                rig.Rigger = this;
            }
        }


        [Button]
        public void MakeRig()
        {
            var m_rigBuilder = Animator.gameObject.AddComponent<RigBuilder>();
            m_rigBuilder.layers.Clear();

            var rigHolder = new GameObject("RigHolder");
            rigHolder.transform.parent = m_rigBuilder.gameObject.transform;

            foreach (var desiredRig in m_rigs)
            {
                if (!desiredRig.GetTransforms())
                {
                    this.Error("Not successful in getting what we need.");

                    return;
                }

                var newRigGO = Instantiate(desiredRig, rigHolder.transform, true);
                var rigInfo = newRigGO.GetComponent<RigInfo>();

                if (rigInfo == null)
                {
                    this.Error("Whu is this null?");

                    return;
                }

                var rig = newRigGO.GetComponent<Rig>();
                m_rigBuilder.layers.Add(new RigLayer(rig));
            }

            Animator.Rebind();
            m_rigBuilder.Build();
        }
    }
}