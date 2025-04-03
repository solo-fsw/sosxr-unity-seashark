using UnityEngine;


namespace SOSXR.SeaShark.Patterns.Command
{
    public class AudioCommand : ICommand
    {
        private readonly Player _player;
        private readonly AudioClip _audioClip;


        public AudioCommand(Player player, AudioClip audioClip)
        {
            _player = player;
            _audioClip = audioClip;
        }


        public void Execute()
        {
            _player.PlayAudio(_audioClip);
        }


        public void Undo()
        {
            // No undo for audio
        }


        public void Redo()
        {
            // No redo for audio
        }
    }
}