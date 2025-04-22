using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.States
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        public PlayerJumpingState(PlayerStateMachine stateMachine, RigidbodyController controller) : base(stateMachine, controller) {}
        
        public override void EnterState()
        {
            Debug.Log("Entered PlayerJumpingState");
        }
        
        public override void ExitState()
        {
        }
        
        public override void FixedUpdateState()
        {
            // Call base implementation first to apply gravity
            base.FixedUpdateState();
            
            Controller.RbcMovement.CheckJumpReset(Controller.RbcGroundChecker.IsGrounded);
            
            if (!Controller.RbcMovement.IsJumping && !Controller.InputHandler.JumpButtonWasReleased)
            {
                Controller.RbcHeightSpring.HandleHeightSpring();
            }
            
            Controller.RbcMovement.HandleJumping(Controller.InputHandler.JumpPressed, Controller.InputHandler.JumpButtonWasReleased, Controller.RbcGroundChecker.IsGrounded);
            Controller.RbcMovement.HandleMovement(Controller.InputHandler.MoveInput);
        }
        
        public override void CheckSwitchState()
        {
            // Check if player has completed the jump sequence and is on the ground
            if (Controller.RbcGroundChecker.IsGrounded && 
                !Controller.RbcMovement.IsJumping && 
                !Controller.RbcMovement.IsJumpForceActive && 
                Controller.RbcMovement.IsPlayerFalling &&
                Controller.InputHandler.JumpButtonWasReleased)
            {
                // Check if there's movement input to determine next state
                if (Controller.InputHandler.MoveInput.magnitude > 0.1f)
                {
                    StateMachine.ChangeState(PlayerStateType.Moving);
                }
                else
                {
                    StateMachine.ChangeState(PlayerStateType.Idle);
                }
            }
        }
    }
}