using Julian.RBController.Components;

using UnityEngine;

namespace Julian.RBController.States
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateMachine StateMachine { get; }
        protected RigidbodyController Controller { get; }

        protected PlayerBaseState(PlayerStateMachine stateMachine, RigidbodyController controller)
        {
            StateMachine = stateMachine;
            Controller = controller;
        }
        public abstract void EnterState();
        public abstract void ExitState();
        
        // Provide a default implementation for FixedUpdate that applies gravity
        public virtual void FixedUpdateState()
        {
            // Apply gravity by default in all states unless overridden
            Controller.RbcPhysics.ApplyGravity();
            
            Controller.RbcGroundChecker.PlayerGroundCheck();
            
            Controller.RbcHeightSpring.HandleUprightTorque();
            
            Controller.RbcMovement.HandleModelRotation(Controller.InputHandler.MoveInput, Controller.InputHandler.ShootPressed);
            

        }
        
        public virtual void UpdateState()
        {
            Controller.RbcShooting.HandleShooting(Controller.InputHandler.ShootPressed);
        }
        
        public virtual void LateUpdateState()
        {
            Controller.RbcCamera.PositionCameraPivot(Controller.Rb.position);
            Controller.RbcCamera.HandleLookInput(Controller.InputHandler.LookInput);
        }
        
        public abstract void CheckSwitchState();
    }
}