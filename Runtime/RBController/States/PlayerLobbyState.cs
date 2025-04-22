using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.States
{
    public class PlayerLobbyState : PlayerBaseState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        public PlayerLobbyState(PlayerStateMachine stateMachine, RigidbodyController controller) : base(stateMachine, controller) {}
        
        public override void EnterState()
        {
            Debug.Log("Entered PlayerLobbyState");
            Controller.PlayerAnimator.SetTrigger(Idle);
        }
        
        public override void ExitState()
        {
            Controller.RbcCamera.ResetPositionInitialization();
        }
        
        public override void FixedUpdateState()
        {
            Controller.RbcPhysics.ApplyGravity();
            Controller.RbcHeightSpring.HandleHeightSpring();
            Controller.RbcMovement.HandleModelRotation(Controller.InputHandler.MoveInput, false);
        }
        
        public override void UpdateState()
        {
        }
        
        public override void LateUpdateState()
        {
        }

        public override void CheckSwitchState()
        {
            // If f12 is pressed, switch to idle state
            if (Input.GetKeyDown(KeyCode.F12))
            {
                StateMachine.ChangeState(PlayerStateType.Idle);
            }
        }
    }
}