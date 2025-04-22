using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.States
{
    public class PlayerStunnedState : PlayerBaseState
    {
        private static readonly int Struggle = Animator.StringToHash("Struggle");
        public PlayerStunnedState(PlayerStateMachine stateMachine, RigidbodyController controller) : base(stateMachine, controller) {}
        
        public override void EnterState()
        {
            Debug.Log("Entered PlayerStunnedState");
            Controller.PlayerAnimator.SetTrigger(Struggle);
        }
        
        public override void ExitState()
        {
            Controller.RbcMovement.HandleModelRotation(Controller.InputHandler.MoveInput, Controller.InputHandler.ShootPressed);
        }
        
        public override void FixedUpdateState()
        {
            Controller.RbcPhysics.ApplyGravity();
        }
        
        public override void UpdateState()
        {
        }
        
        public override void LateUpdateState()
        {
            Controller.RbcCamera.PositionCameraPivot(Controller.Rb.position);
            Controller.RbcCamera.HandleLookInput(Controller.InputHandler.LookInput);
        }
        
        public override void CheckSwitchState()
        {

        }
    }
}