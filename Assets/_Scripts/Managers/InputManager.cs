using Managers;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        public Vector2 MoveInput=> moveInput;
        public Vector2 LookInput=> lookInput;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpPressed;
        private bool interactPressed;
        private bool pausePressed;

        public void ReadMovementInput(InputAction.CallbackContext context)
        {
            if(Time.timeScale==0)
                return;
            moveInput = context.ReadValue<Vector2>();
        }
        public void ReadLookInput(InputAction.CallbackContext context)
        {
            if(Time.timeScale==0)
                return;
            lookInput = context.ReadValue<Vector2>();
        }

        public void ReadJumpInput(InputAction.CallbackContext context)
        {
            if(context.started)
                EventManager.RaiseEvent(new JumpEventArgs());
        }

        public void ReadInteractInput(InputAction.CallbackContext context)
        {
            if(context.started)
                EventManager.RaiseEvent(new InteractEventArgs());
        }

        public void ReadPauseInput(InputAction.CallbackContext context)
        {
            EventManager.RaiseEvent( new PauseEventArgs());
        }
    }
}
