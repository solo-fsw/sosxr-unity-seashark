using System;
using System.Threading.Tasks;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class CopyPositionCommand : ICommand
    {
        private readonly Player _player;
        private readonly float _duration;
        private readonly Vector3 _position;


        public CopyPositionCommand(Player player, Vector3 position, float duration)
        {
            _player = player;
            _position = position;
            _duration = duration;
        }


        public async void Execute()
        {
            try
            {
                await CommandOverTime(_position);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }


        public async void Undo()
        {
            try
            {
                await CommandOverTime(-_position);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }


        public void Redo()
        {
            Execute();
        }


        private async Task CommandOverTime(Vector3 newPosition)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            if (_player != null)
            {
                _player.Move(newPosition);
            }
        }
    }
}