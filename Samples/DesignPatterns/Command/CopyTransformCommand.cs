using System;
using System.Threading.Tasks;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class CopyTransformCommand : ICommand
    {
        private readonly Transform _toMove;
        private readonly Vector3 _newPosition;
        private readonly Quaternion _newRotation;
        private readonly float _duration;


        public CopyTransformCommand(Transform toMove, Vector3 newPosition, Quaternion newRotation, float duration)
        {
            _toMove = toMove;
            _newPosition = newPosition;
            _newRotation = newRotation;
            _duration = duration;
        }


        public async void Execute()
        {
            try
            {
                await CommandOverTime(_newPosition, _newRotation);
            }
            catch (Exception e)
            {
                //Log.Static(e.Message, LogLevel.Warning);

                Debug.LogException(e);
            }
        }


        public async void Undo()
        {
            try
            {
                await CommandOverTime(-_newPosition, Quaternion.Inverse(_newRotation));
            }
            catch (Exception e)
            {
                //Log.Static(e.Message, LogLevel.Warning);
                Debug.LogException(e);
            }
        }


        public void Redo()
        {
            Execute();
        }


        private async Task CommandOverTime(Vector3 newPosition, Quaternion newRotation)
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

            if (_toMove != null)
            {
                _toMove.position = newPosition;

                _toMove.rotation = newRotation;
            }
        }
    }
}