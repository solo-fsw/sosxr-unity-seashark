using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SOSXR.SeaShark.Patterns.Command
{
    public class InputHandler : MonoBehaviour
    {
        public InputActionReference MoveUpAction;
        public InputActionReference MoveDownAction;
        public InputActionReference UndoMoveAction;
        public InputActionReference RedoMoveAction;
        [SerializeField] private Player m_player;

        private Command _invoker;

        private Action<InputAction.CallbackContext> _moveUpDelegate;
        private Action<InputAction.CallbackContext> _moveDownDelegate;
        private Action<InputAction.CallbackContext> _undoDelegate;
        private Action<InputAction.CallbackContext> _redoDelegate;


        private void Awake()
        {
            if (m_player == null)
            {
                m_player = FindFirstObjectByType<Player>();
            }

            _invoker = new CommandStack();

            _moveUpDelegate = _ => MoveUp();
            _moveDownDelegate = _ => MoveDown();
            _undoDelegate = _ => Undo();
            _redoDelegate = _ => Redo();
        }


        private void OnEnable()
        {
            MoveUpAction.action.performed += _moveUpDelegate;
            MoveDownAction.action.performed += _moveDownDelegate;
            UndoMoveAction.action.performed += _undoDelegate;
            RedoMoveAction.action.performed += _redoDelegate;
        }


        private void Undo()
        {
            _invoker.Undo();
        }


        private void Redo()
        {
            _invoker.Redo();
        }


        private void MoveDown()
        {
            _invoker.Execute(new MoveCommand(m_player, Vector3.down, 1f));
        }


        private void MoveUp()
        {
            _invoker.Execute(new MoveCommand(m_player, Vector3.up, 1f));
        }


        private void OnDisable()
        {
            MoveUpAction.action.performed -= _moveUpDelegate;
            MoveDownAction.action.performed -= _moveDownDelegate;
            UndoMoveAction.action.performed -= _undoDelegate;
            RedoMoveAction.action.performed -= _redoDelegate;
        }
    }


    public class AudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioClip m_audioClip;
        [SerializeField] private Player m_player;
        private Command _invoker;


        private void Awake()
        {
            if (m_player == null)
            {
                m_player = FindFirstObjectByType<Player>();
            }

            _invoker = new CommandQueue();
        }


        [ContextMenu(nameof(PlayAudio))]
        private void PlayAudio()
        {
            _invoker.Execute(new AudioCommand(m_player, m_audioClip));
        }
    }
}