using UnityEngine;


namespace SOSXR.SeaShark
{
    public class AnimatorHelpers : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;


        private void Awake()
        {
            if (m_animator == null)
            {
                m_animator = GetComponent<Animator>();
            }
        }


        private void Start()
        {
            m_animator.avatar = GetAvatar();
        }


        private Avatar GetAvatar()
        {
            // return HumanoidAvatarBuilder.Build(gameObject);

            Debug.LogError("Currently not implemented. Needs Avaturn package.");

            return null;
        }
    }
}
