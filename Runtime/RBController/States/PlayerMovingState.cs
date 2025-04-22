using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.States
{
    public class PlayerMovingState : PlayerBaseState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        public PlayerMovingState(PlayerStateMachine stateMachine, RigidbodyController controller) : base(stateMachine, controller) {}
        
        public override void EnterState()
        {
            Debug.Log("Entered PlayerMovingState");
            if(Controller.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;

            Controller.PlayerAnimator.SetTrigger(Walk);
        }
        
        public override void ExitState()
        {
        }
        
        public override void FixedUpdateState()
        {
            // Call base implementation first to apply gravity
            base.FixedUpdateState();
            
            Controller.RbcHeightSpring.HandleHeightSpring();
            
            Controller.RbcMovement.HandleJumping(Controller.InputHandler.JumpPressed, Controller.InputHandler.JumpButtonWasReleased, Controller.RbcGroundChecker.IsGrounded);
            Controller.RbcMovement.HandleMovement(Controller.InputHandler.MoveInput);
        }
        

        
        public override void CheckSwitchState()
        {
            if (Controller.InputHandler.MoveInput.magnitude < 0.01f)
            {
                StateMachine.ChangeState(PlayerStateType.Idle);
            } 
            else if (Controller.InputHandler.JumpPressed)
            {
                StateMachine.ChangeState(PlayerStateType.Jumping);
            }
        }
    }
}