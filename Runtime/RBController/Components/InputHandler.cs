using UnityEngine;
using UnityEngine.InputSystem;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.Components
{
    public class InputHandler
    {
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpButtonWasReleased { get; private set; }
        public bool ShootPressed { get; private set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpPressed = context.performed;
            JumpButtonWasReleased = context.canceled;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }
        
        public void OnShoot(InputAction.CallbackContext context)
        {
            ShootPressed = context.performed;
        }
    }
}