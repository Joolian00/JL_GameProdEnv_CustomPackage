using System.Collections;

using Julian.RBController.Components;
using Julian.RBController.ScriptableObjects;
using Julian.RBController.States;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Julian.RBController
{
    public class RigidbodyController : MonoBehaviour
    {
        #region Fields

        #region Configuration Fields (Exposed in Inspector)

        [Header("Core References")] 
        [Header("Camera")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform cameraPositionTarget;
        [SerializeField] private Transform cameraLookTarget;
        [Header("Model")]
        [SerializeField] private Transform modelPivot;
        [SerializeField] private GameObject model;
        
        [Header("Settings")]
        [SerializeField] private RigidbodyControllerSettings settings;
        
        #endregion

        #region Public Properties (Read-Only Access for States/Components)

        public Rigidbody Rb { get; private set; }
        public Animator PlayerAnimator { get; private set; }
        public InputHandler InputHandler { get; private set; }
        
        public Camera PlayerCamera => playerCamera;
        public Transform ModelPivot => modelPivot;
        public Transform CameraPivot => cameraPivot;
        public Transform CameraPositionTarget => cameraPositionTarget;
        public Transform CameraLookTarget => cameraLookTarget;
        
        public PlayerStateMachine StateMachine { get; private set; }
        
        #endregion
        
        #region Components
        
        [Header("Components")]
        [Space(5)]
        [Header("\u26a0\ufe0f WARNING: Changes made here will be reset on play! \u26a0\ufe0f\n\u2705 Use the ScriptableObject settings instead!")]
        [Space(5)]
        [SerializeField] private RbcPhysics rbcPhysics;
        [SerializeField] private RbcMovement rbcMovement;
        [SerializeField] private RbcHeightSpring rbcHeightSpring;
        [SerializeField] private RbcGroundChecker rbcGroundChecker;
        [SerializeField] private RbcCamera rbcCamera;
        [SerializeField] private RbcShooting rbcShooting;
        
        // Public properties for accessing components. Only for serialization for easy editing in the inspector during runtime.
        // Will (or rather should) be removed and only scriptable objects will be used instead.
        public RbcPhysics RbcPhysics => rbcPhysics;
        public RbcMovement RbcMovement => rbcMovement;
        public RbcHeightSpring RbcHeightSpring => rbcHeightSpring;
        public RbcGroundChecker RbcGroundChecker => rbcGroundChecker;
        public RbcCamera RbcCamera => rbcCamera;
        public RbcShooting RbcShooting => rbcShooting;

        #endregion
        
        #endregion

        #region EventHandlers

        public void OnMove(InputAction.CallbackContext context) => InputHandler?.OnMove(context);
        public void OnJump(InputAction.CallbackContext context) => InputHandler?.OnJump(context);
        public void OnLook(InputAction.CallbackContext context) => InputHandler?.OnLook(context);
        public void OnShoot(InputAction.CallbackContext context) => InputHandler?.OnShoot(context);

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeCoreComponents();
            InitializeModularComponents();
            InitializeStateMachine();
            
            
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdateState();
        }

        private void Update()
        {
            StateMachine.UpdateState();
        }

        private void LateUpdate()
        {
            StateMachine.LateUpdateState();
        }

        #endregion

        #region Initialization

        private void InitializeCoreComponents()
        {
            Rb = GetComponent<Rigidbody>();
            PlayerAnimator = model.GetComponent<Animator>();
            InputHandler = new InputHandler();
        }
        
        private void InitializeModularComponents()
        {
            rbcCamera = new RbcCamera(cameraPivot, cameraPositionTarget, cameraLookTarget, settings.Camera);
            rbcPhysics = new RbcPhysics(Rb, settings.Physics);
            rbcGroundChecker = new RbcGroundChecker(Rb, settings.GroundChecker);
            rbcHeightSpring = new RbcHeightSpring(Rb, rbcGroundChecker, settings.HeightSpring);
            rbcMovement = new RbcMovement(Rb, modelPivot, playerCamera.transform, settings.Movement);
            rbcShooting = new RbcShooting(Rb.transform, playerCamera.transform, settings.Shooting);
        }

        private void InitializeStateMachine()
        {
            StateMachine = new PlayerStateMachine(this);

            // Define states
            var lobbyState = new PlayerLobbyState(StateMachine, this);
            var idleState = new PlayerIdleState(StateMachine, this);
            var movingState = new PlayerMovingState(StateMachine, this);
            var jumpingState = new PlayerJumpingState(StateMachine, this);
            var stunnedState = new PlayerStunnedState(StateMachine, this);

            // Register states with state machine
            StateMachine.RegisterState(PlayerStateType.Lobby, lobbyState);
            StateMachine.RegisterState(PlayerStateType.Idle, idleState);
            StateMachine.RegisterState(PlayerStateType.Moving, movingState);
            StateMachine.RegisterState(PlayerStateType.Jumping, jumpingState);
            StateMachine.RegisterState(PlayerStateType.Stunned, stunnedState);

            // Set initial state
            StateMachine.ChangeState(PlayerStateType.Lobby);
        }

        #endregion

        #region Public Methods

        public void Stun(float duration)
        {
            StateMachine.ChangeState(PlayerStateType.Stunned);
            StartCoroutine(StunRoutine(duration));
        }

        private IEnumerator StunRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (StateMachine.CurrentStateType == PlayerStateType.Stunned)
                StateMachine.ChangeState(PlayerStateType.Idle);
        }

        #endregion

        #region Gizmos
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(cameraPositionTarget.position, 0.1f);
            Gizmos.DrawLine(cameraPivot.position, cameraPositionTarget.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(cameraLookTarget.position, 0.1f);
            Gizmos.DrawLine(cameraPivot.position, cameraLookTarget.position);
            
            // Draw the ground check sphere
            if (!Application.isPlaying) return;
            
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rbcGroundChecker.MaxGroundCheckDistance);

            if (rbcGroundChecker.IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, rbcGroundChecker.LastGroundHitPoint);
                Gizmos.DrawSphere(rbcGroundChecker.LastGroundHitPoint, rbcGroundChecker.SphereCastRadius);
            }
            else if (!rbcGroundChecker.IsGrounded)
            {
                Gizmos.color = Color.red;
                // Draw a line from the player to the max ground check distance
                Gizmos.DrawSphere(transform.position + Vector3.down * rbcGroundChecker.MaxGroundCheckDistance, rbcGroundChecker.SphereCastRadius);
            }
            
            
            
        }

        #endregion
    }
}