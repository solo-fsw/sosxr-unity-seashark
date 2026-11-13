using System;
using System.Threading.Tasks;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class MoveCommand : ICommand
    {
        private readonly Player _player;
        private readonly float _duration;
        private readonly Vector3 _direction;


        public MoveCommand(Player player, Vector3 direction, float duration)
        {
            _player = player;
            _direction = direction;
            _duration = duration;
        }


        public async void Execute()
        {
            try
            {
                await CommandOverTime(_direction);
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
                await CommandOverTime(-_direction);
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


        private async Task CommandOverTime(Vector3 direction)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var elapsedTime = 0f;
            var startPosition = _player.transform.position;
            var endPosition = startPosition + direction;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            if (_player != null)
            {
                _player.Move(endPosition);
            }
        }
    }
}