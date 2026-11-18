using UnityEngine;


namespace SOSXR.SeaShark
{
    public class CopyPosition : MonoBehaviour
    {
        [SerializeField] private GameObject m_followTarget;

        [SerializeField] private Player m_player;
        [SerializeField] [Range(0f, 5f)] private float m_moveDelay = 1f;

        private Command _invoker;


        private void Awake()
        {
            if (m_player == null)
            {
                m_player = FindAnyObjectByType<Player>();
            }

            _invoker = new CommandStack();
        }


        private void Update()
        {
            _invoker.Execute(new CopyPositionCommand(m_player, m_followTarget.transform.position, m_moveDelay));
        }


        private void OnDisable()
        {
            _invoker.Clear();
        }
    }
}