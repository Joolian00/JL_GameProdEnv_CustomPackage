using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.States
{
    public class PlayerIdleState : PlayerBaseState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        public PlayerIdleState(PlayerStateMachine stateMachine, RigidbodyController controller) : base(stateMachine, controller) {}
        
        public override void EnterState()
        {
            Debug.Log("Entered PlayerIdleState");
            if(Controller.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
            Controller.PlayerAnimator.SetTrigger(Idle);
        }
        
        public override void ExitState()
        {
        }
        
        public override void FixedUpdateState()
        {
            // Call base implementation first to apply gravity
            base.FixedUpdateState();
            
            Controller.RbcMovement.ApplyFriction();
            Controller.RbcHeightSpring.HandleHeightSpring();
        }

        public override void CheckSwitchState()
        {
            if (Controller.InputHandler.MoveInput.magnitude > 0.01f)
            {
                StateMachine.ChangeState(PlayerStateType.Moving);
            } 
            else if (Controller.InputHandler.JumpPressed)
            {
                StateMachine.ChangeState(PlayerStateType.Jumping);
            }
        }
    }
}