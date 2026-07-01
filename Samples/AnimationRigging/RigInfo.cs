using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(IRigConstraint))]
    public abstract class RigInfo : MonoBehaviour
    {
        [HideInInspector] public Rigger Rigger;

        protected IRigConstraint _iConstraint;
        protected BoneRenderer _boneRenderer;


        protected virtual void Awake()
        {
            _iConstraint ??= GetComponent<IRigConstraint>();
            _boneRenderer ??= GetComponent<BoneRenderer>();
        }


        public abstract bool GetTransforms();


        protected void RenderBones(Transform start, Transform end)
        {
            this.Warning("This currently doesn't work yet. Due to BoneRenderer.transforms being read-only when in Build..?");

            /*if (_boneRenderer == null)
            {
                return;
            }

            var bones = new List<Transform>();
            var current = end;

            while (current != null)
            {
                bones.Add(current);

                if (current == start)
                {
                    break;
                }

                current = GetParent(current);
            }

            bones.Reverse();

            _boneRenderer.transforms = bones.ToArray();*/
        }


        private Transform GetParent(Transform child)
        {
            return child == null ? null : child.parent;
        }
    }
}