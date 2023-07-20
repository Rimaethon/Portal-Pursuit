using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

            private float horizontalMoveAxis;
            private float verticalMoveAxis;
            private float horizontalLookAxis;
            private float verticalLookAxis;
            private bool firePressed;
            private bool fireHeld;
            private bool jumpPressed;
            private bool jumpHeld;
            private bool ePressed;
            private bool eHeld;
            private bool pausePressed;
            private float cycleWeaponInput;
            private bool nextWeaponPressed;
            private bool previousWeaponPressed;
            

        private void Awake()
        {
            // Set up the instance of this
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

 
        public void ReadMovementInput(InputAction.CallbackContext context)
        {
            var inputVector = context.ReadValue<Vector2>();
            horizontalMoveAxis = inputVector.x;
            verticalMoveAxis = inputVector.y;
        }

   
        public void ReadLookInput(InputAction.CallbackContext context)
        {
            var inputVector = context.ReadValue<Vector2>();
            horizontalLookAxis = inputVector.x;
            verticalLookAxis = inputVector.y;
        }

 
        public void ReadFireInput(InputAction.CallbackContext context)
        {
            firePressed = !context.canceled;
            fireHeld = !context.canceled;
            StartCoroutine("ResetFireStart");
        }

 
        private IEnumerator ResetFireStart()
        {
            yield return new WaitForEndOfFrame();
            firePressed = false;
        }


        public void ReadJumpInput(InputAction.CallbackContext context)
        {
            jumpPressed = !context.canceled;
            jumpHeld = !context.canceled;
            StartCoroutine("ResetJumpStart");
        }

        public void ReadInteractInput(InputAction.CallbackContext context)
        {
            ePressed = !context.canceled;
            StartCoroutine(ResetInteractPressed());
        }

 
        private IEnumerator ResetJumpStart()
        {
            yield return new WaitForEndOfFrame();
            jumpPressed = false;
        }

        private IEnumerator ResetInteractPressed()
        {
            yield return new WaitForEndOfFrame();
            ePressed = false;
        }


        public void ReadPauseInput(InputAction.CallbackContext context)
        {
            pausePressed = !context.canceled;
            StartCoroutine(ResetPausePressed());
        }


        private IEnumerator ResetPausePressed()
        {
            yield return new WaitForEndOfFrame();
            pausePressed = false;
        }

        public void ReadCycleWeaponInput(InputAction.CallbackContext context)
        {
            var mouseScrollInput = context.ReadValue<Vector2>();
            if (mouseScrollInput.y == 0)
                cycleWeaponInput = 0;
            else
                cycleWeaponInput = Mathf.Sign(mouseScrollInput.y);
        }

        public void ReadNextWeaponInput(InputAction.CallbackContext context)
        {
            nextWeaponPressed = !context.canceled;
            StartCoroutine("ResetNextWeaponPressedStart");
        }

 
        private IEnumerator ResetNextWeaponPressedStart()
        {
            yield return new WaitForEndOfFrame();
            nextWeaponPressed = false;
        }


        public void ReadPreviousWeaponInput(InputAction.CallbackContext context)
        {
            previousWeaponPressed = !context.canceled;
            StartCoroutine("ResetPreviousWeaponPressedStart");
        }

        private IEnumerator ResetPreviousWeaponPressedStart()
        {
            yield return new WaitForEndOfFrame();
            previousWeaponPressed = false;
        }
    }
}