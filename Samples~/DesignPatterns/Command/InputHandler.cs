using System;
using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


namespace SOSXR.SeaShark
{
    public class InputHandler : MonoBehaviour
    {
        public InputActionProperty MoveUpAction;
        public InputActionProperty MoveDownAction;
        public InputActionProperty MoveLeftAction;
        public InputActionProperty MoveRightAction;
        public InputActionProperty UndoMoveAction;
        public InputActionProperty RedoMoveAction;

        [SerializeField] private Player m_player;
        [SerializeField] [Range(0f, 5f)] private float m_moveDelay = 1f;

        private Command _invoker;

        private Action<InputAction.CallbackContext> _moveUpDelegate;
        private Action<InputAction.CallbackContext> _moveDownDelegate;
        private Action<InputAction.CallbackContext> _moveLeftDelegate;
        private Action<InputAction.CallbackContext> _moveRightDelegate;
        private Action<InputAction.CallbackContext> _undoDelegate;
        private Action<InputAction.CallbackContext> _redoDelegate;


        private void Awake()
        {
            if (m_player == null)
            {
                m_player = FindFirstObjectByType<Player>();
            }

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                this.Warning("Had to spawn Event System, since none was available. ");
                var o = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            }

            _invoker = new CommandStack();

            _moveUpDelegate = _ => MoveUp();
            _moveDownDelegate = _ => MoveDown();
            _moveLeftDelegate = _ => MoveLeft();
            _moveRightDelegate = _ => MoveRight();
            _undoDelegate = _ => Undo();
            _redoDelegate = _ => Redo();
        }


        private void OnEnable()
        {
            MoveUpAction.action.Enable();
            MoveDownAction.action.Enable();
            MoveLeftAction.action.Enable();
            MoveRightAction.action.Enable();
            UndoMoveAction.action.Enable();
            RedoMoveAction.action.Enable();


            var map = MoveUpAction.action.actionMap;

            if (map is {enabled: false})
            {
                map.Enable();
            }

            MoveUpAction.action.performed += _moveUpDelegate;
            MoveDownAction.action.performed += _moveDownDelegate;
            MoveLeftAction.action.performed += _moveLeftDelegate;
            MoveRightAction.action.performed += _moveRightDelegate;
            UndoMoveAction.action.performed += _undoDelegate;
            RedoMoveAction.action.performed += _redoDelegate;
        }


        private void MoveUp()
        {
            this.Verbose("Calling MoveUp");
            _invoker.Execute(new MoveCommand(m_player, new Vector3(0, 1, 0), m_moveDelay));
        }


        private void MoveDown()
        {
            this.Verbose("Calling MoveDown");
            _invoker.Execute(new MoveCommand(m_player, new Vector3(0, -1, 0), m_moveDelay));
        }


        private void MoveLeft()
        {
            this.Verbose("Calling MoveLeft");
            _invoker.Execute(new MoveCommand(m_player, new Vector3(-1, 0, 0), m_moveDelay));
        }


        private void MoveRight()
        {
            this.Verbose("Calling MoveRight");
            _invoker.Execute(new MoveCommand(m_player, new Vector3(1, 0, 0), m_moveDelay));
        }


        private void Undo()
        {
            this.Verbose("Calling Undo");
            _invoker.Undo();
        }


        private void Redo()
        {
            this.Verbose("Calling Redo");
            _invoker.Redo();
        }


        private void OnDisable()
        {
            _invoker.Clear();

            MoveUpAction.action.performed -= _moveUpDelegate;
            MoveDownAction.action.performed -= _moveDownDelegate;
            MoveLeftAction.action.performed -= _moveLeftDelegate;
            MoveRightAction.action.performed -= _moveRightDelegate;
            UndoMoveAction.action.performed -= _undoDelegate;
            RedoMoveAction.action.performed -= _redoDelegate;

            MoveUpAction.action.Disable();
            MoveDownAction.action.Disable();
            MoveLeftAction.action.Disable();
            MoveRightAction.action.Disable();
            UndoMoveAction.action.Disable();
            RedoMoveAction.action.Disable();

            var map = MoveUpAction.action.actionMap;

            if (map is {enabled: true})
            {
                map.Disable();
            }
        }
    }
}



namespace SOSXR.SeaShark
{
}