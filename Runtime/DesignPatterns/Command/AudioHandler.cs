using UnityEngine;


namespace SOSXR.SeaShark
{
    public class AudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioClip m_audioClip;
        [SerializeField] private Player m_player;
        private Command _invoker;

        private void OnValidate()
        {
          if (m_player == null)
          {
              m_player = GetComponent<Player>();
          }
        }

        private void Awake()
        {
            _invoker = new CommandQueue();
        }


        [ContextMenu(nameof(PlayAudio))]
        private void PlayAudio()
        {
            _invoker.Execute(new AudioCommand(m_player, m_audioClip));
        }
    }
}