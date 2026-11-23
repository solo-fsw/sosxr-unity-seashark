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
            if (m_animator == null)
            {
                return;
            }

            var avatar = GetAvatar();

            // Keep the Animator's current avatar until building one is implemented.
            if (avatar != null)
            {
                m_animator.avatar = avatar;
            }
        }


        private Avatar GetAvatar()
        {
            // return HumanoidAvatarBuilder.Build(gameObject);

            Debug.LogWarning("Building an avatar is not implemented yet (needs the Avaturn package). The Animator keeps its current avatar.", this);

            return null;
        }
    }
}
