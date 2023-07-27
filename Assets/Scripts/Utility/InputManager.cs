using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        private float horizontalMoveAxis;
        public float HorizontalMoveAxis
        {
            get { return horizontalMoveAxis; }
            private set { horizontalMoveAxis = value; }
        }

        private float verticalMoveAxis;
        public float VerticalMoveAxis
        {
            get { return verticalMoveAxis; }
            private set { verticalMoveAxis = value; }
        }

        private float horizontalLookAxis;
        public float HorizontalLookAxis
        {
            get { return horizontalLookAxis; }
            private set { horizontalLookAxis = value; }
        }

        private float verticalLookAxis;
        public float VerticalLookAxis
        {
            get { return verticalLookAxis; }
            private set { verticalLookAxis = value; }
        }

        private bool firePressed;
        public bool FirePressed
        {
            get { return firePressed; }
            private set { firePressed = value; }
        }

        private bool fireHeld;
        public bool FireHeld
        {
            get { return fireHeld; }
            private set { fireHeld = value; }
        }

        private bool jumpPressed;
        public bool JumpPressed
        {
            get { return jumpPressed; }
            private set { jumpPressed = value; }
        }

        private bool jumpHeld;
        public bool JumpHeld
        {
            get { return jumpHeld; }
            private set { jumpHeld = value; }
        }

        private bool ePressed;
        public bool EPressed
        {
            get { return ePressed; }
            private set { ePressed = value; }
        }

        private bool eHeld;
        public bool EHeld
        {
            get { return eHeld; }
            private set { eHeld = value; }
        }

        private bool pausePressed;
        public bool PausePressed
        {
            get { return pausePressed; }
            private set { pausePressed = value; }
        }

        private float cycleWeaponInput;
        public float CycleWeaponInput
        {
            get { return cycleWeaponInput; }
            private set { cycleWeaponInput = value; }
        }

        private bool nextWeaponPressed;
        public bool NextWeaponPressed
        {
            get { return nextWeaponPressed; }
            private set { nextWeaponPressed = value; }
        }

        private bool previousWeaponPressed;
        public bool PreviousWeaponPressed
        {
            get { return previousWeaponPressed; }
            private set { previousWeaponPressed = value; }
        }




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